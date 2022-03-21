using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            string jsonString = JsonConvert.SerializeObject(CustomCloudEventBase.GetCustomCloudEvent());
            MemoryStream FssEventBodyData = new(Encoding.UTF8.GetBytes(jsonString));
            _fakeFssEventBodyData = FssEventBodyData;

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

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual("*", _controller.HttpContext.Response.Headers.Where(a => a.Key == "WebHook-Allowed-Rate").Select(b => b.Value).FirstOrDefault());
            Assert.AreEqual(requestHeaderValue, _controller.HttpContext.Response.Headers.Where(a => a.Key == "WebHook-Allowed-Origin").Select(b => b.Value).FirstOrDefault());
        }
        #endregion

        #region PostFssEvent
        [Test]
        public async Task WhenPostFssNullEventInRequest_ThenReceiveSuccessfulResponse()
        {
            string jsonString = JsonConvert.SerializeObject(new JObject());
            var requestData = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Request.Body = requestData;

            var result = (StatusCodeResult)await _controller.Post();

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }

        [Test]
        public async Task WhenPostFssInvalidEventTypeInRequest_ThenReceiveSuccessfulResponse()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Request.Body = _fakeFssEventBodyData;

            A.CallTo(() => _fakeEventProcessorFactory.GetProcessor(A<string>.Ignored)).Returns(null);

            var result = (StatusCodeResult)await _controller.Post();

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }

        [Test]
        public async Task WhenPostFssInvalidEventPayloadInRequest_ThenReceiveSuccessfulResponse()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Request.Body = _fakeFssEventBodyData;

            A.CallTo(() => _fakeEventProcessorFactory.GetProcessor(A<string>.Ignored)).Returns(_fakeEventProcessor);
            A.CallTo(() => _fakeEventProcessor.Process(A<CustomCloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                           .Returns(new ExternalNotificationServiceProcessResponse() { Errors = new List<Error>() { new Error() {Description ="test", Source="test" } },
                                                                                       StatusCode = HttpStatusCode.OK });

            var result = (OkObjectResult)await _controller.Post();

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }

        [Test]
        public async Task WhenPostFssValidEventRequest_ThenReceiveSuccessfulResponse()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Request.Body = _fakeFssEventBodyData;

            A.CallTo(() => _fakeEventProcessorFactory.GetProcessor(A<string>.Ignored)).Returns(_fakeEventProcessor);
            A.CallTo(() => _fakeEventProcessor.Process(A<CustomCloudEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                            .Returns( new ExternalNotificationServiceProcessResponse() {StatusCode = HttpStatusCode.OK });

            var result = (StatusCodeResult)await _controller.Post();

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }
        #endregion
    }
}
