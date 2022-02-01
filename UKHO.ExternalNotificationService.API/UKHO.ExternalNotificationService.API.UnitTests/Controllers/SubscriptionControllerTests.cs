using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Controllers;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Models.Request;

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
        public async Task WhenValidNullSubscriptionRequest_ThenPostReturnsOkResponse()
        {
            A.CallTo(() => _fakeSubscriptionService.ConvertToSubscriptionRequestModel(A<D365Payload>.Ignored)).Returns(_fakeSubscriptionRequest);

            var result = (StatusCodeResult)await _controller.Post(_fakeD365PayloadDetails);

            Assert.AreEqual(StatusCodes.Status202Accepted, result.StatusCode);
        }

        private D365Payload GetD365Payload()
        {
            var d365Payload = new D365Payload()
            {
                CorrelationId = "",
                InputParameters = new InputParameter[] { new InputParameter {
                                     value = new InputParameterValue {
                                         Attributes = new D365Attribute[] { new D365Attribute { key = "ukho_webhookurl", value= "https://dummy.address"}, new D365Attribute { key = "ukho_webhookurl", value = "https://dummy.address" } },
                                         FormattedValues = new FormattedValue[] { new FormattedValue { key = "ukho_subscriptiontype", value = "ADDS Data Pipeline" }, new FormattedValue { key = "ukho_subscriptiontype", value = "ADDS Data Pipeline" } },
                                     }}}
            };

            return d365Payload;
        }

        private SubscriptionRequest GetSubscriptionRequest()
        {
            return new SubscriptionRequest()
            {
                CorrelationId = "6ea03f10-2672-46fb-92a1-5200f6a4fedc",
                IsActive = true,
                NotificationType = "ADDS Data test",
                SubscriptionId = "ad0ccbc8-2975-ec11-8943-002248818233",
                WebhookUrl = "https://test.data"
            };
        }
    }
}
