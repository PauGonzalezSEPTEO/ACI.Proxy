using ACI.HAM.Core.Data;
using ACI.HAM.Mail.Dtos;
using ACI.HAM.Mail.Services;
using ACI.HAM.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Linq;


namespace ACI.HAM.Core.Services
{
    public class UserApiKeyRotationBackgroundService : BackgroundService
    {
        private readonly IOptions<ApiKeySettings> _configuration;
        private readonly ILogger<UserApiKeyRotationBackgroundService> _logger;
        private readonly IMailService _mailService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly UISettings _uiSettings;

        public UserApiKeyRotationBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<UserApiKeyRotationBackgroundService> logger, IOptions<ApiKeySettings> configuration, IMailService mailService, IOptions<UISettings> uiSettings)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _configuration = configuration;
            _mailService = mailService;
            _uiSettings = uiSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
#if DEBUG
            await RunScheduledTaskAsync();
#endif
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextMidnight = now.Date.AddDays(1);
                var timeToMidnight = nextMidnight - now;
                _logger.LogInformation("Daily api key rotation job will run at: {time}", nextMidnight);
                await Task.Delay(timeToMidnight, stoppingToken);
                await RunScheduledTaskAsync();
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                    await RunScheduledTaskAsync();
                }
            }
        }

        private async Task RunScheduledTaskAsync()
        {
            CultureInfo originalCulture = CultureInfo.CurrentCulture;
            CultureInfo originalUICulture = CultureInfo.CurrentUICulture;
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var baseContext = scope.ServiceProvider.GetRequiredService<BaseContext>();
                    var userApiKeysToRotate = await baseContext.UserApiKeys
                        .Include(x => x.User)
                        .Where(x => (x.IsActive))
                        .GroupBy(x => x.UserId)
                        .Select(x => x.OrderByDescending(y => y.Expiration).FirstOrDefault())
                        .Where(x => x.Expiration <= DateTimeOffset.Now.AddDays(_configuration.Value.DaysBeforeExpirationWarning))
                        .ToListAsync();
                    Uri baseUri = new Uri(_uiSettings.BaseUrl);
                    Uri apiKeysUrl = new Uri(baseUri, _uiSettings.ApiKeysRelativeUrl);
                    foreach (var userApiKey in userApiKeysToRotate)
                    {
                        var daysUntilExpiration = (userApiKey.Expiration - DateTimeOffset.Now).Days;
                        if (daysUntilExpiration == _configuration.Value.DaysBeforeExpirationWarning || daysUntilExpiration == _configuration.Value.DaysBeforeExpirationSecondWarning || daysUntilExpiration == 0)
                        {
                            CultureInfo culture;
                            if (!string.IsNullOrEmpty(userApiKey.User.LanguageAlpha2Code))
                            {
                                culture = new CultureInfo(userApiKey.User.LanguageAlpha2Code);
                            }
                            else
                            {
                                culture = originalCulture;
                            }
                            CultureInfo.CurrentCulture = culture;
                            CultureInfo.CurrentUICulture = culture;
                            SendApiKeyRotationMailDto sendApiKeyRotationMailDto = new SendApiKeyRotationMailDto()
                            {
                                Expiration = userApiKey.Expiration,
                                Subject = Mail.Localization.DataAnnotations._API_Key_expiration_notice,
                                To = userApiKey.User.Email,
                                Url = apiKeysUrl.AbsoluteUri
                            };
                            await _mailService.SendApiKeyRotationMailAsync(sendApiKeyRotationMailDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while running the scheduled api key rotation task");
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
                CultureInfo.CurrentUICulture = originalUICulture;
            }
        }
    }
}
