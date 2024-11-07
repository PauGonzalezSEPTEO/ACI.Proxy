using Microsoft.AspNetCore.Mvc;

namespace ACI.Proxy.Api.Controllers
{
    [ApiExplorerSettings(GroupName = "Public Api")]
    [Produces("application/json")]
    [ApiController]
    public abstract class ApiKeyControllerBase : ControllerBase
    {
    }
}
