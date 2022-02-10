﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FakeItEasy;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Controllers;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;

namespace UKHO.ExternalNotificationService.API.UnitTests.Controllers
{
    [TestFixture]
    public class SubscriptionControllerTests
    {
        private SubscriptionController _controller;
        private ILogger<SubscriptionController> _fakeLogger;
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private ISubscriptionService _fakeSubscriptionService;
        private D365Payload _fakeD365PayloadDetails;
        private SubscriptionRequest _fakeSubscriptionRequest;
        private readonly string _xmsDynamicsMsgSizeExceededHeader = "x-ms-dynamics-msg-size-exceeded";

        [SetUp]
        public void Setup()
        {
            _fakeD365PayloadDetails = GetD365Payload();
            _fakeSubscriptionRequest = GetSubscriptionRequest();

            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<SubscriptionController>>();
            _fakeSubscriptionService = A.Fake<ISubscriptionService>();

            _controller = new SubscriptionController(_fakeHttpContextAccessor, _fakeLogger, _fakeSubscriptionService);
        }

        [Test] 
        public async Task WhenPostInvalidNullPayload_ThenRecieveBadRequest()
        {
            D365Payload d365Payload = null;

            BadRequestObjectResult result = (BadRequestObjectResult)await _controller.Post(d365Payload);
            ErrorDescription errors = (ErrorDescription)result.Value;

            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Either body is null or malformed.", errors.Errors.Single().Description);
        }

        [Test] 
        public async Task WhenPostInvalidNullInputParameters_ThenRecieveBadRequest()
        {
            ValidationFailure validationMessage = new ValidationFailure("InputParameters", "D365Payload InputParameters cannot be blank or null.")
            {
                ErrorCode = HttpStatusCode.BadRequest.ToString()
            };

            A.CallTo(() => _fakeSubscriptionService.ValidateD365PayloadRequest(A<D365Payload>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure> { validationMessage }));

            BadRequestObjectResult result = (BadRequestObjectResult)await _controller.Post(_fakeD365PayloadDetails);
            ErrorDescription errors = (ErrorDescription)result.Value;

            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("D365Payload InputParameters cannot be blank or null.", errors.Errors.Single().Description);
        }

        [Test]
        public async Task WhenPostValidPayload_ThenRecieveSuccessfulResponse()
        {
            A.CallTo(() => _fakeSubscriptionService.ValidateD365PayloadRequest(A<D365Payload>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure>()));

            A.CallTo(() => _fakeSubscriptionService.ConvertToSubscriptionRequestModel(A<D365Payload>.Ignored)).Returns(_fakeSubscriptionRequest);
            
            StatusCodeResult result = (StatusCodeResult)await _controller.Post(_fakeD365PayloadDetails);

            A.CallTo(_fakeLogger).Where(call => call.GetArgument<LogLevel>(0) == LogLevel.Error).MustNotHaveHappened();
            Assert.AreEqual(StatusCodes.Status202Accepted, result.StatusCode);
        }

        [Test]
        public async Task WhenD365HttpPayloadSizeExceeded_ThenLogError()
        {
            DefaultHttpContext defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Headers.Add(_xmsDynamicsMsgSizeExceededHeader, string.Empty);
            A.CallTo(() => _fakeHttpContextAccessor.HttpContext).Returns(defaultHttpContext);
            A.CallTo(() => _fakeSubscriptionService.ConvertToSubscriptionRequestModel(A<D365Payload>.Ignored)).Returns(_fakeSubscriptionRequest);

            StatusCodeResult result = (StatusCodeResult)await _controller.Post(_fakeD365PayloadDetails);

            A.CallTo(_fakeLogger).Where(call => call.GetArgument<LogLevel>(0) == LogLevel.Error).MustHaveHappened();
            Assert.AreEqual(StatusCodes.Status202Accepted, result.StatusCode);
        }

        private D365Payload GetD365Payload()
        {
            D365Payload d365Payload = new D365Payload()
            {
                CorrelationId = "6ea03f10-2672-46fb-92a1-5200f6a4fabc",
                InputParameters = new InputParameter[] {},
                PostEntityImages = new EntityImage[] { },
                OperationCreatedOn = "/Date(1642149320000+0000)/"
            };

            return d365Payload;
        }

        private SubscriptionRequest GetSubscriptionRequest()
        {
            return new SubscriptionRequest()
            {
                D365CorrelationId = "6ea03f10-2672-46fb-92a1-5200f6a4fabc",
                IsActive = true,
                NotificationType = "Data test",
                SubscriptionId = "ad0ccbc8-2975-ec11-8943-002248818111",
                WebhookUrl = "https://test.data"
            };
        }
    }
}
