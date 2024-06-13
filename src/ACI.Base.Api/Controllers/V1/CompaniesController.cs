using ACI.Base.Api.Controllers;
using ACI.Base.Core.Dtos;
using ACI.Base.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACI.Base.Api.V1.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v{version:apiVersion}/companies")]
    [ApiVersion("1.0")]
    [EnableCors("ApiCorsPolicy")]
    public class CompaniesController : ApiControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CompaniesController(ICompanyService companyService, IHttpContextAccessor httpContextAccessor)
        {
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        [HttpPost]
        [Authorize(Policy = "CanCreateCompany")]
        [Route("create")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CompanyEditableDto>> CreateAsync([FromBody] CompanyEditableDto companyCreateDto, CancellationToken cancellationToken = default)
        {
            int id = await _companyService.CreateAsync(companyCreateDto, cancellationToken);
            return CreatedAtAction(nameof(ReadEditableByIdAsync), new { id }, companyCreateDto);
        }

        [HttpDelete]
        [Route("delete-by-id/{id}")]
        [Authorize(Policy = "CanDeleteCompany")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CompanyEditableDto>> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            CompanyEditableDto companyEditableDto = await _companyService.DeleteByIdAsync(id, cancellationToken);
            return Ok(companyEditableDto);
        }

        [HttpPost]
        [Route("read")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<CompanyDto>>> ReadAsync([FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            List<CompanyDto> companiesDto = await _companyService.ReadAsync(languageCode, cancellationToken);
            return Ok(companiesDto);
        }

        [HttpPost]
        [Route("read-data-table")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DataTablesResult<CompanyDto>>> ReadDataTableAsync([FromBody] DataTablesParameters dataTablesParameters, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            var claimsPrincipal = _httpContextAccessor.HttpContext.User;
            DataTablesResult<CompanyDto> companiesDto = await _companyService.ReadDataTableAsync(dataTablesParameters, claimsPrincipal, languageCode, cancellationToken);
            return Ok(companiesDto);
        }

        [HttpGet]
        [Route("read-editable-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CompanyEditableDto>> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            CompanyEditableDto companyEditableDto = await _companyService.ReadEditableByIdAsync(id, cancellationToken);
            return Ok(companyEditableDto);
        }

        [HttpPost]
        [Route("search")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<CompanyDto>>> SearchAsync([FromQuery] string search = null, [FromQuery] string ordering = null, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            List<CompanyDto> companiesDto = await _companyService.SearchAsync(search, ordering, languageCode, cancellationToken);
            return Ok(companiesDto);
        }

        [HttpPut]
        [Route("update-by-id/{id}")]
        [Authorize(Policy = "CanUpdateCompany")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CompanyEditableDto>> UpdateByIdAsync(int id, [FromBody] CompanyEditableDto companyEditableDto, CancellationToken cancellationToken = default)
        {
            bool result = await _companyService.UpdateByIdAsync(id, companyEditableDto, cancellationToken);
            return Ok(companyEditableDto);
        }
    }
}
