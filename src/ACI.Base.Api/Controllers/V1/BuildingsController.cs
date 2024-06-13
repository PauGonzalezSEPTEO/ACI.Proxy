using ACI.Base.Api.Controllers;
using ACI.Base.Core.Dtos;
using ACI.Base.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Base.Api.V1.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BuildingsController : ApiControllerBase
    {
        private readonly IBuildingService _buildingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BuildingsController(IBuildingService buildingService, IHttpContextAccessor httpContextAccessor)
        {
            _buildingService = buildingService ?? throw new ArgumentNullException(nameof(buildingService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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
        [Route("read-data-table")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DataTablesResult<BuildingDto>>> ReadDataTableAsync([FromBody] DataTablesParameters dataTablesParameters, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            var claimsPrincipal = _httpContextAccessor.HttpContext.User;
            DataTablesResult<BuildingDto> buildingsDto = await _buildingService.ReadDataTableAsync(dataTablesParameters, claimsPrincipal, languageCode, cancellationToken);
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
        public async Task<ActionResult<BuildingEditableDto>> UpdateByIdAsync(int id, [FromBody] BuildingEditableDto buildingEditableDto, CancellationToken cancellationToken = default)
        {
            bool result = await _buildingService.UpdateByIdAsync(id, buildingEditableDto, cancellationToken);
            return Ok(buildingEditableDto);
        }
    }
}
