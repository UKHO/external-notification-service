using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Net;

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

        protected IActionResult GetEnsStubResponse(HttpStatusCode httpStatusCode)
        {
            return httpStatusCode switch
            {
                HttpStatusCode.OK => OkResponse(),
                HttpStatusCode.NoContent => NoContentResponse(),
                HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError),
                HttpStatusCode.BadRequest => BadRequestResponse(),
                HttpStatusCode.NotFound => NotFoundResponse(),
                _ => BadRequestResponse(),
            };
        }

    }
}
