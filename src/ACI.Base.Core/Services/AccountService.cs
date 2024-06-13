using System.Threading.Tasks;
using System.Threading;
using ACI.Base.Core.Dtos;
using ACI.Base.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using AutoMapper;

namespace ACI.Base.Core.Services
{
    public interface IAccountService
    {
        Task<AccountResultDto> GetAccountAsync(string id, CancellationToken cancellationToken = default);

        Task<UpdateProfileDetailsResultDto> UpdateProfileDetailsAsync(string id, ProfileDetailsDto profileDetailsDto, CancellationToken cancellationToken = default);
    }

    public class AccountService : IAccountService
    {
        private readonly IStringLocalizer<AccountService> _localizer;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public AccountService(UserManager<User> userManager, IMapper mapper, IStringLocalizer<AccountService> localizer)
        {
            _userManager = userManager;
            _mapper = mapper;
            _localizer = localizer;
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
