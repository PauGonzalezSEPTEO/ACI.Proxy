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
    [Route("api/v{version:apiVersion}/boards")]
    [ApiVersion("1.0")]
    [EnableCors("ApiCorsPolicy")]
    public class BoardsController : JwtControllerBase
    {
        private readonly IBoardService _boardService;
        private readonly IStringLocalizer<UsersController> _messages;

        public BoardsController(IBoardService boardService, IStringLocalizer<UsersController> messages)
        {
            _boardService = boardService ?? throw new ArgumentNullException(nameof(boardService));
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        [HttpPost]
        [Route("create")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BoardEditableDto>> CreateAsync([FromBody] BoardEditableDto boardCreateDto, CancellationToken cancellationToken = default)
        {
            int id = await _boardService.CreateAsync(boardCreateDto, cancellationToken);
            return CreatedAtAction(nameof(ReadEditableByIdAsync), new { id }, boardCreateDto);
        }

        [HttpDelete]
        [Route("delete-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BoardEditableDto>> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            BoardEditableDto boardEditableDto = await _boardService.DeleteByIdAsync(id, cancellationToken);
            return Ok(boardEditableDto);
        }

        [HttpPost]
        [Route("read-data-table")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DataTablesResult<BoardDto>>> ReadDataTableAsync([FromBody] DataTablesParameters dataTablesParameters, [FromQuery] string languageCode = null, CancellationToken cancellationToken = default)
        {
            DataTablesResult<BoardDto> boardsDto = await _boardService.ReadDataTableAsync(dataTablesParameters, languageCode, cancellationToken);
            return Ok(boardsDto);
        }

        [HttpGet]
        [Route("read-editable-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BoardEditableDto>> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            BoardEditableDto boardEditableDto = await _boardService.ReadEditableByIdAsync(id, cancellationToken);
            return Ok(boardEditableDto);
        }

        [HttpPut]
        [Route("update-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BoardEditableDto>> UpdateByIdAsync(int id, [FromBody] BoardEditableDto boardEditableDto, CancellationToken cancellationToken = default)
        {
            bool result = await _boardService.UpdateByIdAsync(id, boardEditableDto, cancellationToken);
            if (result)
            {
                return Ok(boardEditableDto);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, _messages["Error updating the board in the database"].Value);
            }
        }
    }
}
