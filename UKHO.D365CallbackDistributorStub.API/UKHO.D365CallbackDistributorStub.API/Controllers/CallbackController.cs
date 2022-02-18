using Microsoft.AspNetCore.Mvc;
using UKHO.D365CallbackDistributorStub.API.Models.Request;

namespace UKHO.D365CallbackDistributorStub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        private readonly ILogger<CallbackController> _logger;
        public CallbackController(ILogger<CallbackController> logger)
        {
            _logger = logger;
        }

        readonly List<RecordCallbackRequest> recordCallbackRequests = new();
        [HttpPost]
        public IActionResult Post([FromBody] CallbackRequest callbackRequest)
        {
            _logger.LogInformation("Callback posted.");
            recordCallbackRequests.Add(new RecordCallbackRequest
            {
                CallbackRequest = callbackRequest,
                Guid = Guid.NewGuid(),
                SubscriptionId = string.Empty

            });
            _logger.LogInformation("Callback request stored in memory.");
            return NoContent();
        }
    }
}
