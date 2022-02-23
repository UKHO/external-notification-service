using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace UKHO.D365CallbackDistributorStub.API.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<T> : ControllerBase
    {
        protected IActionResult GetWebhookResponse()
        {
            return new OkObjectResult(StatusCodes.Status200OK);
        }
        protected IActionResult BuildInternalServerErrorResponse()
        {
            return new OkObjectResult(StatusCodes.Status500InternalServerError);
        }
    }
}
