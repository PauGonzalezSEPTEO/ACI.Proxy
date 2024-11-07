using ACI.Proxy.Api.Controllers;
using ACI.Proxy.Core.Dtos;
using ACI.Proxy.Core.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ACI.Proxy.Api.V1.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BuildingsController : JwtControllerBase
    {
        private readonly IBuildingService _buildingService;
        private readonly IStringLocalizer<UsersController> _messages;

        public BuildingsController(IBuildingService buildingService, IStringLocalizer<UsersController> messages)
        {
            _buildingService = buildingService ?? throw new ArgumentNullException(nameof(buildingService));
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        [HttpPost]
        [Route("create")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BuildingEditableDto>> CreateAsync([FromBody] BuildingEditableDto buildingEditableDto, CancellationToken cancellationToken = default)
        {
            int id = await _buildingService.CreateAsync(buildingEditableDto, cancellationToken);
            return CreatedAtAction(nameof(ReadEditableByIdAsync), new { id }, buildingEditableDto);
        }

        [HttpDelete]
        [Route("delete-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BuildingEditableDto>> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            BuildingEditableDto buildingEditableDto = await _buildingService.DeleteByIdAsync(id, cancellationToken);
            return Ok(buildingEditableDto);
        }

        [HttpPost]
        [Route("read")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<BuildingDto>>> ReadAsync([FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            List<BuildingDto> buildingsDto = await _buildingService.ReadAsync(languageCode, cancellationToken);
            return Ok(buildingsDto);
        }

        [HttpPost]
        [Route("read-by-project-ids")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<BuildingDto>>> ReadByProjectIdsAsync([FromBody] int[] projectIds, string languageCode = null, CancellationToken cancellationToken = default)
        {
            List<BuildingDto> buildingsDto = await _buildingService.ReadByProjectIdsAsync(projectIds, languageCode, cancellationToken);
            return Ok(buildingsDto);
        }

        [HttpPost]
        [Route("read-data-table")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DataTablesResult<BuildingDto>>> ReadDataTableAsync([FromBody] DataTablesParameters dataTablesParameters, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            DataTablesResult<BuildingDto> buildingsDto = await _buildingService.ReadDataTableAsync(dataTablesParameters, languageCode, cancellationToken);
            return Ok(buildingsDto);
        }

        [HttpGet]
        [Route("read-editable-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BuildingEditableDto>> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            BuildingEditableDto buildingEditableDto = await _buildingService.ReadEditableByIdAsync(id, cancellationToken);
            return Ok(buildingEditableDto);
        }

        [HttpPost]
        [Route("search")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<BuildingDto>>> SearchAsync([FromQuery] string search = null, [FromQuery] string ordering = null, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            List<BuildingDto> buildingsDto = await _buildingService.SearchAsync(search, ordering, languageCode, cancellationToken);
            return Ok(buildingsDto);
        }

        [HttpPut]
        [Route("update-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BuildingEditableDto>> UpdateByIdAsync(int id, [FromBody] BuildingEditableDto buildingEditableDto, CancellationToken cancellationToken = default)
        {
            bool result = await _buildingService.UpdateByIdAsync(id, buildingEditableDto, cancellationToken);
            if (result)
            {
                return Ok(buildingEditableDto);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, _messages["Error updating the building in the database"].Value);
            }
        }
    }
}
