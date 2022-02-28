using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace UKHO.D365CallbackDistributorStub.API.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    public abstract class BaseController<T> : ControllerBase
    {
        protected IActionResult OkResponse()
        {
            return Ok();
        }

        protected IActionResult BadRequestResponse()
        {
            return BadRequest();
        }

        protected IActionResult NoContentResponse()
        {
            return NoContent();
        }

        protected IActionResult NotFoundResponse()
        {
            return NotFound();
        }

    }
}
