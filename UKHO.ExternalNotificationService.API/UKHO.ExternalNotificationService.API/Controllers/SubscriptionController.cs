using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Models.Response;
using System.Net;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;
using System.Collections.Generic;
using UKHO.ExternalNotificationService.API.Extensions;
using Newtonsoft.Json;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    public class SubscriptionController : BaseController<SubscriptionController>
    {
        private readonly ILogger<SubscriptionController> _logger;
        private readonly ISubscriptionService _subscriptionService;
        private List<Error> _errors = null;
        private readonly string  _xmsDynamicsMsgSizeExceededHeader ="x-ms-dynamics-msg-size-exceeded";

        public SubscriptionController(IHttpContextAccessor contextAccessor, ILogger<SubscriptionController> logger, ISubscriptionService subscriptionService) : base(contextAccessor, logger)
        {
            _logger = logger;
            _subscriptionService = subscriptionService;
        }

        [HttpPost]
        public async virtual Task<IActionResult> Post([FromBody] D365Payload d365Payload)
        {

            _logger.LogInformation(EventIds.ENSSubscriptionRequestStart.ToEventId(), "Subscription request for D365Payload:{JsonConvert.SerializeObject(objPayload)} with _X-Correlation-ID:{correlationId}", JsonConvert.SerializeObject(d365Payload), GetCurrentCorrelationId());

            if (HttpContext.Request.Headers.ContainsKey(_xmsDynamicsMsgSizeExceededHeader))
            {
                _logger.LogError(EventIds.DataTruncationD365HttpPayloadSizeExceeded.ToEventId(), "Data Truncation - D365 HTTP payload size exceeded, Recieved x-ms-dynamics-msg-size-exceeded header for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
            }

            if (d365Payload == null)
            {
                var error = new List<Error>
                {
                    new Error()
                    {
                        Source = "requestBody",
                        Description = "Either body is null or malformed."
                    }
                };
                return BuildBadRequestErrorResponse(error);
            }

            var validationD365PayloadResult = await _subscriptionService.ValidateD365PayloadRequest(d365Payload);

            if (!validationD365PayloadResult.IsValid && validationD365PayloadResult.HasBadRequestErrors(out _errors))
            {
                return BuildBadRequestErrorResponse(_errors);
            }

            SubscriptionRequest subscription = _subscriptionService.ConvertToSubscriptionRequestModel(d365Payload);

            _logger.LogInformation(EventIds.Accepted.ToEventId(), "Subscription request Accepted for D365Payload:{JsonConvert.SerializeObject(objPayload)} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{correlationId}", JsonConvert.SerializeObject(d365Payload), d365Payload.CorrelationId, GetCurrentCorrelationId());

            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.Accepted });
        }
    }
}
