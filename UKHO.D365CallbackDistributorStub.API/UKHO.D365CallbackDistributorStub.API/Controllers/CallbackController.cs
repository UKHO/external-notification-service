using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
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

            FailCallbackRequest? failCallbackRequest = _callbackService.SubscriptionInFailCallbackList(subscriptionId);
            if (failCallbackRequest != null)
            {
                _logger.LogInformation("Callback request failed for subscriptionId: {subscriptionId} with httpStatusCode as {httpStatusCode}", subscriptionId, failCallbackRequest.httpStatusCode.ToString());
                return GetEnsStubResponse(failCallbackRequest.httpStatusCode);
            }
            else
            {
                bool callbackRequestSaved = CallbackService.SaveCallbackRequest(callbackRequest, subscriptionId);
                if (callbackRequestSaved)
                {
                    _logger.LogInformation("Callback request stored in memory for subscriptionId: {subscriptionId}", subscriptionId);
                    return NoContentResponse();
                }
                else
                {
                    _logger.LogInformation("Callback request not stored in memory for subscriptionId: {subscriptionId}", subscriptionId);
                    return BadRequestResponse();
                }
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

        [HttpPost("command-to-return-status/{subscriptionId}/{httpStatusCode?}")]
        public IActionResult CommandToReturnStatus(string subscriptionId, HttpStatusCode? httpStatusCode)
        {
            _logger.LogInformation("fail callback accessed for subscriptionId: {subscriptionId}", subscriptionId);

            bool failCallbackRequestSaved = _callbackService.SaveFailCallbackRequest(subscriptionId, httpStatusCode);
            if (failCallbackRequestSaved)
            {
                _logger.LogInformation("Fail callback stored in memory for subscriptionId: {subscriptionId} with httpStatusCode as {httpStatusCode}", subscriptionId, httpStatusCode.ToString());
                return OkResponse();
            }
            return BadRequestResponse();
        }
    }
}
