using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Models.Response;
using System.Net;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    public class SubscriptionController : BaseController<SubscriptionController>
    {
        private readonly ILogger<SubscriptionController> _logger;
        private readonly ISubscriptionService _subscriptionService;
        public SubscriptionController(IHttpContextAccessor contextAccessor, ILogger<SubscriptionController> logger, ISubscriptionService subscriptionService) : base(contextAccessor, logger)
        {
            _logger = logger;
            _subscriptionService = subscriptionService;
        }

        [HttpPost]
        public async virtual Task<IActionResult> Post([FromBody] D365Payload d365Payload)
        {
            SubscriptionRequest subscription = _subscriptionService.ConvertToSubscriptionRequestModel(d365Payload);

            _logger.LogInformation(EventIds.LogRequest.ToEventId(), "Subscription request Accepted", d365Payload);

            await Task.CompletedTask;
            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.Accepted });
        }
    }
}
