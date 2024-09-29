using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ACI.HAM.Api.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "Jwt")]
    [Produces("application/json")]
    [ApiController]
    public abstract class JwtControllerBase : ControllerBase
    {
    }
}
