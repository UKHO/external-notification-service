using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    [Authorize]
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
        public virtual async Task<IActionResult> Post()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string jsonContent = await reader.ReadToEndAsync();

                CustomEventGridEvent eventGridEvent = JsonConvert.DeserializeObject<CustomEventGridEvent>(jsonContent);

                _logger.LogInformation(EventIds.EESWebhookRequestStart.ToEventId(), "Enterprise event service webhook request started for event:{eventGridEvent} and _X-Correlation-ID:{correlationId}.", JsonConvert.SerializeObject(eventGridEvent), GetCurrentCorrelationId());

                return GetWebhookResponse();
            }
        }
    }
}
