using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UKHO.ExternalNotificationService.API.Models;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Response;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    public class SubscriptionController : BaseController<SubscriptionController>
    {
        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(IHttpContextAccessor contextAccessor, ILogger<SubscriptionController> logger):base(contextAccessor, logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] D365Payload objPayload)
        {
            _logger.LogInformation(EventIds.Accepted.ToEventId(), "Subscription request Accepted for D365Payload:{JsonConvert.SerializeObject(objPayload)} with _X-Correlation-ID:{correlationId}", JsonConvert.SerializeObject(objPayload), GetCurrentCorrelationId());
            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.Accepted });
        }
    }
}
