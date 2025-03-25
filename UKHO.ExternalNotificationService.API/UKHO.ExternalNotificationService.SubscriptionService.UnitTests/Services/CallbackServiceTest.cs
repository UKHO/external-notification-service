using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.SubscriptionService.D365Callback;
using UKHO.ExternalNotificationService.SubscriptionService.Helpers;
using UKHO.ExternalNotificationService.SubscriptionService.Services;

namespace UKHO.ExternalNotificationService.Webjob.UnitTests.Services
{
    public class CallbackServiceTest
    {
        private IAuthTokenProvider _fakeAuthTokenProvider;
        private ILogger<CallbackService> _fakeLogger;
        private ICallbackClient _fakeCallbackClient;
        private CallbackService _callbackService;

        private const string FakeExternalEntityPath = "ukho_externalnotifications(1ea01f10-1372-13fb-13a1-1300f3a3faaa)";
        private const string FakeAccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ0ZXN0IHNlcnZlciIsImlhdCI6MTU1ODMyOTg2MCwiZXhwIjoxNTg5OTUyMjYwLCJhdWQiOiJ3d3cudGVzdC5jb20iLCJzdWIiOiJ0ZXN0dXNlckB0ZXN0LmNvbSIsIm9pZCI6IjE0Y2I3N2RjLTFiYTUtNDcxZC1hY2Y1LWEwNDBkMTM4YmFhOSJ9.uOPTbf2Tg6M2OIC6bPHsBAOUuFIuCIzQL_MV3qV6agc";

        [SetUp]
        public void Setup()
        {
            _fakeAuthTokenProvider = A.Fake<IAuthTokenProvider>();
            _fakeLogger = A.Fake<ILogger<CallbackService>>();
            _fakeCallbackClient = A.Fake<ICallbackClient>();

            _callbackService = new CallbackService(_fakeAuthTokenProvider, _fakeLogger, _fakeCallbackClient);
        }

