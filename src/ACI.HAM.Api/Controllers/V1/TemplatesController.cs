using ACI.HAM.Mail.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACI.HAM.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class TemplatesController : JwtControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplatesController(ITemplateService templateService)
        {
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        }

        [HttpGet]
        [Route("get-template-by-name/{name}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> GetTemplateAsync(string name, CancellationToken cancellationToken = default)
        {
            string template = await _templateService.GetTemplateAsync(name, cancellationToken);
            return Ok(template);
        }
    }
}
