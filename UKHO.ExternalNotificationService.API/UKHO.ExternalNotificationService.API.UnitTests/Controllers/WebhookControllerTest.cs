using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.Controllers;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;
using Attribute = UKHO.ExternalNotificationService.Common.Models.EventModel.Attribute;
using File = UKHO.ExternalNotificationService.Common.Models.EventModel.File;

namespace UKHO.ExternalNotificationService.API.UnitTests.Controllers
{
    [TestFixture]
    public class WebhookControllerTest
    {
        private WebhookController _controller;
        private ILogger<WebhookController> _fakeLogger;
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private IWebhookService _fakeWebhookService;
        private IEventProcessor _fakeEventProcessor;
        private MemoryStream _fakeFssEventBodyData;

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<WebhookController>>();
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeWebhookService = A.Fake<IWebhookService>();
            _fakeEventProcessor = A.Fake<IEventProcessor>();
            _fakeFssEventBodyData = GetFssEventBodyData();

            _controller = new WebhookController(_fakeHttpContextAccessor, _fakeLogger, _fakeWebhookService);
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
            MemoryStream requestData = new();

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

            A.CallTo(() => _fakeWebhookService.GetProcessor(A<string>.Ignored)).Returns(null);

            var result = (StatusCodeResult)await _controller.Post();

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }

        [Test]
        public async Task WhenPostFssInvalidEventPayloadInRequest_ThenReceiveSuccessfulResponse()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Request.Body = _fakeFssEventBodyData;

            A.CallTo(() => _fakeWebhookService.GetProcessor(A<string>.Ignored)).Returns(_fakeEventProcessor);
            A.CallTo(() => _fakeEventProcessor.Process(A<CustomEventGridEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
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

            A.CallTo(() => _fakeWebhookService.GetProcessor(A<string>.Ignored)).Returns(_fakeEventProcessor);
            A.CallTo(() => _fakeEventProcessor.Process(A<CustomEventGridEvent>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                            .Returns( new ExternalNotificationServiceProcessResponse() {StatusCode = HttpStatusCode.OK });

            var result = (StatusCodeResult)await _controller.Post();

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }
        #endregion

        private static MemoryStream GetFssEventBodyData()
        {
            var ensWebhookJson = JObject.Parse(@"{""Type"":""uk.gov.UKHO.FileShareService.NewFilesPublished.v1""}");
            ensWebhookJson["Source"] = "https://files.admiralty.co.uk";
            ensWebhookJson["Id"] = "49c67cca-9cca-4655-a38e-583693af55ea";
            ensWebhookJson["Subject"] = "83d08093-7a67-4b3a-b431-92ba42feaea0";
            ensWebhookJson["DataContentType"] = "application/json";
            ensWebhookJson["Data"] = JObject.FromObject(GetFssEventData());

            string jsonString = JsonConvert.SerializeObject(ensWebhookJson);
            var requestData = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            return requestData;
        }

        private static FssEventData GetFssEventData()
        {
            Link linkBatchDetails = new()
            {
                Href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0"
            };
            Link linkBatchStatus = new()
            {
                Href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/status"
            };

            FileLinks fileLinks = new()
            {
               Get  = new Link() { Href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/AVCS_S631-1_Update_Wk45_21_Only.zip" },
            };

            BatchLinks links = new()
            {
                BatchDetails = linkBatchDetails,
                BatchStatus = linkBatchStatus
            };

            return new FssEventData() { Links = links,
                                        BusinessUnit = "AVCSData",
                                        Attributes = new List<Attribute> {},
                                        BatchId = "83d08093-7a67-4b3a-b431-92ba42feaea0",
                                        BatchPublishedDate = DateTime.UtcNow,
                                        Files = new File[] {new() { MIMEType= "application/zip",
                                                                    FileName= "AVCS_S631-1_Update_Wk45_21_Only.zip",
                                                                    FileSize=99073923,
                                                                    Hash="yNpJTWFKhD3iasV8B/ePKw==",
                                                                    Attributes=new List<Attribute> {},
                                                                    Links = fileLinks   }}};
        }
    }
}
