using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using UKHO.ExternalNotificationService.Common.Logging;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(ILogger<SubscriptionController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JObject jobj)
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            _logger.LogInformation(EventIds.LogRequest.ToEventId(), "test log is generating or not {id}", id);
            return Ok("tested");
        }
    }
}
