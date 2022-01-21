using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Controllers;

namespace UKHO.ExternalNotificationService.API.UnitTests.Controllers
{
    [TestFixture]
    public class SubscriptionControllerTests
    {
        private SubscriptionController controller;

        [SetUp]
        public void Setup()
        {
            controller = new SubscriptionController();
        }

        [Test]
        public async Task TestSubscription()
        {
            dynamic jsonObject = new JObject();
            var result = await controller.Post(jsonObject);
            Assert.IsInstanceOf<OkResult>(result);
        }
    }
}