        [Test]
        public async Task WhenInvalidTokenPassedToCallbackToD365UsingDataverse_ThenReturnsUnauthorized()
        {
            A.CallTo(() => _fakeAuthTokenProvider.GetADAccessToken(A<SubscriptionRequestMessage>.Ignored)).Returns(string.Empty);

            var response = await _callbackService.CallbackToD365UsingDataverse(FakeExternalEntityPath, GetExternalNotificationEntity(), GetSubscriptionRequestMessage());
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                Assert.That(response.IsSuccessStatusCode, Is.False);
            });
        }

        [Test]
        public async Task WhenInvalidCallbackToD365UsingDataverseRequest_ThenReturnsBadRequest()
        {
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, RequestMessage = new HttpRequestMessage { Method = HttpMethod.Patch, RequestUri = new Uri("http://test.com") }, Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("Bad Request"))) };
            A.CallTo(() => _fakeAuthTokenProvider.GetADAccessToken(A<SubscriptionRequestMessage>.Ignored)).Returns(FakeAccessToken);
            A.CallTo(() => _fakeCallbackClient.GetCallbackD365Client(A<string>.Ignored, A<string>.Ignored, A<object>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored)).Returns(httpResponse);

            var response = await _callbackService.CallbackToD365UsingDataverse(FakeExternalEntityPath, GetExternalNotificationEntity(), GetSubscriptionRequestMessage());
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(response.IsSuccessStatusCode, Is.False);
            });
        }

        [Test]
        public async Task WhenValidCallbackToD365UsingDataverseRequest_ThenReturnsNoContent()
        {
            var getExternalNotificationEntry = GetExternalNotificationEntity();
            var jsonString = JsonSerializer.Serialize(getExternalNotificationEntry);
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent, RequestMessage = new HttpRequestMessage { Method = HttpMethod.Patch, RequestUri = new Uri("http://test.com") }, Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(jsonString))) };
            A.CallTo(() => _fakeAuthTokenProvider.GetADAccessToken(A<SubscriptionRequestMessage>.Ignored)).Returns(FakeAccessToken);
            A.CallTo(() => _fakeCallbackClient.GetCallbackD365Client(A<string>.Ignored, A<string>.Ignored, A<object>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored)).Returns(httpResponse);

            var response = await _callbackService.CallbackToD365UsingDataverse(FakeExternalEntityPath, getExternalNotificationEntry, GetSubscriptionRequestMessage());
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
                Assert.That(response.IsSuccessStatusCode);
            });
        }

        [Test]
        public async Task WhenInvalidTokenPassedToDeadLetterCallbackToD365UsingDataverse_ThenReturnsUnauthorized()
        {
            A.CallTo(() => _fakeAuthTokenProvider.GetADAccessToken(A<SubscriptionRequestMessage>.Ignored)).Returns(string.Empty);

            var response = await _callbackService.DeadLetterCallbackToD365UsingDataverse(FakeExternalEntityPath, GetExternalNotificationEntity(), GetSubscriptionRequestMessage());
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                Assert.That(response.IsSuccessStatusCode, Is.False);
            });
        }

        [Test]
        public async Task WhenInvalidDeadLetterCallbackToD365UsingDataverseRequest_ThenReturnsBadRequest()
        {
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, RequestMessage = new HttpRequestMessage { Method = HttpMethod.Patch, RequestUri = new Uri("http://test.com") }, Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("Bad Request"))) };
            A.CallTo(() => _fakeAuthTokenProvider.GetADAccessToken(A<SubscriptionRequestMessage>.Ignored)).Returns(FakeAccessToken);
            A.CallTo(() => _fakeCallbackClient.GetCallbackD365Client(A<string>.Ignored, A<string>.Ignored, A<object>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored)).Returns(httpResponse);

            var response = await _callbackService.DeadLetterCallbackToD365UsingDataverse(FakeExternalEntityPath, GetExternalNotificationEntity(), GetSubscriptionRequestMessage());
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(response.IsSuccessStatusCode, Is.False);
            });
        }

        [Test]
        public async Task WhenValidDeadLetterCallbackToD365UsingDataverseRequest_ThenReturnsNoContent()
        {
            var getExternalNotificationEntry = GetExternalNotificationEntity();
            var jsonString = JsonSerializer.Serialize(getExternalNotificationEntry);
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent, RequestMessage = new HttpRequestMessage { Method = HttpMethod.Patch, RequestUri = new Uri("http://test.com") }, Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(jsonString))) };
            A.CallTo(() => _fakeAuthTokenProvider.GetADAccessToken(A<SubscriptionRequestMessage>.Ignored)).Returns(FakeAccessToken);
            A.CallTo(() => _fakeCallbackClient.GetCallbackD365Client(A<string>.Ignored, A<string>.Ignored, A<object>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored)).Returns(httpResponse);

            var response = await _callbackService.DeadLetterCallbackToD365UsingDataverse(FakeExternalEntityPath, getExternalNotificationEntry, GetSubscriptionRequestMessage());
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
                Assert.That(response.IsSuccessStatusCode);
            });
        }

        private static ExternalNotificationEntity GetExternalNotificationEntity()
        {
            return new ExternalNotificationEntity()
            {
                ResponseStatusCode = 123232,
                ResponseDetails = Convert.ToString(DateTime.UtcNow)
            };
        }

        private static SubscriptionRequestMessage GetSubscriptionRequestMessage()
        {
            return new SubscriptionRequestMessage()
            {
                CorrelationId = Guid.NewGuid().ToString(),
                D365CorrelationId = Guid.NewGuid().ToString(),
                IsActive = true,
                NotificationType = "ADDS Data Pipeline",
                NotificationTypeTopicName = "avcs-contentPublished",
                SubscriptionId = Guid.NewGuid().ToString(),
                WebhookUrl = "https://abc.com/"
            };
        }
    }
}
