using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Controllers;
using UKHO.ExternalNotificationService.Common.Helper;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Storage;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.UnitTests.Controllers
{
    [TestFixture]
    public class SubscriptionControllerTests
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
        public async Task TestSubscription()
        {
            dynamic jsonObject = new JObject();
            A.CallTo(() => _fakeSubscriptionStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored))
                            .Returns("");
            A.CallTo(() => _fakeAzureMessageQueueHelper.AddQueueMessage(A<string>.Ignored, A<string>.Ignored, A<SubscriptionRequestMessage>.Ignored, A<string>.Ignored));
            var result = (StatusCodeResult)await _controller.Post(jsonObject);
            Assert.AreEqual(StatusCodes.Status202Accepted, result.StatusCode);
        }
    }
}
