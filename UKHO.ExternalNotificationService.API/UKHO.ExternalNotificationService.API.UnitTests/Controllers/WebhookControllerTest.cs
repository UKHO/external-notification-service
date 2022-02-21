using System.IO;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Controllers;

namespace UKHO.ExternalNotificationService.API.UnitTests.Controllers
{
    [TestFixture]
    public class WebhookControllerTest
    {
        private WebhookController _controller;
        private ILogger<WebhookController> _fakeLogger;
        private IHttpContextAccessor _fakeHttpContextAccessor;

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<WebhookController>>();
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();

            _controller = new WebhookController(_fakeHttpContextAccessor, _fakeLogger);
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
            MemoryStream requestData = GetEventBodyData();

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Request.Body = requestData;

            var result = (OkObjectResult)await _controller.Post();

            Assert.AreEqual(200, result.StatusCode);
        }

        private static MemoryStream GetEventBodyData()
        {
            var fakeJson = JObject.Parse(@"{""Type"":""FilesPublished""}");
            fakeJson["Id"] = "25d6c6c1-418b-40f9-bb76-f6dfc0f133bc";

            string jsonString = JsonConvert.SerializeObject(fakeJson);
            var requestData = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            return requestData;
        }
    }
}
