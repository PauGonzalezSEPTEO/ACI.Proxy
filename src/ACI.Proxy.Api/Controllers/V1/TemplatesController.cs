using ACI.Proxy.Api.Controllers;
using ACI.Proxy.Core.Dtos;
using ACI.Proxy.Core.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ACI.Proxy.Api.V1.Controllers
{
    [Route("api/v{version:apiVersion}/templates")]
    [ApiVersion("1.0")]
    [EnableCors("ApiCorsPolicy")]
    public class TemplatesController : JwtControllerBase
    {
        private readonly ITemplateService _templateService;
        private readonly IStringLocalizer<UsersController> _messages;

        public TemplatesController(ITemplateService templateService, IStringLocalizer<UsersController> messages)
        {
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        [HttpPost]
        [Route("create")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TemplateEditableDto>> CreateAsync([FromBody] TemplateEditableDto templateCreateDto, CancellationToken cancellationToken = default)
        {
            int id = await _templateService.CreateAsync(templateCreateDto, cancellationToken);
            return CreatedAtAction(nameof(ReadEditableByIdAsync), new { id }, templateCreateDto);
        }

        [HttpDelete]
        [Route("delete-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TemplateEditableDto>> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            TemplateEditableDto templateEditableDto = await _templateService.DeleteByIdAsync(id, cancellationToken);
            return Ok(templateEditableDto);
        }

        [HttpPost]
        [Route("read-data-table")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DataTablesResult<TemplateDto>>> ReadDataTableAsync([FromBody] DataTablesParameters dataTablesParameters, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            DataTablesResult<TemplateDto> templatesDto = await _templateService.ReadDataTableAsync(dataTablesParameters, languageCode, cancellationToken);
            return Ok(templatesDto);
        }

        [HttpGet]
        [Route("read-editable-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TemplateEditableDto>> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            TemplateEditableDto templateEditableDto = await _templateService.ReadEditableByIdAsync(id, cancellationToken);
            return Ok(templateEditableDto);
        }

        [HttpPut]
        [Route("update-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TemplateEditableDto>> UpdateByIdAsync(int id, [FromBody] TemplateEditableDto templateEditableDto, CancellationToken cancellationToken = default)
        {
            bool result = await _templateService.UpdateByIdAsync(id, templateEditableDto, cancellationToken);
            if (result)
            {
                return Ok(templateEditableDto);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, _messages["Error updating the template in the database"].Value);
            }
        }
    }
}
