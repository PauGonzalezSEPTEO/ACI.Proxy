using ACI.Proxy.Api.Controllers;
using ACI.Proxy.Core.Dtos;
using ACI.Proxy.Core.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ACI.Proxy.Api.V1.Controllers
{
    [Route("api/v{version:apiVersion}/roles")]
    [ApiVersion("1.0")]
    [EnableCors("ApiCorsPolicy")]
    public class RolesController : JwtControllerBase
    {
        private readonly IStringLocalizer<UsersController> _messages;
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService, IStringLocalizer<UsersController> messages)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Policy = "CanCreateRole")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RoleEditableDto>> CreateAsync([FromBody] RoleEditableDto roleCreateDto, CancellationToken cancellationToken = default)
        {
            string id = await _roleService.CreateAsync(roleCreateDto, cancellationToken);
            return CreatedAtAction(nameof(ReadEditableByIdAsync), new { id }, roleCreateDto);
        }

        [HttpDelete]
        [Route("delete-by-id/{id}")]
        [Authorize(Policy = "CanDeleteRole")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RoleEditableDto>> DeleteByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            RoleEditableDto roleEditableDto = await _roleService.DeleteByIdAsync(id, cancellationToken);
            return Ok(roleEditableDto);
        }

        [HttpDelete]
        [Route("delete-user-by-id/{id}/{userId}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> DeleteUserByIdAsync(string id, string userId, CancellationToken cancellationToken = default)
        {
            bool result = await _roleService.DeleteUserByIdAsync(id, userId, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("read")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<RoleDto>>> ReadAsync([FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            List<RoleDto> rolesDto = await _roleService.ReadAsync(languageCode, cancellationToken);
            return Ok(rolesDto);
        }

        [HttpPost]
        [Route("read-data-table")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DataTablesResult<RoleDto>>> ReadDataTableAsync([FromBody] DataTablesParameters dataTablesParameters, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            DataTablesResult<RoleDto> rolesDto = await _roleService.ReadDataTableAsync(dataTablesParameters, languageCode, cancellationToken);
            return Ok(rolesDto);
        }

        [HttpPost]
        [Route("read-users-data-table")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DataTablesResult<UserDto>>> ReadUsersDataTableAsync([FromQuery] string id, [FromBody] DataTablesParameters dataTablesParameters, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            DataTablesResult<UserDto> usersDto = await _roleService.ReadUsersDataTableAsync(id, dataTablesParameters, languageCode, cancellationToken);
            return Ok(usersDto);
        }

        [HttpGet]
        [Route("read-editable-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RoleEditableDto>> ReadEditableByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            RoleEditableDto roleEditableDto = await _roleService.ReadEditableByIdAsync(id, cancellationToken);
            return Ok(roleEditableDto);
        }

        [HttpPost]
        [Route("search")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<RoleDto>>> SearchAsync([FromQuery] string search = null, [FromQuery] string ordering = null, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            List<RoleDto> rolesDto = await _roleService.SearchAsync(search, ordering, languageCode, cancellationToken);
            return Ok(rolesDto);
        }

        [HttpPut]
        [Route("update-by-id/{id}")]
        [Authorize(Policy = "CanUpdateRole")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RoleEditableDto>> UpdateByIdAsync(string id, [FromBody] RoleEditableDto roleEditableDto, CancellationToken cancellationToken = default)
        {
            bool result = await _roleService.UpdateByIdAsync(id, roleEditableDto, cancellationToken);
            if (result)
            {
                return Ok(roleEditableDto);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, _messages["Error updating the role type in the database"].Value);
            }
        }
    }
}
