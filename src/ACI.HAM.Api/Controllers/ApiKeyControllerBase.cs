using Microsoft.AspNetCore.Mvc;

namespace ACI.HAM.Api.Controllers
{
    [ApiExplorerSettings(GroupName = "ApiKey")]
    [Produces("application/json")]
    [ApiController]
    public abstract class ApiKeyControllerBase : ControllerBase
    {
    }
}
