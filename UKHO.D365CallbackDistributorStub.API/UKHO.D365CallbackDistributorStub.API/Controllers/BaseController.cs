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
            return Ok();
        }

        protected IActionResult GetBadRequestResponse()
        {
            return BadRequest();
        }

        protected IActionResult GetNotContentResponse()
        {
            return NoContent();
        }

        protected IActionResult GetNotFoundResponse()
        {
            return NotFound();
        }

    }
}
