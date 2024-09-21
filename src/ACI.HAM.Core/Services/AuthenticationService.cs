using AutoMapper;
using System.Threading.Tasks;
using System.Threading;
using ACI.HAM.Core.Dtos;
using ACI.HAM.Core.Models;
using Microsoft.AspNetCore.Identity;
using ACI.HAM.Mail.Services;
using ACI.HAM.Mail.Dtos;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using ACI.HAM.Core.Data;
using System.Linq;
using ACI.HAM.Settings;

namespace ACI.HAM.Core.Services
{
    public interface IAuthenticationService
    {
        Task<ChangeEmailResultDto> ChangeEmailAsync(string name, ChangeEmailDto changeEmailDto, CancellationToken cancellationToken);

        Task<ChangePasswordResultDto> ChangePasswordAsync(string name, ChangePasswordDto changePasswordDto, CancellationToken cancellationToken);

        Task<RegistrationResultDto> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken);

        Task<DeactivateResultDto> DeactivateAsync(string name, CancellationToken cancellationToken = default);

        Task<ForgotPasswordResultDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto, CancellationToken cancellationToken);

        Task<LoginResultDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);

        Task<AccountResultDto> ReadEditableByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<AccountResultDto> ReadEditableByNameAsync(string name, CancellationToken cancellationToken = default);

        Task<RefreshResultDto> RefreshTokenAsync(RefreshDto tokenRefreshDto, CancellationToken cancellationToken);

        Task<RegistrationResultDto> RegisterAsync(RegistrationDto registrationDto, CancellationToken cancellationToken);

        Task<ResendVerifyEmailResultDto> ResendVerifyEmailAsync(ResendVerifyEmailDto resendVerifyEmailDto, CancellationToken cancellationToken);

        Task<ResetPasswordResultDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken);

        Task<SetTwoFactorEnabledResultDto> SetTwoFactorEnabledAsync(string name, bool enabled, CancellationToken cancellationToken);

        Task<TwoFactorResultDto> TwoFactorAsync(TwoFactorDto twoFactorDto, CancellationToken cancellationToken);

        Task<VerifyEmailAndSetPasswordResultDto> VerifyEmailAndSetPasswordAsync(VerifyEmailAndSetPasswordDto verifyEmailAndSetPasswordDto, CancellationToken cancellationToken);

        Task<VerifyEmailResultDto> VerifyEmailAsync(VerifyEmailDto verifyEmailDto, CancellationToken cancellationToken);

        Task<VerifyNewEmailResultDto> VerifyNewEmailAsync(VerifyNewEmailDto verifyNewEmailDto, CancellationToken cancellationToken);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly BaseContext _baseContext;
        private readonly IStringLocalizer<AuthenticationService> _localizer;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UISettings _uiSettings;
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;

        public AuthenticationService(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, BaseContext baseContext, IMapper mapper, IMailService mailService, IStringLocalizer<AuthenticationService> localizer, IOptions<UISettings> uiSettings, IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _baseContext = baseContext;
            _mapper = mapper;
            _mailService = mailService;
            _localizer = localizer;
            _uiSettings = uiSettings.Value;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<ChangeEmailResultDto> ChangeEmailAsync(string name, ChangeEmailDto changeEmailDto, CancellationToken cancellationToken)
        {
            ChangeEmailResultDto changeEmailResultDto = new ChangeEmailResultDto();
            User user = await _userManager.FindByEmailAsync(name);
            if (user == null)
            {
                changeEmailResultDto.HasErrors = true;
                changeEmailResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = _localizer["User does not exist"]
                    }
                };
            }
            else
            {
                string changeEmailToken = await _userManager.GenerateChangeEmailTokenAsync(user, changeEmailDto.NewEmail);
                string token = HttpUtility.UrlEncode(changeEmailToken);
                Uri baseUri = new Uri(_uiSettings.BaseUrl);
                Uri changeEmailUrl = new Uri(baseUri, _uiSettings.ChangeEmailRelativeUrl + $"?email={name}&newEmail={changeEmailDto.NewEmail}&token={token}");
                SendChangeEmailMailDto sendChangeEmailMailDto = new SendChangeEmailMailDto()
                {
                    Subject = _localizer["Change email"],
                    To = changeEmailDto.NewEmail,
                    Url = changeEmailUrl.AbsoluteUri
                };
                await (_mailService.SendChangeEmailMailAsync(sendChangeEmailMailDto, cancellationToken));
            }
            return changeEmailResultDto;
        }

        public async Task<ChangePasswordResultDto> ChangePasswordAsync(string name, ChangePasswordDto changePasswordDto, CancellationToken cancellationToken)
        {
            ChangePasswordResultDto changePasswordResultDto = new ChangePasswordResultDto();
            User user = await _userManager.FindByEmailAsync(name);
            if (user == null)
            {
                changePasswordResultDto.HasErrors = true;
                changePasswordResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = _localizer["User does not exist"]
                    }
                };
            }
            else
            {
                IdentityResult identityResult = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                if (!identityResult.Succeeded)
                {
                    changePasswordResultDto.HasErrors = true;
                    changePasswordResultDto.Errors = identityResult.Errors;
                }
            }
            return changePasswordResultDto;
        }

        public async Task<RegistrationResultDto> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default)
        {
            RegistrationResultDto registrationResultDto = new RegistrationResultDto();
            User user = _mapper.Map<User>(createUserDto);
            user.CreateDate = DateTimeOffset.UtcNow;
            IdentityResult identityResult = await _userManager.CreateAsync(user);
            if (!identityResult.Succeeded)
            {
                registrationResultDto.HasErrors = true;
                registrationResultDto.Errors = identityResult.Errors;
                return registrationResultDto;
            }
            else
            {
                registrationResultDto.Id = user.Id;
                registrationResultDto.User = new AccountDto()
                {
                    Firstname = user.Firstname,
                    Lastname = user.Lastname
                };
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string tokenEmailConfirmation = HttpUtility.UrlEncode(emailConfirmationToken);
                string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                string tokenPasswordReset = HttpUtility.UrlEncode(passwordResetToken);
                Uri baseUri = new Uri(_uiSettings.BaseUrl);
                Uri confirmEmailUrl = new Uri(baseUri, _uiSettings.ConfirmEmailAndSetPasswordRelativeUrl + $"?email={user.Email}&email-token={tokenEmailConfirmation}&password-token={tokenPasswordReset}");
                SendPasswordResetMailDto sendPasswordResetMailDto = new SendPasswordResetMailDto()
                {
                    Subject = _localizer["Confirm email and set password"],
                    To = user.Email,
                    Url = confirmEmailUrl.AbsoluteUri
                };
                _ = Task.Run(() => _mailService.SendPasswordResetMailAsync(sendPasswordResetMailDto, cancellationToken));
                return registrationResultDto;
            }
        }

        public async Task<DeactivateResultDto> DeactivateAsync(string name, CancellationToken cancellationToken)
        {
            DeactivateResultDto deactivateResultDto = new DeactivateResultDto();
            User user = await _userManager.FindByEmailAsync(name);
            if (user == null)
            {
                deactivateResultDto.HasErrors = true;
                deactivateResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = _localizer["User does not exist"]
                    }
                };
            }
            else
            {
                IdentityResult identityResult = await _userManager.DeleteAsync(user);
                if (!identityResult.Succeeded)
                {
                    deactivateResultDto.Errors = identityResult.Errors;
                }
            }
            return deactivateResultDto;
        }

        public async Task<ForgotPasswordResultDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto, CancellationToken cancellationToken)
        {
            ForgotPasswordResultDto forgotPasswordResultDto = new ForgotPasswordResultDto();
            User user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                forgotPasswordResultDto.HasErrors = true;
                forgotPasswordResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = _localizer["User does not exist"]
                    }
                };
            }
            else
            {
                string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                string token = HttpUtility.UrlEncode(passwordResetToken);
                Uri baseUri = new Uri(_uiSettings.BaseUrl);
                Uri resetPasswordUrl = new Uri(baseUri, _uiSettings.ResetPasswordRelativeUrl + $"?email={user.Email}&token={token}");
                SendPasswordResetMailDto sendPasswordResetMailDto = new SendPasswordResetMailDto()
                {
                    Subject = _localizer["Reset password"],
                    To = forgotPasswordDto.Email,
                    Url = resetPasswordUrl.AbsoluteUri
                };
                await (_mailService.SendPasswordResetMailAsync(sendPasswordResetMailDto, cancellationToken));
            }
            return forgotPasswordResultDto;
        }

        private async Task<string> GenerateAccessTokenAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            byte[] securityKey = Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey);
            SymmetricSecurityKey secret = new SymmetricSecurityKey(securityKey);
            SigningCredentials signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.ValidIssuer,
                audience: _jwtSettings.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.TokenValidityInMinutes),
                signingCredentials: signingCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private DateTime GetRefreshTokenExpiryTime()
        {
            return DateTime.Now.AddDays(_jwtSettings.RefreshTokenValidityInDays);
        }

        public async Task<LoginResultDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            LoginResultDto loginResultDto = new LoginResultDto
            {
                Email = loginDto.Email
            };
            User user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                loginResultDto.IsUnauthorized = true;
            }
            else
            {
                SignInResult signInResult = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, true);
                if (signInResult.IsLockedOut)
                {
                    loginResultDto.IsLockedOut = true;
                    string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    string token = HttpUtility.UrlEncode(passwordResetToken);
                    Uri baseUri = new Uri(_uiSettings.BaseUrl);
                    Uri resetPasswordUrl = new Uri(baseUri, _uiSettings.ResetPasswordRelativeUrl + $"?email={user.Email}&token={token}");
                    SendLockoutMailDto sendLockoutMailDto = new SendLockoutMailDto()
                    {
                        Subject = _localizer["Locked out account information"],
                        To = loginDto.Email,
                        Url = resetPasswordUrl.AbsoluteUri
                    };
                    await (_mailService.SendLockoutMailAsync(sendLockoutMailDto, cancellationToken));

                }
                else if (signInResult.IsNotAllowed)
                {
                    loginResultDto.EmailNotConfirmed = true;
                }
                else if (signInResult.RequiresTwoFactor)
                {
                    IList<string> providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
                    const string EMAIL_TWO_FACTOR_PROVIDER = "Email";
                    if (!providers.Contains(EMAIL_TWO_FACTOR_PROVIDER))
                    {
                        loginResultDto.IsInvalidTwoFactorVerificationProvider = true;
                    }
                    else
                    {
                        loginResultDto.TwoFactorProvider = EMAIL_TWO_FACTOR_PROVIDER;
                        string twoFactorToken = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                        SendTwoFactorMailDto sendTwoFactorMailDto = new SendTwoFactorMailDto()
                        {
                            Subject = _localizer["Authentication token"],
                            To = loginDto.Email,
                            Token = twoFactorToken
                        };
                        await (_mailService.SendTwoFactorMailAsync(sendTwoFactorMailDto, cancellationToken));
                        loginResultDto.IsTwoFactorVerificationRequired = true;
                    }
                }
                else if (signInResult.Succeeded)
                {
                    loginResultDto.AccessToken = await GenerateAccessTokenAsync(user);
                    loginResultDto.RefreshToken = GenerateRefreshToken();
                    loginResultDto.RefreshTokenExpiryTime = GetRefreshTokenExpiryTime();
                    user.RefreshToken = loginResultDto.RefreshToken;
                    user.RefreshTokenExpiryTime = loginResultDto.RefreshTokenExpiryTime;
                    user.LastLoginDate = DateTime.UtcNow;
                    await _baseContext.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    loginResultDto.AccessFailedCount = user.AccessFailedCount;
                }
            }
            return loginResultDto;
        }

        public async Task<AccountResultDto> ReadEditableByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            AccountResultDto readEditableResultDto = new AccountResultDto();
            User user = await _userManager.FindByIdAsync(id);
            return ReadEditable(user);
        }

        public async Task<AccountResultDto> ReadEditableByNameAsync(string id, CancellationToken cancellationToken = default)
        {
            AccountResultDto readEditableResultDto = new AccountResultDto();
            User user = await _userManager.FindByNameAsync(id);
            return ReadEditable(user);
        }

        private AccountResultDto ReadEditable(User user, CancellationToken cancellationToken = default)
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
                readEditableResultDto.Account = new AccountDto()
                {
                    AllowCommercialMail = user.AllowCommercialMail,
                    Avatar = user.Avatar,
                    CommunicationByMail = user.CommunicationByMail,
                    CommunicationByPhone = user.CommunicationByPhone,
                    CompanyName = user.CompanyName,
                    CountryAlpha2Code = user.CountryAlpha2Code,
                    CurrencyCode = user.CurrencyCode,
                    Firstname = user.Firstname,
                    LanguageAlpha2Code = user.LanguageAlpha2Code,
                    Lastname = user.Lastname
                };
            }
            return readEditableResultDto;
        }

        public async Task<RefreshResultDto> RefreshTokenAsync(RefreshDto tokenRefreshDto, CancellationToken cancellationToken)
        {
            RefreshResultDto refreshResultDto = new RefreshResultDto();
            byte[] securityKey = Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey);
            SymmetricSecurityKey secret = new SymmetricSecurityKey(securityKey);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = _jwtSettings.ValidIssuer,
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = secret,
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationResult tokenValidationResult = await tokenHandler.ValidateTokenAsync(tokenRefreshDto.AccessToken, tokenValidationParameters);
            var jwtSecurityToken = tokenValidationResult.SecurityToken as JwtSecurityToken;
            if ((jwtSecurityToken == null) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                refreshResultDto.IsInvalidToken = true;
            }
            string email = tokenValidationResult.ClaimsIdentity.Name;
            User user = await _userManager.FindByEmailAsync(email);
            if ((user == null) || (user.RefreshToken != tokenRefreshDto.RefreshToken) || (user.RefreshTokenExpiryTime <= DateTime.Now))
            {
                refreshResultDto.IsInvalidToken = true;
            }
            else
            {
                refreshResultDto.AccessToken = await GenerateAccessTokenAsync(user);
                refreshResultDto.RefreshToken = GenerateRefreshToken();
                refreshResultDto.RefreshTokenExpiryTime = GetRefreshTokenExpiryTime();
                user.RefreshToken = refreshResultDto.RefreshToken;
                user.RefreshTokenExpiryTime = refreshResultDto.RefreshTokenExpiryTime;
                await _baseContext.SaveChangesAsync(cancellationToken);
            }
            return refreshResultDto;
        }

        public async Task<RegistrationResultDto> RegisterAsync(RegistrationDto registrationDto, CancellationToken cancellationToken = default)
        {
            RegistrationResultDto registrationResultDto = new RegistrationResultDto();
            User user = _mapper.Map<User>(registrationDto);
            user.CreateDate = DateTimeOffset.UtcNow;
            IdentityResult identityResult = await _userManager.CreateAsync(user, registrationDto.Password);
            if (!identityResult.Succeeded)
            {
                registrationResultDto.HasErrors = true;
                registrationResultDto.Errors = identityResult.Errors;
            }
            else
            {
                registrationResultDto.Id = user.Id;
                registrationResultDto.User = new AccountDto()
                {
                    Firstname = user.Firstname,
                    Lastname = user.Lastname
                };
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string token = HttpUtility.UrlEncode(emailConfirmationToken);
                Uri baseUri = new Uri(_uiSettings.BaseUrl);
                Uri confirmEmailUrl = new Uri(baseUri, _uiSettings.ConfirmEmailRelativeUrl + $"?email={user.Email}&token={token}");
                SendVerifyEmailMailDto sendPasswordResetMailDto = new SendVerifyEmailMailDto()
                {
                    Subject = _localizer["Confirm email"],
                    To = user.Email,
                    Url = confirmEmailUrl.AbsoluteUri
                };
                await (_mailService.SendVerifyEmailMailAsync(sendPasswordResetMailDto, cancellationToken));
            }
            return registrationResultDto;
        }

        public async Task<ResendVerifyEmailResultDto> ResendVerifyEmailAsync(ResendVerifyEmailDto resendVerifyEmailDto, CancellationToken cancellationToken)
        {
            ResendVerifyEmailResultDto resendVerifyEmailResultDto = new ResendVerifyEmailResultDto();
            User user = await _userManager.FindByEmailAsync(resendVerifyEmailDto.Email);
            if (user == null)
            {
                resendVerifyEmailResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = string.Format(_localizer["Email does not exist"], resendVerifyEmailDto.Email)
                    }
                };
            }
            else
            {
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string token = HttpUtility.UrlEncode(emailConfirmationToken);
                Uri baseUri = new Uri(_uiSettings.BaseUrl);
                Uri confirmEmailUrl = new Uri(baseUri, _uiSettings.ConfirmEmailRelativeUrl + $"?email={user.Email}&token={token}");
                SendVerifyEmailMailDto sendPasswordResetMailDto = new SendVerifyEmailMailDto()
                {
                    Subject = _localizer["Confirm email"],
                    To = user.Email,
                    Url = confirmEmailUrl.AbsoluteUri
                };
                _ = Task.Run(() => _mailService.SendVerifyEmailMailAsync(sendPasswordResetMailDto, cancellationToken));
                return resendVerifyEmailResultDto;
            }
            return resendVerifyEmailResultDto;
        }

        public async Task<ResetPasswordResultDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken)
        {
            ResetPasswordResultDto resetPasswordResultDto = new ResetPasswordResultDto();
            User user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                resetPasswordResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = string.Format(_localizer["Email does not exist"], resetPasswordDto.Email)
                    }
                };
            }
            else
            {
                IdentityResult identityResult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
                if (!identityResult.Succeeded)
                {
                    resetPasswordResultDto.Errors = identityResult.Errors;
                }
                else
                {
                    SendPasswordChangeMailDto sendPasswordChangeMailDto = new SendPasswordChangeMailDto()
                    {
                        Subject = _localizer["Change password"],
                        To = resetPasswordDto.Email
                    };
                    _ = _mailService.SendPasswordChangeMailAsync(sendPasswordChangeMailDto, cancellationToken);
                    resetPasswordResultDto.IsSucceded = true;
                }
            }
            return resetPasswordResultDto;
        }

        public async Task<SetTwoFactorEnabledResultDto> SetTwoFactorEnabledAsync(string name, bool enabled, CancellationToken cancellationToken)
        {
            SetTwoFactorEnabledResultDto twoFactorEnabledResultDto = new SetTwoFactorEnabledResultDto();
            User user = await _userManager.FindByEmailAsync(name);
            if (user == null)
            {
                twoFactorEnabledResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = _localizer["User does not exist"]
                    }
                };
            }
            else
            {
                IdentityResult identityResult = await _userManager.SetTwoFactorEnabledAsync(user, enabled);
                if (!identityResult.Succeeded)
                {
                    twoFactorEnabledResultDto.Errors = identityResult.Errors;
                }
                else
                {
                    twoFactorEnabledResultDto.IsSucceded = true;
                }
            }
            return twoFactorEnabledResultDto;
        }

        public async Task<TwoFactorResultDto> TwoFactorAsync(TwoFactorDto twoFactorDto, CancellationToken cancellationToken)
        {
            TwoFactorResultDto twoFactorResultDto = new TwoFactorResultDto();
            User user = await _userManager.FindByEmailAsync(twoFactorDto.Email);
            if (user == null)
            {
                twoFactorResultDto.HasErrors = true;
                twoFactorResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = string.Format(_localizer["Email does not exist"], twoFactorDto.Email)
                    }
                };
            }
            else
            {
                if (!await _userManager.VerifyTwoFactorTokenAsync(user, twoFactorDto.Provider, twoFactorDto.Token))
                {
                    twoFactorResultDto.IsInvalidToken = true;
                }
                else
                {
                    twoFactorResultDto.AccessToken = await GenerateAccessTokenAsync(user);
                    twoFactorResultDto.RefreshToken = GenerateRefreshToken();
                    twoFactorResultDto.RefreshTokenExpiryTime = GetRefreshTokenExpiryTime();
                    user.RefreshToken = twoFactorResultDto.RefreshToken;
                    user.RefreshTokenExpiryTime = twoFactorResultDto.RefreshTokenExpiryTime;
                    await _baseContext.SaveChangesAsync(cancellationToken);
                }
            }
            return twoFactorResultDto;
        }

        public async Task<VerifyEmailAndSetPasswordResultDto> VerifyEmailAndSetPasswordAsync(VerifyEmailAndSetPasswordDto verifyEmailAndSetPasswordDto, CancellationToken cancellationToken)
        {
            VerifyEmailAndSetPasswordResultDto verifyEmailAndSetPasswordResultDto = new VerifyEmailAndSetPasswordResultDto();
            User user = await _userManager.FindByEmailAsync(verifyEmailAndSetPasswordDto.Email);
            if (user == null)
            {
                verifyEmailAndSetPasswordResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = string.Format(_localizer["Email does not exist"], verifyEmailAndSetPasswordDto.Email)
                    }
                };
            }
            else
            {
                bool error = false;
                IdentityResult identityResult = await _userManager.ConfirmEmailAsync(user, verifyEmailAndSetPasswordDto.EmailToken);
                if (!identityResult.Succeeded)
                {
                    verifyEmailAndSetPasswordResultDto.Errors = identityResult.Errors;
                    error = true;
                }
                identityResult = await _userManager.ResetPasswordAsync(user, verifyEmailAndSetPasswordDto.PasswordToken, verifyEmailAndSetPasswordDto.Password);
                if (!identityResult.Succeeded)
                {
                    if (verifyEmailAndSetPasswordResultDto.Errors == null)
                    {
                        verifyEmailAndSetPasswordResultDto.Errors = new List<IdentityError>();
                    }
                    verifyEmailAndSetPasswordResultDto.Errors = verifyEmailAndSetPasswordResultDto.Errors.Concat(identityResult.Errors);
                    error = true;
                }
                if (!error)
                {
                    verifyEmailAndSetPasswordResultDto.IsSucceded = true;
                }
            }
            return verifyEmailAndSetPasswordResultDto;
        }

        public async Task<VerifyEmailResultDto> VerifyEmailAsync(VerifyEmailDto verifyEmailDto, CancellationToken cancellationToken)
        {
            VerifyEmailResultDto verifyEmailResultDto = new VerifyEmailResultDto();
            User user = await _userManager.FindByEmailAsync(verifyEmailDto.Email);
            if (user == null)
            {
                verifyEmailResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = string.Format(_localizer["Email does not exist"], verifyEmailDto.Email)
                    }
                };
            }
            else
            {
                IdentityResult identityResult = await _userManager.ConfirmEmailAsync(user, verifyEmailDto.Token);
                if (!identityResult.Succeeded)
                {
                    verifyEmailResultDto.Errors = identityResult.Errors;
                }
                else
                {
                    verifyEmailResultDto.IsSucceded = true;
                }
            }
            return verifyEmailResultDto;
        }

        public async Task<VerifyNewEmailResultDto> VerifyNewEmailAsync(VerifyNewEmailDto verifyNewEmailDto, CancellationToken cancellationToken)
        {
            VerifyNewEmailResultDto verifyNewEmailResultDto = new VerifyNewEmailResultDto();
            User user = await _userManager.FindByEmailAsync(verifyNewEmailDto.Email);
            if (user == null)
            {
                verifyNewEmailResultDto.Errors = new List<IdentityError>()
                {
                    new IdentityError()
                    {
                        Description = string.Format(_localizer["Email does not exist"], verifyNewEmailDto.Email)
                    }
                };
            }
            else
            {
                IdentityResult identityResult = await _userManager.ChangeEmailAsync(user, verifyNewEmailDto.NewEmail, verifyNewEmailDto.Token);
                if (!identityResult.Succeeded)
                {
                    verifyNewEmailResultDto.Errors = identityResult.Errors;
                }
                else
                {
                    user.Email = verifyNewEmailDto.NewEmail;
                    user.UserName = verifyNewEmailDto.NewEmail;
                    identityResult = await _userManager.UpdateAsync(user);
                    if (!identityResult.Succeeded)
                    {
                        verifyNewEmailResultDto.Errors = identityResult.Errors;
                    }
                    else
                    {
                        verifyNewEmailResultDto.IsSucceded = true;
                    }
                }
            }
            return verifyNewEmailResultDto;
        }
    }
}
