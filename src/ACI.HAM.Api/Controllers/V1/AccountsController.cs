using ACI.HAM.Core.Dtos;
using ACI.HAM.Core.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACI.HAM.Api.Controllers.V1
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AccountsController : ApiControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        [HttpPost]
        [Route("generate-api-key")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<GenerateApiKeyResultDto>> GenerateApiKeyAsync(CancellationToken cancellationToken = default)
        {
            string name = User.Identity.Name;
            GenerateApiKeyResultDto generateApiKeyResultDto = await _accountService.GenerateApiKeyAsync(name, cancellationToken);
            if (!generateApiKeyResultDto.HasErrors)
            {
                return Ok(generateApiKeyResultDto.ApiKey);
            }
            else
            {
                return BadRequest(generateApiKeyResultDto.Errors);
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
            AccountResultDto accountResultDto = await _accountService.GetAccountAsync(name, cancellationToken);
            if (!accountResultDto.HasErrors)
            {
                return Ok(accountResultDto.Account);
            }
            else
            {
                return BadRequest(accountResultDto.Errors);
            }
        }

        [HttpGet]
        [Route("read-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AccountDto>> ReadByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            AccountResultDto accountResultDto = await _accountService.GetAccountAsync(id, cancellationToken);
            if (!accountResultDto.HasErrors)
            {
                return Ok(accountResultDto.Account);
            }
            else
            {
                return BadRequest(accountResultDto.Errors);
            }
        }

        [HttpPut]
        [Route("update-profile-details")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProfileDetailsDto>> UpdateProfileDetailsAsync([FromBody] ProfileDetailsDto profileDetailsDto, CancellationToken cancellationToken = default)
        {
            string name = User.Identity.Name;
            UpdateProfileDetailsResultDto updateProfileDetailsResultDto = await _accountService.UpdateProfileDetailsAsync(name, profileDetailsDto, cancellationToken);
            if (!updateProfileDetailsResultDto.HasErrors)
            {
                return Ok(profileDetailsDto);
            }
            return BadRequest(updateProfileDetailsResultDto.Errors);            
        }
    }
}
