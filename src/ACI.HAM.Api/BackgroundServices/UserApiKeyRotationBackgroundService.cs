using ACI.HAM.Core.Data;
using ACI.HAM.Mail.Dtos;
using ACI.HAM.Mail.Services;
using ACI.HAM.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;


namespace ACI.HAM.Core.Services
{
    public class UserApiKeyRotationBackgroundService : BackgroundService
    {
        private readonly IOptions<ApiKeySettings> _configuration;
        private readonly ILogger<UserApiKeyRotationBackgroundService> _logger;
        private readonly IMailService _mailService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UserApiKeyRotationBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<UserApiKeyRotationBackgroundService> logger, IOptions<ApiKeySettings> configuration, IMailService mailService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _configuration = configuration;
            _mailService = mailService;
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
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var baseContext = scope.ServiceProvider.GetRequiredService<BaseContext>();
                    var userApiKeysToRotate = await baseContext.UserApiKeys
                        .Include(x => x.User)
                        .Where(x => (x.IsActive))
#if !DEBUG
                        .GroupBy(x => x.UserId)
                        .Select(x => x.OrderByDescending(y => y.Expiration).FirstOrDefault())
                        .Where(x => x.Expiration <= DateTimeOffset.Now.AddDays(_configuration.Value.DaysBeforeExpirationWarning))
#endif
                        .ToListAsync();
                    foreach (var userApiKey in userApiKeysToRotate)
                    {
                        SendApiKeyRotationMailDto sendApiKeyRotationMailDto = new SendApiKeyRotationMailDto()
                        {
                        };
                        await _mailService.SendApiKeyRotationMailAsync(sendApiKeyRotationMailDto);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while running the scheduled api key rotation task");
            }
        }
    }
}
