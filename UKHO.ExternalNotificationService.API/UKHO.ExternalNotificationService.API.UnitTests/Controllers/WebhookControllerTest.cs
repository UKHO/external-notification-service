using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Controllers;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.API.UnitTests.BaseClass;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;

namespace UKHO.ExternalNotificationService.API.UnitTests.Controllers
{
    [TestFixture]
    public class WebhookControllerTest
    {
        private WebhookController _controller;
        private ILogger<WebhookController> _fakeLogger;
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private IEventProcessorFactory _fakeEventProcessorFactory;
        private IEventProcessor _fakeEventProcessor;
        private MemoryStream _fakeFssEventBodyData;

        [SetUp]
        public void Setup()
        {
            string jsonString = JsonSerializer.Serialize(CustomCloudEventBase.GetCustomCloudEvent());
            _fakeFssEventBodyData = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            _fakeLogger = A.Fake<ILogger<WebhookController>>();
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeEventProcessorFactory = A.Fake<IEventProcessorFactory>();
            _fakeEventProcessor = A.Fake<IEventProcessor>();

            _controller = new WebhookController(_fakeHttpContextAccessor, _fakeLogger, _fakeEventProcessorFactory);
        }

        #region Options
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
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(_controller.HttpContext.Response.Headers.Where(a => a.Key == "WebHook-Allowed-Rate").Select(b => b.Value).FirstOrDefault(), Is.EqualTo("*"));
                Assert.That(_controller.HttpContext.Response.Headers.Where(a => a.Key == "WebHook-Allowed-Origin").Select(b => b.Value).FirstOrDefault(), Is.EqualTo(requestHeaderValue));
            });
        }
        #endregion

        #region PostFssEvent
        [Test]
        public async Task WhenPostFssNullEventInRequest_ThenReceiveSuccessfulResponse()
        {
            string jsonString = JsonSerializer.Serialize(new JsonObject());
            var requestData = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Request.Body = requestData;

            var result = (StatusCodeResult)await _controller.Post();

            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task WhenPostFssInvalidEventTypeInRequest_ThenReceiveSuccessfulResponse()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Request.Body = _fakeFssEventBodyData;

            A.CallTo(() => _fakeEventProcessorFactory.GetProcessor(A<string>.Ignored)).Returns(null);

            var result = (StatusCodeResult)await _controller.Post();

            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task WhenPostFssInvalidEventPayloadInRequest_ThenReceiveSuccessfulResponse()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Request.Body = _fakeFssEventBodyData;

            A.CallTo(() => _fakeEventProcessorFactory.GetProcessor(A<string>.Ignored)).Returns(_fakeEventProcessor);
            A.CallTo(() => _fakeEventProcessor.Process(A<CustomCloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                           .Returns(new ExternalNotificationServiceProcessResponse()
                           {
                               Errors = new List<Error>() { new Error() { Description = "test", Source = "test" } },
                               StatusCode = HttpStatusCode.OK
                           });

            var result = (OkObjectResult)await _controller.Post();

            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task WhenPostFssValidEventRequest_ThenReceiveSuccessfulResponse()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Request.Body = _fakeFssEventBodyData;

            A.CallTo(() => _fakeEventProcessorFactory.GetProcessor(A<string>.Ignored)).Returns(_fakeEventProcessor);
            A.CallTo(() => _fakeEventProcessor.Process(A<CustomCloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                            .Returns(new ExternalNotificationServiceProcessResponse() { StatusCode = HttpStatusCode.OK });

            var result = (StatusCodeResult)await _controller.Post();

            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }
        #endregion
    }
}
