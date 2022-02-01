﻿using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Controllers;
using UKHO.ExternalNotificationService.API.Models;

namespace UKHO.ExternalNotificationService.API.UnitTests.Controllers
{
    [TestFixture]
    public class SubscriptionControllerTests
    {
        private SubscriptionController _controller;
        private ILogger<SubscriptionController> _fakeLogger;
        private IHttpContextAccessor _fakeHttpContextAccessor;

        [SetUp]
        public void Setup()
        {
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<SubscriptionController>>();
            A.CallTo(() => _fakeHttpContextAccessor.HttpContext).Returns(new DefaultHttpContext());
            _controller = new SubscriptionController(_fakeHttpContextAccessor, _fakeLogger);
        }

        [Test]
        public async Task TestSubscription()
        {
            var result = (StatusCodeResult)await _controller.Post(GetD365Payload());
            Assert.AreEqual(StatusCodes.Status202Accepted, result.StatusCode);
        }

        private static D365Payload GetD365Payload()
        {
            return new D365Payload
            {
                D365CorrelationId = "7b4cdb10-ddfd-4ed6-b2be-d1543d8b7272",
                OperationCreatedOn = "Date(1642158297000 + 0000)",
                InputParameters = new InputParameter[] { new InputParameter { value = new InputParameterValue
                {
                    Attributes= new D365Attribute[] { new D365Attribute { key = "subscribedacc", value = "test" }, new D365Attribute{ key = "test_name", value = "Clay" }},
                    FormattedValues =new FormattedValue[] { new FormattedValue { key ="state", value = "Active"}, new FormattedValue{ key = "acc", value = "A"}}
                }}}
            };
        }
    }
}
