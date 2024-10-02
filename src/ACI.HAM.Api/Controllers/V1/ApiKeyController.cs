using ACI.HAM.Core.Dtos;
using ACI.HAM.Core.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACI.HAM.Api.Controllers.V1
{
    [Route("publicapi/v{version:apiVersion}")]
    [ApiVersion("1.0")]
    [EnableCors("ApiCorsPolicy")]
    public class ApiKeyController : ApiKeyControllerBase
    {
        private readonly IBoardService _boardService;

        public ApiKeyController(IBoardService boardService)
        {
            _boardService = boardService ?? throw new ArgumentNullException(nameof(boardService));
        }

        [HttpGet]
        [Route("/boards/read-by-id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BoardEditableDto>> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            BoardEditableDto boardEditableDto = await _boardService.ReadEditableByIdAsync(id, cancellationToken);
            return Ok(boardEditableDto);
        }
    }
}
