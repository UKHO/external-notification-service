
using Microsoft.AspNetCore.Mvc;
using UKHO.D365CallbackDistributorStub.API.Models.Request;

namespace UKHO.D365CallbackDistributorStub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        List<RecordCallbackRequest> recordCallbackRequests = new List<RecordCallbackRequest>();
        [HttpPost]
        public IActionResult Post([FromBody] CallbackRequest callbackRequest)
        {
            recordCallbackRequests.Add(new RecordCallbackRequest
            {
                CallbackRequest = callbackRequest,
                Guid = Guid.NewGuid(),
                SubscriptionId = string.Empty

            });
            return NoContent();
        }
    }
}
