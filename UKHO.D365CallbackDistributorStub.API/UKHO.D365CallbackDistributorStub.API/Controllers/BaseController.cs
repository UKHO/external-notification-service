using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace UKHO.D365CallbackDistributorStub.API.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<T> : ControllerBase
    {
        protected IActionResult GetOkResponse()
        {
            return new ObjectResult(StatusCodes.Status200OK);
        }

        protected IActionResult GetInternalServerErrorResponse()
        {
            return new ObjectResult(StatusCodes.Status500InternalServerError);
        }

        protected IActionResult GetNotContentResponse()
        {
            return new ObjectResult(StatusCodes.Status204NoContent);
        }

        protected IActionResult GetNotFoundResponse()
        {
            return new ObjectResult(StatusCodes.Status404NotFound);
        }

    }
}
