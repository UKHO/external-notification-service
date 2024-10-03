using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.Services;
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
        private readonly IEventProcessorFactory _eventProcessorFactory;

        public WebhookController(IHttpContextAccessor contextAccessor,
                                    ILogger<WebhookController> logger,
                                    IEventProcessorFactory eventProcessorFactory)
        : base(contextAccessor, logger)
        {
            _logger = logger;
            _eventProcessorFactory = eventProcessorFactory;
        }

        [HttpOptions]
        public IActionResult Options()
        {
            string? webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
            HttpContext.Response.Headers.Append("WebHook-Allowed-Rate", "*");
            if (!string.IsNullOrWhiteSpace(webhookRequestOrigin))
            {
               HttpContext.Response.Headers.Append("WebHook-Allowed-Origin", webhookRequestOrigin);
            }

            _logger.LogInformation(EventIds.ENSWebhookOptionsEndPointRequested.ToEventId(), "External notification service webhook options end point requested for _X-Correlation-ID:{correlationId}.", GetCurrentCorrelationId());

            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.OK });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            string jsonContent = await reader.ReadToEndAsync();
            CustomCloudEvent? customCloudEvent = JsonSerializer.Deserialize<CustomCloudEvent>(jsonContent, JOptions);

            _logger.LogInformation(EventIds.ENSWebhookRequestStart.ToEventId(), "External notification service webhook request started for event:{eventGridEvent} and _X-Correlation-ID:{correlationId}.", JsonSerializer.Serialize(customCloudEvent), GetCurrentCorrelationId());

            if (customCloudEvent?.Type != null)
            {
                IEventProcessor? processor = _eventProcessorFactory.GetProcessor(customCloudEvent.Type);

                if (processor != null)
                {
                    ExternalNotificationServiceProcessResponse result = await processor.Process(customCloudEvent, GetCurrentCorrelationId());

                    if (result.Errors != null && result.Errors.Count > 0)
                    {
                        _logger.LogInformation(EventIds.ENSWebhookRequestInvalidEventPayload.ToEventId(), "External notification service webhook request has an invalid payload for subject:{subject}, businessUnit:{businessUnit} and _X-Correlation-ID:{correlationId}.", customCloudEvent.Subject, result.BusinessUnit, GetCurrentCorrelationId());
                        _logger.LogInformation(EventIds.ENSWebhookRequestCompleted.ToEventId(), "External notification service webhook request successfully completed for subject:{subject} and _X-Correlation-ID:{correlationId}.", customCloudEvent.Subject, GetCurrentCorrelationId());
                        
                        return BuildOkRequestErrorResponse(result.Errors);
                    }
                }
                else 
                {
                    _logger.LogInformation(EventIds.ENSWebhookRequestTypeNotMatch.ToEventId(), "External notification service webhook request failed due to receiving an event with a type that cannot be handled for subject:{subject}, type:{type} and _X-Correlation-ID:{correlationId}.", customCloudEvent.Subject, customCloudEvent.Type, GetCurrentCorrelationId());
                }

                _logger.LogInformation(EventIds.ENSWebhookRequestCompleted.ToEventId(), "External notification service webhook request successfully completed for subject:{subject} and _X-Correlation-ID:{correlationId}.", customCloudEvent.Subject, GetCurrentCorrelationId());
            }
            else
            {
                _logger.LogInformation(EventIds.ENSWebhookRequestWithNullEventPayload.ToEventId(), "External notification service webhook request has an invalid payload being null for _X-Correlation-ID:{correlationId}.", GetCurrentCorrelationId());
                _logger.LogInformation(EventIds.ENSWebhookRequestCompleted.ToEventId(), "External notification service webhook request successfully completed for _X-Correlation-ID:{correlationId}.", GetCurrentCorrelationId());
            }

            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.OK });
        }
    }
}
