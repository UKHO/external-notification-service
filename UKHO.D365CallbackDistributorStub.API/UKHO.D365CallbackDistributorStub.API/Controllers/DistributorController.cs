
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace UKHO.D365CallbackDistributorStub.API.Controllers
{
    [Route("/webhook/Notification")]
    [ApiController]
    public class DistributorController : BaseController<DistributorController>
    {
        private readonly ILogger<DistributorController> _logger;
        public DistributorController(ILogger<DistributorController> logger)
        {
            _logger = logger;
        }

        [HttpOptions]
        public IActionResult Options()
        {
            _logger.LogInformation("Distributor option accessed.");
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
                HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
                HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);
            }
            return GetWebhookResponse();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post()
        {
            _logger.LogInformation("Distributor Webhook posted.");

            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string jsonContent = await reader.ReadToEndAsync();
                return GetWebhookResponse();
            }
        }
    }
}
