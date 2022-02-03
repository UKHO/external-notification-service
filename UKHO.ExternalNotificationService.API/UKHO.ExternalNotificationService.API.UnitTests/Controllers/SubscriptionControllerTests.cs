using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Controllers;
using UKHO.ExternalNotificationService.API.Helper;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Configuration;

namespace UKHO.ExternalNotificationService.API.UnitTests.Controllers
{
    [TestFixture]
    public class SubscriptionControllerTests
    {
        private SubscriptionController _controller;
        private SubscriptionService _fakeSubscriptionService;
        private ILogger<SubscriptionController> _fakeLogger;
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private IAzureMessageQueueHelper _fakeAzureMessageQueueHelper;
        private IOptions<EnsSubscriptionStorageConfiguration> _fakeEnsStorageConfiguration;

        [SetUp]
        public void Setup()
        {
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeSubscriptionService = A.Fake<SubscriptionService>();
            _fakeLogger = A.Fake<ILogger<SubscriptionController>>();
            _fakeAzureMessageQueueHelper = A.Fake<IAzureMessageQueueHelper>();
            _fakeEnsStorageConfiguration = A.Fake<IOptions<EnsSubscriptionStorageConfiguration>>();
            A.CallTo(() => _fakeHttpContextAccessor.HttpContext).Returns(new DefaultHttpContext());
            _controller = new SubscriptionController(_fakeHttpContextAccessor, _fakeSubscriptionService, _fakeAzureMessageQueueHelper, _fakeEnsStorageConfiguration, _fakeLogger);
        }

        [Test]
        public async Task TestSubscription()
        {
            dynamic jsonObject = new JObject();
            var result = await _controller.Post(jsonObject);
            Assert.IsInstanceOf<OkResult>(result);
        }
    }
}
