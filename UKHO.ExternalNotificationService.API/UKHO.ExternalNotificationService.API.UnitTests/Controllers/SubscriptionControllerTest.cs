using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Controllers;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Storage;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.API.Models;
using UKHO.ExternalNotificationService.Common.Helpers;

namespace UKHO.ExternalNotificationService.API.UnitTests.Controllers
{
    [TestFixture]
    public class SubscriptionControllerTest
    {
        private SubscriptionController _controller;
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private ILogger<SubscriptionController> _fakeLogger;
        private ISubscriptionService _fakeSubscriptionService;       
        private IAzureMessageQueueHelper _fakeAzureMessageQueueHelper;
        private IOptions<SubscriptionStorageConfiguration> _fakeStorageConfiguration;
        private ISubscriptionStorageService _fakeSubscriptionStorageService;

        [SetUp]
        public void Setup()
        {
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<SubscriptionController>>();
            _fakeSubscriptionService = A.Fake<ISubscriptionService>();                              
            _fakeAzureMessageQueueHelper = A.Fake<IAzureMessageQueueHelper>();
            _fakeStorageConfiguration = A.Fake<IOptions<SubscriptionStorageConfiguration>>();
            _fakeSubscriptionStorageService = A.Fake<ISubscriptionStorageService>();

            A.CallTo(() => _fakeHttpContextAccessor.HttpContext).Returns(new DefaultHttpContext());

            _controller = new SubscriptionController(_fakeHttpContextAccessor, _fakeLogger, _fakeSubscriptionService, _fakeAzureMessageQueueHelper, _fakeStorageConfiguration, _fakeSubscriptionStorageService);
        }

        [Test]
        public async Task WhenPostValidPayload_ThenRecieveSuccessfulResponse()
        {
            A.CallTo(() => _fakeSubscriptionStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored))
                           .Returns("");
            A.CallTo(() => _fakeAzureMessageQueueHelper.AddQueueMessage(A<string>.Ignored, A<string>.Ignored, A<SubscriptionRequestMessage>.Ignored, A<string>.Ignored));
            var result = (StatusCodeResult)await _controller.Post(GetD365Payload());
            Assert.AreEqual(StatusCodes.Status202Accepted, result.StatusCode);
        }

        private static D365Payload GetD365Payload()
        {
            return new D365Payload
            {
                CorrelationId = "7b4cdb10-ddfd-4ed6-b2be-d1543d8b7272",
                OperationCreatedOn = "Date(1000097000 + 0000)",
                InputParameters = new InputParameter[] { new InputParameter { Value = new InputParameterValue
                {
                    Attributes = new D365Attribute[] { new D365Attribute { Key = "subscribedacc", Value = "test" }, new D365Attribute{ Key = "test_name", Value = "Clay" }},
                    FormattedValues = new FormattedValue[] { new FormattedValue { Key ="state", Value = "Active"}, new FormattedValue{ Key = "acc", Value = "A"}}
                }}},
                PostEntityImages = new EntityImage[] { new EntityImage { Key = "AsynchronousTestName" , ImageValue = new EntityImageValue
                {
                    Attributes = new D365Attribute[] { new D365Attribute { Key = "subscribedacc", Value = "test" }, new D365Attribute{ Key = "test_name", Value = "Clay" }},
                    FormattedValues = new FormattedValue[] { new FormattedValue { Key ="state", Value = "Active"}, new FormattedValue{ Key = "acc", Value = "A"}}
                }}}
            };
        }
    }
}
