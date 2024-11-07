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
    [Route("api/v{version:apiVersion}/projects")]
    [ApiVersion("1.0")]
    [EnableCors("ApiCorsPolicy")]
    public class ProjectsController : JwtControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IStringLocalizer<UsersController> _messages;

        public ProjectsController(IProjectService projectService, IStringLocalizer<UsersController> messages)
        {
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Policy = "CanCreateProject")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProjectEditableDto>> CreateAsync([FromBody] ProjectEditableDto projectCreateDto, CancellationToken cancellationToken = default)
        {
            int id = await _projectService.CreateAsync(projectCreateDto, cancellationToken);
            return CreatedAtAction(nameof(ReadEditableByIdAsync), new { id }, projectCreateDto);
        }

        [HttpDelete]
        [Route("delete-by-id/{id}")]
        [Authorize(Policy = "CanDeleteProject")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProjectEditableDto>> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            ProjectEditableDto projectEditableDto = await _projectService.DeleteByIdAsync(id, cancellationToken);
            return Ok(projectEditableDto);
        }

        [HttpPost]
        [Route("read-by-company-ids")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ProjectDto>>> ReadByCompanyIdsAsync([FromBody] int[] companyIds, CancellationToken cancellationToken = default)
        {
            List<ProjectDto> projectsDto = await _projectService.ReadByCompanyIdsAsync(companyIds, cancellationToken);
            return Ok(projectsDto);
        }

        [HttpPost]
        [Route("read-data-table")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DataTablesResult<ProjectDto>>> ReadDataTableAsync([FromBody] DataTablesParameters dataTablesParameters, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            DataTablesResult<ProjectDto> projectsDto = await _projectService.ReadDataTableAsync(dataTablesParameters, languageCode, cancellationToken);
            return Ok(projectsDto);
        }

        [HttpGet]
        [Route("read-editable-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProjectEditableDto>> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            ProjectEditableDto projectEditableDto = await _projectService.ReadEditableByIdAsync(id, cancellationToken);
            return Ok(projectEditableDto);
        }

        [HttpPost]
        [Route("search")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ProjectDto>>> SearchAsync([FromQuery] string search = null, [FromQuery] string ordering = null, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            List<ProjectDto> projectsDto = await _projectService.SearchAsync(search, ordering, languageCode, cancellationToken);
            return Ok(projectsDto);
        }

        [HttpPut]
        [Route("update-by-id/{id}")]
        [Authorize(Policy = "CanUpdateProject")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProjectEditableDto>> UpdateByIdAsync(int id, [FromBody] ProjectEditableDto projectEditableDto, CancellationToken cancellationToken = default)
        {
            bool result = await _projectService.UpdateByIdAsync(id, projectEditableDto, cancellationToken);
            if (result)
            {
                return Ok(projectEditableDto);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, _messages["Error updating the project in the database"].Value);
            }
        }
    }
}
