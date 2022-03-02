using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.Controllers;
using UKHO.ExternalNotificationService.API.Services;

namespace UKHO.ExternalNotificationService.API.UnitTests.Controllers
{
    [TestFixture]
    public class WebhookControllerTest
    {
        private WebhookController _controller;
        private ILogger<WebhookController> _fakeLogger;
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private IWebhookService _fakeWebhookService;

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<WebhookController>>();
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeWebhookService = A.Fake<IWebhookService>();

            _controller = new WebhookController(_fakeHttpContextAccessor, _fakeLogger, _fakeWebhookService);
        }

        [Test]
        public void WhenValidHeaderRequestedInNewFilesPublishedOptions_ThenReturnsOkResponse()
        {
            var context = new DefaultHttpContext();
            string requestHeaderValue = "test.example.com";
            context.Request.Headers["WebHook-Request-Origin"] = requestHeaderValue;

            A.CallTo(() => _fakeHttpContextAccessor.HttpContext).Returns(context);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = context
            };

            var result = (StatusCodeResult)_controller.Options();

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual("*", _controller.HttpContext.Response.Headers.Where(a => a.Key == "WebHook-Allowed-Rate").Select(b => b.Value).FirstOrDefault());
            Assert.AreEqual(requestHeaderValue, _controller.HttpContext.Response.Headers.Where(a => a.Key == "WebHook-Allowed-Origin").Select(b => b.Value).FirstOrDefault());
        }

        ////[Test]
        ////public async Task WhenPostValidRequest_ThenReceiveSuccessfulResponse()
        ////{
        ////    MemoryStream requestData = GetEventBodyData();

        ////    _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        ////    _controller.HttpContext.Request.Body = requestData;

        ////    var result = (StatusCodeResult)await _controller.Post();

        ////    Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        ////}

        private static MemoryStream GetEventBodyData()
        {
            var fakeJson = JObject.Parse(@"{""Type"":""uk.gov.UKHO.FileShareService.NewFilesPublished.v1""}");
            fakeJson["Id"] = "25d6c6c1-418b-40f9-bb76-f6dfc0f133bc";

            string jsonString = JsonConvert.SerializeObject(fakeJson);
            var requestData = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            return requestData;
        }
    }
}
