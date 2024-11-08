using ACI.Proxy.Mail.Dtos;
using ACI.Proxy.Mail.Localization;
using ACI.Proxy.Mail.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Proxy.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class MailController : JwtControllerBase
    {
        private readonly IMailService _mailService;

        public MailController(IMailService mailService)
        {
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }

        [HttpGet]
        [Route("get-models")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<string[]> GetModels()
        {
            var models = _mailService.GetModels();
            return Ok(models);
        }

        [HttpGet]
        [Route("get-template-by-name/{name}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<GetTemplateByNameDto>> GetTemplateByNameAsync(string name, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var template = await _mailService.GetTemplateByNameAsync(name, languageCode, cancellationToken);
            return Ok(template);
        }

        [HttpPost]
        [Route("send-mail")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> SendMailAsync(SendMailDto mailDto, CancellationToken cancellationToken = default)
        {
            bool result = await _mailService.SendMailAsync(mailDto, cancellationToken);
            if (result)
            {
                return StatusCode(StatusCodes.Status200OK, DataAnnotations.Mail_has_successfully_been_sent);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataAnnotations.An_error_occured__The_mail_could_not_be_sent);
            }
        }
    }
}
