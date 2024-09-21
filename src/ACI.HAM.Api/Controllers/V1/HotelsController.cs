using ACI.HAM.Api.Controllers;
using ACI.HAM.Core.Dtos;
using ACI.HAM.Core.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACI.HAM.Api.V1.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v{version:apiVersion}/hotels")]
    [ApiVersion("1.0")]
    [EnableCors("ApiCorsPolicy")]
    public class HotelsController : ApiControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HotelsController(IHotelService hotelService, IHttpContextAccessor httpContextAccessor)
        {
            _hotelService = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Policy = "CanCreateHotel")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<HotelEditableDto>> CreateAsync([FromBody] HotelEditableDto hotelCreateDto, CancellationToken cancellationToken = default)
        {
            int id = await _hotelService.CreateAsync(hotelCreateDto, cancellationToken);
            return CreatedAtAction(nameof(ReadEditableByIdAsync), new { id }, hotelCreateDto);
        }

        [HttpDelete]
        [Route("delete-by-id/{id}")]
        [Authorize(Policy = "CanDeleteHotel")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<HotelEditableDto>> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            HotelEditableDto hotelEditableDto = await _hotelService.DeleteByIdAsync(id, cancellationToken);
            return Ok(hotelEditableDto);
        }

        [HttpPost]
        [Route("read-by-company-ids")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<HotelDto>>> ReadByCompanyIdsAsync([FromBody] int[] companyIds, CancellationToken cancellationToken = default)
        {
            var claimsPrincipal = _httpContextAccessor.HttpContext.User;
            List<HotelDto> hotelsDto = await _hotelService.ReadByCompanyIdsAsync(companyIds, claimsPrincipal, cancellationToken);
            return Ok(hotelsDto);
        }


        [HttpPost]
        [Route("read-data-table")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DataTablesResult<HotelDto>>> ReadDataTableAsync([FromBody] DataTablesParameters dataTablesParameters, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            var claimsPrincipal = _httpContextAccessor.HttpContext.User;
            DataTablesResult<HotelDto> hotelsDto = await _hotelService.ReadDataTableAsync(dataTablesParameters, claimsPrincipal, languageCode, cancellationToken);
            return Ok(hotelsDto);
        }

        [HttpGet]
        [Route("read-editable-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<HotelEditableDto>> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            HotelEditableDto hotelEditableDto = await _hotelService.ReadEditableByIdAsync(id, cancellationToken);
            return Ok(hotelEditableDto);
        }

        [HttpPost]
        [Route("search")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<HotelDto>>> SearchAsync([FromQuery] string search = null, [FromQuery] string ordering = null, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            List<HotelDto> hotelsDto = await _hotelService.SearchAsync(search, ordering, languageCode, cancellationToken);
            return Ok(hotelsDto);
        }

        [HttpPut]
        [Route("update-by-id/{id}")]
        [Authorize(Policy = "CanUpdateHotel")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HotelEditableDto>> UpdateByIdAsync(int id, [FromBody] HotelEditableDto hotelEditableDto, CancellationToken cancellationToken = default)
        {
            bool result = await _hotelService.UpdateByIdAsync(id, hotelEditableDto, cancellationToken);
            if (result)
            {
                return Ok(hotelEditableDto);
            }
            else
            {

                //ToDo: Add translations
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar el hotel en la base de datos.");

            }
        }
    }
}
