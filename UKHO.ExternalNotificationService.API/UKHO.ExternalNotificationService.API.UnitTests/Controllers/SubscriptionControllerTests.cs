using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Controllers;

namespace UKHO.ExternalNotificationService.API.UnitTests.Controllers
{
    [TestFixture]
    public class SubscriptionControllerTests
    {
        private SubscriptionController _controller;
        private ILogger<SubscriptionController> fakeLogger;
        private IHttpContextAccessor fakeHttpContextAccessor;

        [SetUp]
        public void Setup()
        {
            fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            fakeLogger = A.Fake<ILogger<SubscriptionController>>();
            A.CallTo(() => fakeHttpContextAccessor.HttpContext).Returns(new DefaultHttpContext());
            _controller = new SubscriptionController(fakeHttpContextAccessor, fakeLogger);
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
