using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UKHO.ExternalNotificationService.API.Extensions;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    public class SubscriptionController : BaseController<SubscriptionController>
    {
        private readonly ILogger<SubscriptionController> _logger;
        private readonly ISubscriptionService _subscriptionService;
        private List<Error> _errors;

        public SubscriptionController(IHttpContextAccessor contextAccessor, ILogger<SubscriptionController> logger, ISubscriptionService subscriptionService) : base(contextAccessor, logger)
        {
            _logger = logger;
            _subscriptionService = subscriptionService;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] D365Payload d365Payload)
        {
            _logger.LogInformation(EventIds.ENSSubscriptionRequestStart.ToEventId(), "Subscription request for D365Payload:{d365Payload} with _X-Correlation-ID:{correlationId}", JsonConvert.SerializeObject(d365Payload), GetCurrentCorrelationId());

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

            ValidationResult validationD365PayloadResult = await _subscriptionService.ValidateD365PayloadRequest(d365Payload);

            if (!validationD365PayloadResult.IsValid && validationD365PayloadResult.HasBadRequestErrors(out _errors))
            {
                return BuildBadRequestErrorResponse(_errors);
            }

            SubscriptionRequest subscription = _subscriptionService.ConvertToSubscriptionRequestModel(d365Payload);

            _logger.LogInformation(EventIds.Accepted.ToEventId(), "Subscription request Accepted for SubscriptionId:{subscriptionId} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{correlationId}", subscription.SubscriptionId, subscription.D365CorrelationId, GetCurrentCorrelationId());

            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.Accepted });
        }
    }
}
