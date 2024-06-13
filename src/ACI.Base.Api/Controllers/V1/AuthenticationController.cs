using ACI.Base.Core.Dtos;
using ACI.Base.Core.Localization;
using ACI.Base.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ACI.Base.Api.Controllers.V1
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [EnableCors("ApiCorsPolicy")]
    public class AuthenticationController : ApiControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IStringLocalizer<Messages> _messages;

        public AuthenticationController(IAuthenticationService authenticationService, IStringLocalizer<Messages> messages)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        [HttpPost]
        [Route("change-email")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> ChangeEmailAsync([FromBody] ChangeEmailDto changeEmailDto, CancellationToken cancellationToken = default)
        {
            string name = User.Identity.Name;
            ChangeEmailResultDto changeEmailResultDto = await _authenticationService.ChangeEmailAsync(name, changeEmailDto, cancellationToken);
            if (!changeEmailResultDto.HasErrors)
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(changeEmailResultDto.Errors);
            }
        }

        [HttpPost]
        [Route("change-password")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> ChangePasswordAsync([FromBody] ChangePasswordDto changePasswordDto, CancellationToken cancellationToken = default)
        {
            string name = User.Identity.Name;
            ChangePasswordResultDto changePasswordResultDto = await _authenticationService.ChangePasswordAsync(name, changePasswordDto, cancellationToken);
            if (!changePasswordResultDto.HasErrors)
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(changePasswordResultDto.Errors);
            }
        }

        [HttpDelete]
        [Route("deactivate")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AccountDto>> DeactivateAsync(CancellationToken cancellationToken = default)
        {
            string name = User.Identity.Name;
            DeactivateResultDto deactivateResultDto = await _authenticationService.DeactivateAsync(name, cancellationToken);
            if (!deactivateResultDto.HasErrors)
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(deactivateResultDto.Errors);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("forgot-password")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> ForgotPasswordAsync([FromBody] ForgotPasswordDto forgotPasswordDto, CancellationToken cancellationToken = default)
        {
            ForgotPasswordResultDto forgotPasswordResultDto = await _authenticationService.ForgotPasswordAsync(forgotPasswordDto, cancellationToken);
            if (!forgotPasswordResultDto.HasErrors)
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(forgotPasswordResultDto.Errors);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResultDto>> LoginAsync([FromBody] LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            LoginResultDto loginResultDto = await _authenticationService.LoginAsync(loginDto, cancellationToken);
            if (loginResultDto.IsUnauthorized)
            {
                return Unauthorized(_messages["Invalid email or password"].Value);
            }
            else if (loginResultDto.EmailNotConfirmed)
            {
                return Unauthorized(_messages["Email not confirmed, please check your email and confirm your address"].Value);
            }
            else if (loginResultDto.IsInvalidTwoFactorVerificationProvider)
            {
                return Unauthorized(_messages["Invalid two factor verification provider"].Value);
            }
            else
            {
                return Ok(loginResultDto);
            }
        }

        [HttpGet]
        [Route("me")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AccountDto>> MeAsync(CancellationToken cancellationToken = default)
        {
            string name = User.Identity.Name;
            AccountResultDto readEditableResultDto = await _authenticationService.ReadEditableByNameAsync(name, cancellationToken);
            if (!readEditableResultDto.HasErrors)
            {
                return Ok(readEditableResultDto.Account);
            }
            else
            {
                return BadRequest(readEditableResultDto.Errors);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("refresh-token")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RefreshResultDto>> RefreshTokenAsync([FromBody] RefreshDto tokenRefreshDto, CancellationToken cancellationToken)
        {
            RefreshResultDto refreshResultDto = await _authenticationService.RefreshTokenAsync(tokenRefreshDto, cancellationToken);
            if (!refreshResultDto.IsInvalidToken)
            {
                return Ok(refreshResultDto);
            }
            return Unauthorized(_messages["Invalid token"].Value);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountDto>> RegisterAsync([FromBody] RegistrationDto userRegistrationDto, CancellationToken cancellationToken = default)
        {
            RegistrationResultDto registrationResultDto = await _authenticationService.RegisterAsync(userRegistrationDto, cancellationToken);
            if (!registrationResultDto.HasErrors)
            {
                return CreatedAtAction(nameof(AccountsController.ReadByIdAsync), new { Controller = "accounts", id = registrationResultDto.Id, Version = "1" }, registrationResultDto.User);
            }
            return BadRequest(registrationResultDto.Errors);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("resend-verify-email")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> ResendVerifyEmailAsync([FromBody] ResendVerifyEmailDto resendVerifyEmailDto, CancellationToken cancellationToken = default)
        {
            ResendVerifyEmailResultDto resendVerifyEmailResultDto = await _authenticationService.ResendVerifyEmailAsync(resendVerifyEmailDto, cancellationToken);
            if (!resendVerifyEmailResultDto.HasErrors)
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(resendVerifyEmailResultDto.Errors);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("reset-password")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> ResetPasswordAsync([FromBody] ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken = default)
        {
            ResetPasswordResultDto resetPasswordResultDto = await _authenticationService.ResetPasswordAsync(resetPasswordDto, cancellationToken);
            if (resetPasswordResultDto.IsSucceded)
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(resetPasswordResultDto.Errors);
            }
        }

        [HttpPost]
        [Route("set-two-factor-enabled")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> SetTwoFactorEnabledAsync([FromBody] SetTwoFactorEnabledDto setTwoFactorEnabledDto, CancellationToken cancellationToken = default)
        {
            string name = User.Identity.Name;
            SetTwoFactorEnabledResultDto setTwoFactorEnabledResultDto = await _authenticationService.SetTwoFactorEnabledAsync(name, setTwoFactorEnabledDto.Enabled, cancellationToken);
            if (setTwoFactorEnabledResultDto.IsSucceded)
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(setTwoFactorEnabledResultDto.Errors);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("two-factor")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TwoFactorResultDto>> TwoFactorAsync([FromBody] TwoFactorDto twoFactorDto, CancellationToken cancellationToken = default)
        {
            TwoFactorResultDto twoFactorResultDto = await _authenticationService.TwoFactorAsync(twoFactorDto, cancellationToken);
            if (!twoFactorResultDto.HasErrors)
            {
                if (!twoFactorResultDto.IsInvalidToken)
                {
                    return Ok(twoFactorResultDto);
                }
                return Unauthorized(_messages["Invalid token"].Value);
            }
            else
            {
                return BadRequest(twoFactorResultDto.Errors);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("verify-email")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> VerifyEmailAsync([FromBody] VerifyEmailDto verifyEmaildDto, CancellationToken cancellationToken = default)
        {
            VerifyEmailResultDto verifyEmailResultDto = await _authenticationService.VerifyEmailAsync(verifyEmaildDto, cancellationToken);
            if (verifyEmailResultDto.IsSucceded)
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(verifyEmailResultDto.Errors);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("verify-email-and-set-password")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> VerifyEmailAndSetPasswordAsync([FromBody] VerifyEmailAndSetPasswordDto verifyEmailAndSetPasswordDto, CancellationToken cancellationToken = default)
        {
            VerifyEmailAndSetPasswordResultDto verifyEmailAndSetPasswordResultDto = await _authenticationService.VerifyEmailAndSetPasswordAsync(verifyEmailAndSetPasswordDto, cancellationToken);
            if (verifyEmailAndSetPasswordResultDto.IsSucceded)
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(verifyEmailAndSetPasswordResultDto.Errors);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("verify-new-email")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> VerifyNewEmailAsync([FromBody] VerifyNewEmailDto verifyNewEmaildDto, CancellationToken cancellationToken = default)
        {
            VerifyNewEmailResultDto verifyNewEmailResultDto = await _authenticationService.VerifyNewEmailAsync(verifyNewEmaildDto, cancellationToken);
            if (verifyNewEmailResultDto.IsSucceded)
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(verifyNewEmailResultDto.Errors);
            }
        }
    }
}
