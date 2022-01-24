using System.Threading.Tasks;
using FakeItEasy;
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

        [SetUp]
        public void Setup()
        {
            fakeLogger = A.Fake<ILogger<SubscriptionController>>();
            _controller = new SubscriptionController(fakeLogger);
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
