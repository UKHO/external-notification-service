using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Net;
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

        [Route("ukho_externalnotifications({subscriptionId})")]
        [HttpPatch]
        public IActionResult Patch([FromBody] CallbackRequest callbackRequest, string subscriptionId)
        {
            _logger.LogInformation("POST callback accessed for subscriptionId: {subscriptionId}", subscriptionId);

            CommandCallbackRequest? commandCallbackRequest = _callbackService.SubscriptionInCommandCallbackList(subscriptionId);
            bool callbackRequestSaved = CallbackService.SaveCallbackRequest(callbackRequest,
                                                                            subscriptionId,
                                                                            commandCallbackRequest == null ? HttpStatusCode.NoContent : commandCallbackRequest.HttpStatusCode);

            if (callbackRequestSaved)
            {
                _logger.LogInformation("Callback request stored in memory for subscriptionId: {subscriptionId}", subscriptionId);
                if (commandCallbackRequest != null)
                {
                    _logger.LogInformation("Callback request failed for subscriptionId: {subscriptionId} with httpStatusCode as {httpStatusCode}", subscriptionId, commandCallbackRequest.HttpStatusCode.ToString());
                    return GetEnsStubResponse(commandCallbackRequest.HttpStatusCode);
                }
                return NoContentResponse();
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

        [HttpPost("command-to-return-status/{subscriptionId}/{httpStatusCode?}")]
        public IActionResult CommandToReturnStatus(string subscriptionId, HttpStatusCode? httpStatusCode)
        {
            _logger.LogInformation("Command for callback accessed for subscriptionId: {subscriptionId}", subscriptionId);

            bool commandCallbackRequestSaved = _callbackService.SaveCommandCallbackRequest(subscriptionId, httpStatusCode);
            if (commandCallbackRequestSaved)
            {
                _logger.LogInformation("Command for callback stored in memory for subscriptionId: {subscriptionId} with httpStatusCode as {httpStatusCode}", subscriptionId, httpStatusCode.ToString());
                return OkResponse();
            }
            return BadRequestResponse();
        }
    }
}
