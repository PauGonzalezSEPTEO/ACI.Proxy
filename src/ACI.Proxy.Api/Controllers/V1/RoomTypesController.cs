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
    [Route("api/v{version:apiVersion}/room-types")]
    [ApiVersion("1.0")]
    [EnableCors("ApiCorsPolicy")]
    public class RoomTypesController : JwtControllerBase
    {
        private readonly IStringLocalizer<UsersController> _messages;
        private readonly IRoomTypeService _roomTypeService;

        public RoomTypesController(IRoomTypeService roomTypeService, IStringLocalizer<UsersController> messages)
        {
            _roomTypeService = roomTypeService ?? throw new ArgumentNullException(nameof(roomTypeService));
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        [HttpPost]
        [Route("create")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RoomTypeEditableDto>> CreateAsync([FromBody] RoomTypeEditableDto roomTypeCreateDto, CancellationToken cancellationToken = default)
        {
            int id = await _roomTypeService.CreateAsync(roomTypeCreateDto, cancellationToken);
            return CreatedAtAction(nameof(ReadEditableByIdAsync), new { id }, roomTypeCreateDto);
        }

        [HttpDelete]
        [Route("delete-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RoomTypeEditableDto>> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            RoomTypeEditableDto roomTypeEditableDto = await _roomTypeService.DeleteByIdAsync(id, cancellationToken);
            return Ok(roomTypeEditableDto);
        }

        [HttpPost]
        [Route("read-data-table")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DataTablesResult<RoomTypeDto>>> ReadDataTableAsync([FromBody] DataTablesParameters dataTablesParameters, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            DataTablesResult<RoomTypeDto> roomTypesDto = await _roomTypeService.ReadDataTableAsync(dataTablesParameters, languageCode, cancellationToken);
            return Ok(roomTypesDto);
        }

        [HttpGet]
        [Route("read-editable-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RoomTypeEditableDto>> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            RoomTypeEditableDto roomTypeEditableDto = await _roomTypeService.ReadEditableByIdAsync(id, cancellationToken);
            return Ok(roomTypeEditableDto);
        }

        [HttpPut]
        [Route("update-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RoomTypeEditableDto>> UpdateByIdAsync(int id, [FromBody] RoomTypeEditableDto roomTypeEditableDto, CancellationToken cancellationToken = default)
        {
            bool result = await _roomTypeService.UpdateByIdAsync(id, roomTypeEditableDto, cancellationToken);
            if (result)
            {
                return Ok(roomTypeEditableDto);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, _messages["Error updating the room type in the database"].Value);
            }
        }
    }
}
