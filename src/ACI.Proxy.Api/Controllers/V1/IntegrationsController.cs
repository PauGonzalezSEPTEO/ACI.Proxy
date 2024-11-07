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
    public class IntegrationsController : JwtControllerBase
    {
        private readonly IIntegrationService _integrationService;
        private readonly IStringLocalizer<UsersController> _messages;

        public IntegrationsController(IIntegrationService integrationService, IStringLocalizer<UsersController> messages)
        {
            _integrationService = integrationService ?? throw new ArgumentNullException(nameof(integrationService));
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        [HttpPost]
        [Route("create")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IntegrationEditableDto>> CreateAsync([FromBody] IntegrationEditableDto integrationEditableDto, CancellationToken cancellationToken = default)
        {
            int id = await _integrationService.CreateAsync(integrationEditableDto, cancellationToken);
            return CreatedAtAction(nameof(ReadEditableByIdAsync), new { id }, integrationEditableDto);
        }

        [HttpDelete]
        [Route("delete-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IntegrationEditableDto>> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            IntegrationEditableDto integrationEditableDto = await _integrationService.DeleteByIdAsync(id, cancellationToken);
            return Ok(integrationEditableDto);
        }

        [HttpPost]
        [Route("read")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<IntegrationDto>>> ReadAsync([FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            List<IntegrationDto> integrationsDto = await _integrationService.ReadAsync(languageCode, cancellationToken);
            return Ok(integrationsDto);
        }

        [HttpPost]
        [Route("read-by-project-ids")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<IntegrationDto>>> ReadByProjectIdsAsync([FromBody] int[] projectIds, string languageCode = null, CancellationToken cancellationToken = default)
        {
            List<IntegrationDto> integrationsDto = await _integrationService.ReadByProjectIdsAsync(projectIds, languageCode, cancellationToken);
            return Ok(integrationsDto);
        }

        [HttpPost]
        [Route("read-data-table")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DataTablesResult<IntegrationDto>>> ReadDataTableAsync([FromBody] DataTablesParameters dataTablesParameters, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            DataTablesResult<IntegrationDto> integrationsDto = await _integrationService.ReadDataTableAsync(dataTablesParameters, languageCode, cancellationToken);
            return Ok(integrationsDto);
        }

        [HttpGet]
        [Route("read-editable-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IntegrationEditableDto>> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            IntegrationEditableDto integrationEditableDto = await _integrationService.ReadEditableByIdAsync(id, cancellationToken);
            return Ok(integrationEditableDto);
        }

        [HttpPost]
        [Route("search")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<IntegrationDto>>> SearchAsync([FromQuery] string search = null, [FromQuery] string ordering = null, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            List<IntegrationDto> integrationsDto = await _integrationService.SearchAsync(search, ordering, languageCode, cancellationToken);
            return Ok(integrationsDto);
        }

        [HttpPut]
        [Route("update-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IntegrationEditableDto>> UpdateByIdAsync(int id, [FromBody] IntegrationEditableDto integrationEditableDto, CancellationToken cancellationToken = default)
        {
            bool result = await _integrationService.UpdateByIdAsync(id, integrationEditableDto, cancellationToken);
            if (result)
            {
                return Ok(integrationEditableDto);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, _messages["Error updating the integration in the database"].Value);
            }
        }
    }
}
