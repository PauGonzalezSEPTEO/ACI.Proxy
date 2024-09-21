using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ACI.HAM.Api.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Produces("application/json")]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
    }
}
