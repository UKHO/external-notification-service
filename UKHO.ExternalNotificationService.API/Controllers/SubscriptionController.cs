using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        public SubscriptionController()
        {

        }

        [HttpPost]
        [Route("/api/subscription")]
        public async Task<IActionResult> Post(JObject jobj)
        {
            return Ok();
        }
    }
}
