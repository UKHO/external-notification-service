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

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    public class SubscriptionController : BaseController<SubscriptionController>
    {
        private readonly ILogger<SubscriptionController> _logger;
        private readonly ISubscriptionService _subscriptionService;
        private List<Error> _errors = null;

        public SubscriptionController(IHttpContextAccessor contextAccessor, ILogger<SubscriptionController> logger, ISubscriptionService subscriptionService) : base(contextAccessor, logger)
        {
            _logger = logger;
            _subscriptionService = subscriptionService;
        }

        [HttpPost]
        public async virtual Task<IActionResult> Post([FromBody] D365Payload d365Payload)
        {
            if (d365Payload.InputParameters == null && d365Payload.PostEntityImages == null && d365Payload.CorrelationId == null)
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

            var validationResult = await _subscriptionService.ValidateSubscriptionRequest(subscription);

            if (!validationResult.IsValid && validationResult.HasBadRequestErrors(out _errors))
            {
                return BuildBadRequestErrorResponse(_errors);
            }

            _logger.LogInformation(EventIds.LogRequest.ToEventId(), "Subscription request Accepted", d365Payload);

            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.Accepted });
        }
    }
}
