using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    [Authorize]
    public class WebhookController : BaseController<WebhookController>
    {
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IHttpContextAccessor contextAccessor,
                                    ILogger<WebhookController> logger)
        : base(contextAccessor, logger)
        {
            _logger = logger;
        }

        [HttpOptions]
        public IActionResult Options()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
                HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
                HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);
            }
            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.OK });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string jsonContent = await reader.ReadToEndAsync();

                CustomEventGridEvent eventGridEvent = JsonConvert.DeserializeObject<CustomEventGridEvent>(jsonContent);

                _logger.LogInformation(EventIds.ENSWebhookRequestStart.ToEventId(), "External notification service webhook request started for event:{eventGridEvent} and _X-Correlation-ID:{correlationId}.", JsonConvert.SerializeObject(eventGridEvent), GetCurrentCorrelationId());

                return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.OK });
            }
        }
    }
}
