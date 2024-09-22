using ACI.HAM.Api.Controllers;
using ACI.HAM.Api.Controllers.V1;
using ACI.HAM.Core.Dtos;
using ACI.HAM.Core.Localization;
using ACI.HAM.Core.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ACI.HAM.Api.V1.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v{version:apiVersion}/users")]
    [ApiVersion("1.0")]
    [EnableCors("ApiCorsPolicy")]
    public class UsersController : ApiControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IStringLocalizer<UsersController> _messages;
        private readonly IUserService _userService;

        public UsersController(IAuthenticationService authenticationService, IUserService userService, IStringLocalizer<UsersController> messages)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        [HttpPost]
        [Authorize(Policy = "CanCreateUser")]
        [Route("create-user")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountDto>> CreateUserAsync([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken = default)
        {
            RegistrationResultDto registrationResultDto = await _authenticationService.CreateUserAsync(createUserDto, cancellationToken);
            if (!registrationResultDto.HasErrors)
            {
                return CreatedAtAction(nameof(AccountsController.ReadByIdAsync), new { Controller = "accounts", id = registrationResultDto.Id, Version = "1" }, registrationResultDto.User);

            }
            return BadRequest(registrationResultDto.Errors);
        }

        [HttpDelete]
        [Route("delete-by-id/{id}")]
        [Authorize(Policy = "CanDeleteUser")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserEditableDto>> DeleteByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            UserEditableDto userEditableDto = await _userService.DeleteByIdAsync(id, cancellationToken);
            return Ok(userEditableDto);
        }

        [HttpPost]
        [Route("read")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<UserDto>>> ReadAsync([FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            List<UserDto> usersDto = await _userService.ReadAsync(languageCode, cancellationToken);
            return Ok(usersDto);
        }

        [HttpPost]
        [Route("read-data-table")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DataTablesResult<UserDto>>> ReadDataTableAsync([FromBody] DataTablesParameters dataTablesParameters, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            DataTablesResult<UserDto> usersDto = await _userService.ReadDataTableAsync(dataTablesParameters, null, languageCode, cancellationToken);
            return Ok(usersDto);
        }

        [HttpGet]
        [Route("read-editable-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserEditableDto>> ReadEditableByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            UserEditableDto userEditableDto = await _userService.ReadEditableByIdAsync(id, cancellationToken);
            return Ok(userEditableDto);
        }

        [HttpPost]
        [Route("search")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<UserDto>>> SearchAsync([FromQuery] string search = null, [FromQuery] string ordering = null, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            List<UserDto> usersDto = await _userService.SearchAsync(search, ordering, languageCode, cancellationToken);
            return Ok(usersDto);
        }

        [HttpPut]
        [Route("update-by-id/{id}")]
        [Authorize(Policy = "CanUpdateUser")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserEditableDto>> UpdateByIdAsync(string id, [FromBody] UserEditableDto userEditableDto, CancellationToken cancellationToken = default)
        {
            bool result = await _userService.UpdateByIdAsync(id, userEditableDto, cancellationToken);
            if (result)
            {
                return Ok(userEditableDto);
            }
            else
            {                
                return StatusCode(StatusCodes.Status500InternalServerError, _messages["Error updating the user in the database"].Value);

            }            
        }
    }
}
