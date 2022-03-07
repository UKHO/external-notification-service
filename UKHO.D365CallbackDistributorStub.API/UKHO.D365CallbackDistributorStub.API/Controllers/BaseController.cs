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
            switch (httpStatusCode)
            {
                case HttpStatusCode.OK:
                    return OkResponse();

                case HttpStatusCode.NoContent:
                    return NoContentResponse();

                case HttpStatusCode.InternalServerError:
                    return StatusCode((int)HttpStatusCode.InternalServerError);

                case HttpStatusCode.BadRequest:
                    return BadRequestResponse();

                case HttpStatusCode.NotFound:
                    return NotFoundResponse();

                default:
                    return BadRequestResponse();
            }
        }

    }
}
