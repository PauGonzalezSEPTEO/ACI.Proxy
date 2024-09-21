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
    [Route("api/v{version:apiVersion}/boards")]
    [ApiVersion("1.0")]
    [EnableCors("ApiCorsPolicy")]
    public class BoardsController : ApiControllerBase
    {
        private readonly IBoardService _boardService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BoardsController(IBoardService boardService, IHttpContextAccessor httpContextAccessor)
        {
            _boardService = boardService ?? throw new ArgumentNullException(nameof(boardService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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
            var claimsPrincipal = _httpContextAccessor.HttpContext.User;
            DataTablesResult<BoardDto> boardsDto = await _boardService.ReadDataTableAsync(dataTablesParameters, claimsPrincipal, languageCode, cancellationToken);
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

                //ToDo: Add translations
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar la pensión en la base de datos.");

            }
        }
    }
}
