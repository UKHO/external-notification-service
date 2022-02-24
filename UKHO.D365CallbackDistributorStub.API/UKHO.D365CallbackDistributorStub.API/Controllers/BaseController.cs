﻿using System.Diagnostics.CodeAnalysis;
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
            return new OkObjectResult(StatusCodes.Status200OK);
        }
        protected IActionResult GetInternalServerErrorResponse()
        {
            return new OkObjectResult(StatusCodes.Status500InternalServerError);
        }
    }
}
