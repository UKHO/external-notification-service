using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public async virtual Task<IActionResult> Post([FromBody] D365Payload objPayload)
        {
            _logger.LogInformation(EventIds.Accepted.ToEventId(), "Subscription request Accepted", objPayload);
            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.Accepted });
        }
    }
}
