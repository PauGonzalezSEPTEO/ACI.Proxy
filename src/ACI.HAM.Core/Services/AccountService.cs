using System.Threading.Tasks;
using System.Threading;
using ACI.HAM.Core.Dtos;
using ACI.HAM.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using ACI.HAM.Core.Extensions;
using Microsoft.Extensions.Configuration;

namespace ACI.HAM.Core.Services
{
    public interface IAccountService
    {
        Task<string> GenerateApiKeyAsync(string id);

        Task<AccountResultDto> GetAccountAsync(string id, CancellationToken cancellationToken = default);

        Task<UpdateProfileDetailsResultDto> UpdateProfileDetailsAsync(string id, ProfileDetailsDto profileDetailsDto, CancellationToken cancellationToken = default);
    }

    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly IDataProtector _dataProtector;
        private readonly IStringLocalizer<AccountService> _localizer;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public AccountService(UserManager<User> userManager, IMapper mapper, IStringLocalizer<AccountService> localizer, IDataProtectionProvider dataProtectionProvider, IConfiguration configuration)
        {
            _userManager = userManager;
            _mapper = mapper;
            _localizer = localizer;
            _dataProtector = dataProtectionProvider.CreateProtector("AppSettings.ApiKeyProtector");
            _configuration = configuration;
        }

        public async Task<string> GenerateApiKeyAsync(string id)
        {
            string encryptedApiKey = _configuration["AppSettings:EncryptedApiKey"];
            string decryptedApiKey = _dataProtector.Unprotect(encryptedApiKey);
            string apiKey = ApiKeyExtension.GenerateApiKey();
            return apiKey;
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
