using System.Threading.Tasks;
using System.Threading;
using ACI.HAM.Core.Dtos;
using ACI.HAM.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using ACI.HAM.Core.Extensions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ACI.HAM.Core.Data;
using ACI.HAM.Settings;
using Microsoft.Extensions.Options;

namespace ACI.HAM.Core.Services
{
    public interface IAccountService
    {
        Task<UserApiKeyDto> DeleteUserApiKeysByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<GenerateUserApiKeyResultDto> GenerateUserApiKeyAsync(string id, CancellationToken cancellationToken = default);

        Task<AccountResultDto> GetAccountAsync(string id, CancellationToken cancellationToken = default);

        Task<List<UserApiUsageStatisticDto>> GetLastHourUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default);

        Task<DataTablesResult<UserApiKeyDto>> ReadUserApiKeysDataTableAsync(DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default);

        Task<UserApiKeyDto> RevokeUserApiKeysByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<UpdateProfileDetailsResultDto> UpdateProfileDetailsAsync(string id, ProfileDetailsDto profileDetailsDto, CancellationToken cancellationToken = default);
    }

    public class AccountService : IAccountService
    {
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

        public async Task<AccountResultDto> GetAccountAsync(string id, CancellationToken cancellationToken = default)
        {
            AccountResultDto readEditableResultDto = new AccountResultDto();
            User user = await _userManager.FindByNameAsync(id);
            return GetProfileDetails(user);
        }

        public async Task<List<UserApiUsageStatisticDto>> GetLastHourUserApiUsageStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await GetUserApiUsageStatisticsAsync(DateTimeOffset.Now.AddHours(-1), cancellationToken);
        }

        private async Task<List<UserApiUsageStatisticDto>> GetUserApiUsageStatisticsAsync(DateTimeOffset fromDate, CancellationToken cancellationToken = default)
        {
            return await _baseContext.UserApiUsageStatistics
                .Where(x => x.RequestTime >= fromDate)
                .GroupBy(x => x.RequestTime.Date)
                .Select(x => new UserApiUsageStatisticDto
                {
                    Date = x.Key,
                    Count = x.Count()
                }) 
                .ToListAsync();
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
