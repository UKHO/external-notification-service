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
            string webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
            HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
            HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);

            _logger.LogInformation(EventIds.ENSWebhookOptionsEndPointRequested.ToEventId(), "External notification service webhook options end point requested for _X-Correlation-ID:{correlationId}.", GetCurrentCorrelationId());

            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.OK });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            string jsonContent = await reader.ReadToEndAsync();
            CustomEventGridEvent customEventGridEvent = JsonConvert.DeserializeObject<CustomEventGridEvent>(jsonContent);

            _logger.LogInformation(EventIds.ENSWebhookRequestStart.ToEventId(), "External notification service webhook request started for event:{eventGridEvent} and _X-Correlation-ID:{correlationId}.", JsonConvert.SerializeObject(customEventGridEvent), GetCurrentCorrelationId());

            if (customEventGridEvent != null)
            {
                IEventProcessor processor = _eventProcessorFactory.GetProcessor(customEventGridEvent.Type);

                if (processor != null)
                {
                    ExternalNotificationServiceProcessResponse result = await processor.Process(customEventGridEvent, GetCurrentCorrelationId());

                    if (result.Errors != null && result.Errors.Count > 0)
                    {
                        _logger.LogInformation(EventIds.ENSWebhookRequestInvalidEventPayload.ToEventId(), "External notification service webhook request failed due to an invalid event payload for subject:{subject}, businessUnit:{businessUnit} and _X-Correlation-ID:{correlationId}.", customEventGridEvent.Subject, result.BusinessUnit, GetCurrentCorrelationId());
                        _logger.LogInformation(EventIds.ENSWebhookRequestCompleted.ToEventId(), "External notification service webhook request successfully completed for subject:{subject} and _X-Correlation-ID:{correlationId}.", customEventGridEvent.Subject, GetCurrentCorrelationId());
                        
                        return BuildOkRequestErrorResponse(result.Errors);
                    }
                }
                else 
                {
                    _logger.LogInformation(EventIds.ENSWebhookRequestTypeNotMatch.ToEventId(), "External notification service webhook request failed due to receiving an event with a type that cannot be handled for subject:{subject}, type:{type} and _X-Correlation-ID:{correlationId}.", customEventGridEvent.Subject, customEventGridEvent.Type, GetCurrentCorrelationId());
                }

                _logger.LogInformation(EventIds.ENSWebhookRequestCompleted.ToEventId(), "External notification service webhook request successfully completed for subject:{subject} and _X-Correlation-ID:{correlationId}.", customEventGridEvent.Subject, GetCurrentCorrelationId());
            }
            else
            {
                _logger.LogInformation(EventIds.ENSWebhookRequestWithNullEventPayload.ToEventId(), "External notification service webhook request failed due to the event payload being null for _X-Correlation-ID:{correlationId}.", GetCurrentCorrelationId());
                _logger.LogInformation(EventIds.ENSWebhookRequestCompleted.ToEventId(), "External notification service webhook request successfully completed for _X-Correlation-ID:{correlationId}.", GetCurrentCorrelationId());
            }

            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.OK });
        }
    }
}
