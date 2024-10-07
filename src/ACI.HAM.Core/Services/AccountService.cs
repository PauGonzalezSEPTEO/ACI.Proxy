using System.Threading.Tasks;
using System.Threading;
using ACI.HAM.Core.Dtos;
using ACI.HAM.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using ACI.HAM.Core.Extensions;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ACI.HAM.Core.Data;

namespace ACI.HAM.Core.Services
{
    public interface IAccountService
    {
        Task<GenerateApiKeyResultDto> GenerateApiKeyAsync(string id, CancellationToken cancellationToken = default);

        Task<AccountResultDto> GetAccountAsync(string id, CancellationToken cancellationToken = default);

        Task<DataTablesResult<UserApiKeyDto>> ReadUserApiKeysDataTableAsync(DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default);

        Task<UpdateProfileDetailsResultDto> UpdateProfileDetailsAsync(string id, ProfileDetailsDto profileDetailsDto, CancellationToken cancellationToken = default);
    }

    public class AccountService : IAccountService
    {
        private readonly BaseContext _baseContext;
        private readonly IConfiguration _configuration;
        private readonly IDataProtector _dataProtector;
        private readonly IStringLocalizer<AccountService> _localizer;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public AccountService(UserManager<User> userManager, BaseContext baseContext, IMapper mapper, IStringLocalizer<AccountService> localizer, IDataProtectionProvider dataProtectionProvider, IConfiguration configuration)
        {
            _userManager = userManager;
            _baseContext = baseContext;
            _mapper = mapper;
            _localizer = localizer;
            _dataProtector = dataProtectionProvider.CreateProtector("ApiKey.ApiKeyProtector");
            _configuration = configuration;
        }

        public async Task<GenerateApiKeyResultDto> GenerateApiKeyAsync(string id, CancellationToken cancellationToken = default)
        {
            GenerateApiKeyResultDto generateApiKeyResultDto = new GenerateApiKeyResultDto();
            User user = await _userManager.FindByNameAsync(id);
            if (user == null)
            {
                generateApiKeyResultDto.HasErrors = true;
                generateApiKeyResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = _localizer["User does not exist"]
                    }
                };
            }
            else
            {
                string encryptedEncryptionKey = _configuration["ApiKey:EncryptionKey"];
                string encryptionKey = _dataProtector.Unprotect(encryptedEncryptionKey);
                string apiKey = ApiKeyExtension.GenerateApiKey();
                var (hashedApiKey, salt) = ApiKeyExtension.HashApiKey(apiKey);
                string encryptedApiKey = ApiKeyExtension.EncryptApiKey(apiKey, encryptionKey);
                var userApiKey = new UserApiKey
                {
                    EncryptedApiKey = encryptedApiKey,
                    HashedApiKey = hashedApiKey,
                    Salt = salt,
                    CreatedAt = DateTime.UtcNow,
                    Expiration = DateTime.UtcNow.AddMonths(1),
                    IsActive = true,
                    UserId = user.Id
                };
                var currentApiKeys = await _baseContext.UserApiKeys
                    .Where(x => (x.UserId == user.Id) && x.IsActive)
                    .ToListAsync();
                foreach (var oldKey in currentApiKeys)
                {
                    oldKey.IsActive = false;
                }
                _baseContext.UserApiKeys.Add(userApiKey);
                await _baseContext.SaveChangesAsync();
                generateApiKeyResultDto.ApiKey = apiKey;
            }
            return generateApiKeyResultDto;
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

        public async Task<DataTablesResult<UserApiKeyDto>> ReadUserApiKeysDataTableAsync(DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default)
        {
            IQueryable<UserApiKey> query =
                _baseContext.UserApiKeys
                    .AsQueryable();
            return await query.GetDataTablesResultAsync<UserApiKey, UserApiKeyDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
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
