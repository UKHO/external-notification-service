﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using UKHO.D365CallbackDistributorStub.API.Models.Request;
using UKHO.D365CallbackDistributorStub.API.Services;

namespace UKHO.D365CallbackDistributorStub.API.Controllers
{
    [Route("api/[controller]")]
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
            bool isCallbackRequestSave = CallbackService.SaveCallbackRequest(callbackRequest, subscriptionId);
            if(isCallbackRequestSave)
            {
                _logger.LogInformation("Callback request stored in memory for subscriptionId: {subscriptionId}", subscriptionId);
                return GetNotContentResponse();
            }
            else
            {
                _logger.LogInformation("Callback request not stored in memory for subscriptionId: {subscriptionId}", subscriptionId);
                return GetBadRequestResponse();
            }
        }

        [HttpGet]
        public IActionResult Get(string subscriptionId)
        {
            _logger.LogInformation("GET callback accessed for subscriptionId: {subscriptionId}", subscriptionId);
            RecordCallbackRequest? callbackRequest = _callbackService.GetCallbackRequest(subscriptionId);
            if (callbackRequest == null)
            {
                _logger.LogInformation("Callback not found for subscriptionId: {subscriptionId}", subscriptionId);
                return GetNotFoundResponse();
            }
            _logger.LogInformation("Callback found and return for subscriptionId: {subscriptionId}", subscriptionId);
            return Ok(callbackRequest);
        }
    }
}