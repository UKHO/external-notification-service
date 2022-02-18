using System.Threading.Tasks;
using Azure.Messaging;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Controllers;
using UKHO.ExternalNotificationService.API.Services;

namespace UKHO.ExternalNotificationService.API.UnitTests.Controllers
{
    [TestFixture]
    public class EesWebhookControllerTest
    {
        private EesWebhookController _controller;
        private ILogger<EesWebhookController> _fakeLogger;
        private IEesWebhookService _fakeEesWebhookService;
        private IHttpContextAccessor _fakeHttpContextAccessor;

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<EesWebhookController>>();
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeEesWebhookService = A.Fake<IEesWebhookService>();

            _controller = new EesWebhookController(_fakeHttpContextAccessor, _fakeLogger, _fakeEesWebhookService);
        }

        [Test]
        public void WhenValidHeaderRequestedInNewFilesPublishedOptions_ThenReturnsOkResponse()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Request.Headers.Add("WebHook-Request-Origin", "test.example.com");

            var result = (OkObjectResult)_controller.Options();

            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task WhenPostValidRequest_ThenReceiveSuccessfulResponse()
        {
            object data = "{\"subject\": \"test\"}";
            CloudEvent cloudEvent = new("test", "test", data);

            A.CallTo(() => _fakeEesWebhookService.TryGetCloudEventMessage(A<string>.Ignored)).Returns(cloudEvent);

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var result = (OkObjectResult)await _controller.Post();

            Assert.AreEqual(200, result.StatusCode);
        }
    }
}
