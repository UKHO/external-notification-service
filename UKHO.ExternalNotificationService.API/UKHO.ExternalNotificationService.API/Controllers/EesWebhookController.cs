using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Logging;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    public class EesWebhookController : BaseController<EesWebhookController>
    {
        private readonly ILogger<EesWebhookController> _logger;
        private readonly IEesWebhookService _eesWebhookService;

        public EesWebhookController(IHttpContextAccessor contextAccessor,
                                    ILogger<EesWebhookController> logger,
                                    IEesWebhookService eesWebhookService)
        : base(contextAccessor, logger)
        {
            _logger = logger;
            _eesWebhookService = eesWebhookService;
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

                CloudEvent cloudEvent = _eesWebhookService.TryGetCloudEventMessage(jsonContent);

                _logger.LogInformation(EventIds.EESWebhookRequestStart.ToEventId(), " Enterprise Event Service Webhook start for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

                return GetWebhookResponse();
            }
        }
    }
}
