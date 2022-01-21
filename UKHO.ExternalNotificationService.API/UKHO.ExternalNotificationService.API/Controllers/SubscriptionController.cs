using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        public SubscriptionController()
        {
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JObject jobj)
        {
            return Ok();
        }
    }
}
