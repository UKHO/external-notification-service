using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Net;
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
        public async virtual Task<IActionResult> Post([FromBody] JObject jobj)
        {
            _logger.LogInformation(EventIds.LogRequest.ToEventId(), "Subcription request Accepted", jobj);
            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.OK });
        }
    }
}
