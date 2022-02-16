using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    public class EesWebhookController : BaseController<EesWebhookController>
    {
        private readonly ILogger<EesWebhookController> _logger;
        public EesWebhookController(IHttpContextAccessor contextAccessor,
                                    ILogger<EesWebhookController> logger)
        : base(contextAccessor, logger)
        {
            _logger = logger;
        }

        [HttpOptions]
        [Route("/webhook/newEventspublished")]
        public IActionResult Options()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
                HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
                HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);
            }
            return GetWebhookResponse();
        }

        [HttpPost]
        [Route("/webhook/newEventspublished")]
        public virtual async Task<IActionResult> PostEesWebhook([FromBody] JObject request)
        {
            await Task.CompletedTask;
            return GetWebhookResponse();
        }
    }
}
