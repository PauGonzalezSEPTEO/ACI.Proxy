using System.Threading.Tasks;
using System.Threading;
using ACI.Proxy.Core.Dtos;
using ACI.Proxy.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using ACI.Proxy.Core.Extensions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ACI.Proxy.Core.Data;
using ACI.Proxy.Settings;
using Microsoft.Extensions.Options;

namespace ACI.Proxy.Core.Services
{
    public interface IAccountService
    {
        Task<UserApiKeyDto> DeleteUserApiKeysByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<GenerateUserApiKeyResultDto> GenerateUserApiKeyAsync(string id, CancellationToken cancellationToken = default);

        Task<AccountResultDto> GetAccountAsync(string id, CancellationToken cancellationToken = default);

        Task<List<UserApiUsageStatisticDto>> GetLast12HoursUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default);

        Task<List<UserApiUsageStatisticDto>> GetLast14DaysUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default);

        Task<List<UserApiUsageStatisticDto>> GetLast3HoursUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default);

        Task<List<UserApiUsageStatisticDto>> GetLast3MonthsUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default);

        Task<List<UserApiUsageStatisticDto>> GetLast6HoursUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default);

        Task<List<UserApiUsageStatisticDto>> GetLast6MonthsUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default);

        Task<List<UserApiUsageStatisticDto>> GetLast7DaysUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default);

        Task<List<UserApiUsageStatisticDto>> GetLastDayUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default);

        Task<List<UserApiUsageStatisticDto>> GetLastHourUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default);

        Task<List<UserApiUsageStatisticDto>> GetLastMonthUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default);

        Task<List<UserApiUsageStatisticDto>> GetLastYearUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default);

        Task<DataTablesResult<UserApiKeyDto>> ReadUserApiKeysDataTableAsync(DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default);

        Task<UserApiKeyDto> RevokeUserApiKeysByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<UpdateProfileDetailsResultDto> UpdateProfileDetailsAsync(string id, ProfileDetailsDto profileDetailsDto, CancellationToken cancellationToken = default);
    }

    public class AccountService : IAccountService
    {
        private enum TimeInterval
        {
            Minute,
            Hour,
            Day
        }

        private readonly IOptions<ApiKeySettings> _apiKeySettings;
        private readonly BaseContext _baseContext;
        private readonly IDataProtector _dataProtector;
        private readonly IStringLocalizer<AccountService> _localizer;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public AccountService(UserManager<User> userManager, BaseContext baseContext, IMapper mapper, IStringLocalizer<AccountService> localizer, IDataProtectionProvider dataProtectionProvider, IOptions<ApiKeySettings> apiKeySettings)
        {
            _userManager = userManager;
            _baseContext = baseContext;
            _mapper = mapper;
            _localizer = localizer;
            _dataProtector = dataProtectionProvider.CreateProtector("ApiKey.ApiKeyProtector");
            _apiKeySettings = apiKeySettings;
        }

        public async Task<UserApiKeyDto> DeleteUserApiKeysByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            UserApiKey userApiKey = await _baseContext.UserApiKeys
                .SingleOrDefaultAsync(x => x.Id == id);
            _baseContext.Remove(userApiKey);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<UserApiKeyDto>(userApiKey);
        }

        public async Task<GenerateUserApiKeyResultDto> GenerateUserApiKeyAsync(string id, CancellationToken cancellationToken = default)
        {
            GenerateUserApiKeyResultDto generateUserApiKeyResultDto = new GenerateUserApiKeyResultDto();
            User user = await _userManager.FindByNameAsync(id);
            if (user == null)
            {
                generateUserApiKeyResultDto.HasErrors = true;
                generateUserApiKeyResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = _localizer["User does not exist"]
                    }
                };
            }
            else
            {                
                string encryptedEncryptionKey = _apiKeySettings.Value.EncryptionKey;
                string encryptionKey = _dataProtector.Unprotect(encryptedEncryptionKey);
                string apiKey = ApiKeyExtension.GenerateApiKey();
                string apiKeyLast6 = apiKey.Length >= 6 ? apiKey[^6..] : apiKey;
                var (hashedApiKey, salt) = ApiKeyExtension.HashApiKey(apiKey);
                string encryptedApiKey = ApiKeyExtension.EncryptApiKey(apiKey, encryptionKey);
                var userApiKey = new UserApiKey
                {
                    ApiKeyLast6 = apiKeyLast6,
                    EncryptedApiKey = encryptedApiKey,
                    HashedApiKey = hashedApiKey,
                    Salt = salt,
                    CreatedAt = DateTime.UtcNow,
                    Expiration = DateTime.UtcNow.AddDays(_apiKeySettings.Value.KeyExpirationInDays),
                    IsActive = true,
                    UserId = user.Id
                };
                _baseContext.UserApiKeys.Add(userApiKey);
                await _baseContext.SaveChangesAsync();
                generateUserApiKeyResultDto.ApiKey = apiKey;
            }
            return generateUserApiKeyResultDto;
        }

        public async Task<AccountResultDto> GetAccountAsync(string id, CancellationToken cancellationToken = default)
        {
            AccountResultDto readEditableResultDto = new AccountResultDto();
            User user = await _userManager.FindByNameAsync(id);
            return GetProfileDetails(user);
        }

        private AccountResultDto GetProfileDetails(User user, CancellationToken cancellationToken = default)
        {
            AccountResultDto readEditableResultDto = new AccountResultDto();
            if (user == null)
            {
                readEditableResultDto.HasErrors = true;
                readEditableResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = _localizer["User does not exist"]
                    }
                };
            }
            else
            {
                readEditableResultDto.Account = _mapper.Map<AccountDto>(user);
            }
            return readEditableResultDto;
        }

        public async Task<List<UserApiUsageStatisticDto>> GetLast12HoursUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await GetUserApiUsageStatisticsAsync(DateTimeOffset.Now.AddHours(-12), TimeInterval.Minute, cancellationToken);
        }

        public async Task<List<UserApiUsageStatisticDto>> GetLast14DaysUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await GetUserApiUsageStatisticsAsync(DateTimeOffset.Now.AddDays(-14), TimeInterval.Hour, cancellationToken);
        }

        public async Task<List<UserApiUsageStatisticDto>> GetLast3HoursUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await GetUserApiUsageStatisticsAsync(DateTimeOffset.Now.AddHours(-3), TimeInterval.Minute, cancellationToken);
        }

        public async Task<List<UserApiUsageStatisticDto>> GetLast3MonthsUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await GetUserApiUsageStatisticsAsync(DateTimeOffset.Now.AddMonths(-3), TimeInterval.Day, cancellationToken);
        }

        public async Task<List<UserApiUsageStatisticDto>> GetLast6HoursUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await GetUserApiUsageStatisticsAsync(DateTimeOffset.Now.AddHours(-6), TimeInterval.Minute, cancellationToken);
        }

        public async Task<List<UserApiUsageStatisticDto>> GetLast6MonthsUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await GetUserApiUsageStatisticsAsync(DateTimeOffset.Now.AddMonths(-6), TimeInterval.Day, cancellationToken);
        }

        public async Task<List<UserApiUsageStatisticDto>> GetLast7DaysUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await GetUserApiUsageStatisticsAsync(DateTimeOffset.Now.AddDays(-7), TimeInterval.Hour, cancellationToken);
        }

        public async Task<List<UserApiUsageStatisticDto>> GetLastDayUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await GetUserApiUsageStatisticsAsync(DateTimeOffset.Now.AddDays(-1), TimeInterval.Hour, cancellationToken);
        }

        public async Task<List<UserApiUsageStatisticDto>> GetLastHourUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await GetUserApiUsageStatisticsAsync(DateTimeOffset.Now.AddHours(-1), TimeInterval.Minute, cancellationToken);
        }

        public async Task<List<UserApiUsageStatisticDto>> GetLastMonthUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await GetUserApiUsageStatisticsAsync(DateTimeOffset.Now.AddMonths(-1), TimeInterval.Day, cancellationToken);
        }

        public async Task<List<UserApiUsageStatisticDto>> GetLastYearUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await GetUserApiUsageStatisticsAsync(DateTimeOffset.Now.AddDays(-365), TimeInterval.Day, cancellationToken);
        }

        private async Task<List<UserApiUsageStatisticDto>> GetUserApiUsageStatisticsAsync(DateTimeOffset fromDate, TimeInterval interval, CancellationToken cancellationToken = default)
        {
            IQueryable<UserApiUsageStatistic> query = _baseContext.UserApiUsageStatistics
                .Where(x => x.RequestTime >= fromDate);
            IQueryable<UserApiUsageStatisticDto> result;
            switch (interval)
            {
                case TimeInterval.Minute:
                    result = query.GroupBy(x => new { x.RequestTime.Date, x.RequestTime.Hour, x.RequestTime.Minute })
                                 .Select(x => new UserApiUsageStatisticDto
                                 {
                                     Date = new DateTime(x.Key.Date.Year, x.Key.Date.Month, x.Key.Date.Day, x.Key.Hour, x.Key.Minute, 0),
                                     Value = x.Count()
                                 });
                    break;
                case TimeInterval.Hour:
                    result = query.GroupBy(x => new { x.RequestTime.Date, x.RequestTime.Hour })
                                 .Select(x => new UserApiUsageStatisticDto
                                 {
                                     Date = new DateTime(x.Key.Date.Year, x.Key.Date.Month, x.Key.Date.Day, x.Key.Hour, 0, 0),
                                     Value = x.Count()
                                 });
                    break;
                case TimeInterval.Day:
                    result = query.GroupBy(x => x.RequestTime.Date)
                                 .Select(x => new UserApiUsageStatisticDto
                                 {
                                     Date = x.Key,
                                     Value = x.Count()
                                 });
                    break;
                default:
                    throw new ArgumentException(_localizer["Unsupported time interval"]);
            }
            return await result.ToListAsync();
        }

        public async Task<DataTablesResult<UserApiKeyDto>> ReadUserApiKeysDataTableAsync(DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default)
        {
            IQueryable<UserApiKey> query =
                _baseContext.UserApiKeys
                    .AsQueryable();
            return await query.GetDataTablesResultAsync<UserApiKey, UserApiKeyDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<UserApiKeyDto> RevokeUserApiKeysByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            UserApiKey userApiKey = await _baseContext.UserApiKeys
                .SingleOrDefaultAsync(x => x.Id == id);
            if (userApiKey != null)
            {
                userApiKey.IsActive = false;
                await _baseContext.SaveChangesAsync(cancellationToken);
            }
            return _mapper.Map<UserApiKeyDto>(userApiKey);
        }

        public async Task<UpdateProfileDetailsResultDto> UpdateProfileDetailsAsync(string id, ProfileDetailsDto profileDetailsDto, CancellationToken cancellationToken = default)
        {
            UpdateProfileDetailsResultDto updateProfileDetailsResultDto = new UpdateProfileDetailsResultDto();
            User user = await _userManager.FindByNameAsync(id);
            if (user == null)
            {
                updateProfileDetailsResultDto.HasErrors = true;
                updateProfileDetailsResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = _localizer["User does not exist"]
                    }
                };
            }
            else
            {
                _mapper.Map<ProfileDetailsDto, User>(profileDetailsDto, user);
                IdentityResult identityResult = await _userManager.UpdateAsync(user);
                if (!identityResult.Succeeded)
                {
                    updateProfileDetailsResultDto.HasErrors = true;
                    updateProfileDetailsResultDto.Errors = identityResult.Errors;
                }
            }
            return updateProfileDetailsResultDto;
        }
    }
}
