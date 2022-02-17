
using Microsoft.AspNetCore.Mvc;

namespace UKHO.D365CallbackDistributorStub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistributorController : ControllerBase
    {
        public async virtual Task<IActionResult> Post()
        {
            return Ok();
        }
    }
}
