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
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;
using UKHO.ExternalNotificationService.Common.Repository;

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
        private INotificationRepository _fakeNotificationRepository;

        [SetUp]
        public void Setup()
        {
            _fakeD365PayloadDetails = GetD365Payload();
            _fakeSubscriptionRequest = GetSubscriptionRequest();

            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<SubscriptionController>>();
            _fakeSubscriptionService = A.Fake<ISubscriptionService>();
            _fakeNotificationRepository = A.Fake<INotificationRepository>();
            _controller = new SubscriptionController(_fakeHttpContextAccessor, _fakeLogger, _fakeSubscriptionService, _fakeNotificationRepository);
        }

        [Test] 
        public async Task WhenPostInvalidNullPayload_ThenRecieveBadRequest()
        {
            D365Payload d365Payload = null;

            var result = (BadRequestObjectResult)await _controller.Post(d365Payload);
            var errors = (ErrorDescription)result.Value;

            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Either body is null or malformed.", errors.Errors.Single().Description);
        }

        [Test] 
        public async Task WhenPostInvalidNullInputParameters_ThenRecieveBadRequest()
        {
            var validationMessage = new ValidationFailure("InputParameters", "D365Payload InputParameters cannot be blank or null.")
            {
                ErrorCode = HttpStatusCode.BadRequest.ToString()
            };

            A.CallTo(() => _fakeSubscriptionService.ValidateD365PayloadRequest(A<D365Payload>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure> { validationMessage }));

            var result = (BadRequestObjectResult)await _controller.Post(_fakeD365PayloadDetails);
            var errors = (ErrorDescription)result.Value;

            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("D365Payload InputParameters cannot be blank or null.", errors.Errors.Single().Description);
        }

        [Test]
        public async Task WhenPostInvalidNotificationTypesInPayload_ThenRecieveBadRequest()
        {
            var notificationType = new List<NotificationType>() { new NotificationType() { Name = "dataTest", TopicName = "testTopic" } };

            A.CallTo(() => _fakeSubscriptionService.ValidateD365PayloadRequest(A<D365Payload>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure>()));
            A.CallTo(() => _fakeSubscriptionService.ConvertToSubscriptionRequestModel(A<D365Payload>.Ignored)).Returns(_fakeSubscriptionRequest);
            A.CallTo(() => _fakeNotificationRepository.GetAllNotificationTypes()).Returns(notificationType);

            var result = (BadRequestObjectResult)await _controller.Post(_fakeD365PayloadDetails);
            var errors = (ErrorDescription)result.Value;

            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Invalid Notification Type 'Data test'", errors.Errors.Single().Description);
        }

        [Test]
        public async Task WhenPostValidPayload_ThenRecieveSuccessfulResponse()
        {
            var notificationType = new List<NotificationType>() { new NotificationType() { Name = "Data test", TopicName = "testTopic"}};

            A.CallTo(() => _fakeSubscriptionService.ValidateD365PayloadRequest(A<D365Payload>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure>()));
            A.CallTo(() => _fakeSubscriptionService.ConvertToSubscriptionRequestModel(A<D365Payload>.Ignored)).Returns(_fakeSubscriptionRequest);
            A.CallTo(() => _fakeNotificationRepository.GetAllNotificationTypes()).Returns(notificationType);

            var result = (StatusCodeResult)await _controller.Post(_fakeD365PayloadDetails);

            Assert.AreEqual(StatusCodes.Status202Accepted, result.StatusCode);
        }

        private D365Payload GetD365Payload()
        {
            var d365Payload = new D365Payload()
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
