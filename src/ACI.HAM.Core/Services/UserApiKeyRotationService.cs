using ACI.HAM.Core.Data;
using ACI.HAM.Core.Dtos;
using ACI.HAM.Mail.Dtos;
using ACI.HAM.Mail.Services;
using ACI.HAM.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Threading;
using System.Threading.Tasks;


namespace ACI.HAM.Core.Services
{
    public class UserApiKeyRotationService : BackgroundService
    {
        private readonly IOptions<ApiKeySettings> _configuration;
        private readonly ILogger<UserApiKeyRotationService> _logger;
        private readonly IMailService _mailService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UserApiKeyRotationService(IServiceScopeFactory serviceScopeFactory, ILogger<UserApiKeyRotationService> logger, IOptions<ApiKeySettings> configuration, IMailService mailService)
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
                _logger.LogInformation("Daily job will run at: {time}", nextMidnight);
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
#if DEBUG
                        .Where(x => (x.IsActive))
#else
                        .Where(x => (x.Expiration <= DateTimeOffset.Now.AddDays(_configuration.Value.DaysBeforeExpirationWarning)) && x.IsActive)
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
                _logger.LogError(ex, "Error occurred while running the scheduled task");
            }
        }
    }
}
