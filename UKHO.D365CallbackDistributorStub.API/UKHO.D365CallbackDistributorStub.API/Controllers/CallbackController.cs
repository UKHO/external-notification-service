using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using UKHO.D365CallbackDistributorStub.API.Models.Request;
using UKHO.D365CallbackDistributorStub.API.Services;

namespace UKHO.D365CallbackDistributorStub.API.Controllers
{
    [Route("api/dynamics")]
    [ApiController]
    [ExcludeFromCodeCoverage]
    public class CallbackController : BaseController<CallbackController>
    {
        private readonly ILogger<CallbackController> _logger;
        private readonly CallbackService _callbackService;

        public CallbackController(ILogger<CallbackController> logger, CallbackService callbackService)
        {
            _logger = logger;
            _callbackService = callbackService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] CallbackRequest callbackRequest, string subscriptionId)
        {
            _logger.LogInformation("POST callback accessed for subscriptionId: {subscriptionId}", subscriptionId);
            bool callbackRequestSaved = CallbackService.SaveCallbackRequest(callbackRequest, subscriptionId);
            if (callbackRequestSaved)
            {
                _logger.LogInformation("Callback request stored in memory for subscriptionId: {subscriptionId}", subscriptionId);
                return NotContentResponse();
            }
            else
            {
                _logger.LogInformation("Callback request not stored in memory for subscriptionId: {subscriptionId}", subscriptionId);
                return BadRequestResponse();
            }
        }

        [HttpGet]
        public IActionResult Get(string? subscriptionId)
        {
            _logger.LogInformation("GET callback accessed for subscriptionId: {subscriptionId}", subscriptionId);
            List<RecordCallbackRequest>? callbackRequest = _callbackService.GetCallbackRequest(subscriptionId);

            if (callbackRequest == null || callbackRequest.Count == 0)
            {
                _logger.LogInformation("Callback not found for subscriptionId: {subscriptionId}", subscriptionId);
                return NotFoundResponse();
            }

            _logger.LogInformation("Callback found and return for subscriptionId: {subscriptionId}", subscriptionId);
            return Ok(callbackRequest);
        }
    }
}
