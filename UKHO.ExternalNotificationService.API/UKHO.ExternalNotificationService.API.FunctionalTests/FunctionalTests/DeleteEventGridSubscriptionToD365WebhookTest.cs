using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.FunctionalTests.Helper;
using UKHO.ExternalNotificationService.API.FunctionalTests.Model;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.FunctionalTests
{
    class DeleteEventGridSubscriptionToD365WebhookTest
    {
        public object JsonSerialize { get; private set; }
        private EnsApiClient EnsApiClient { get; set; }
        private TestConfiguration TestConfig { get; set; }
        private D365Payload D365Payload { get; set; }
        private string EnsToken { get; set; }

        [SetUp]
        public async Task SetupAsync()
        {
            TestConfig = new TestConfiguration();
            EnsApiClient = new EnsApiClient(TestConfig.EnsApiBaseUrl);

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), TestConfig.PayloadFolder, TestConfig.FssAvcsPayloadFileName);
            D365Payload = JsonSerializer.Deserialize<D365Payload>(await File.ReadAllTextAsync(filePath));
            D365Payload.InputParameters[0].Value.Attributes[9].Value = string.Concat(TestConfig.StubBaseUri, TestConfig.WebhookUrlExtension);

            ADAuthTokenProvider adAuthTokenProvider = new();
            EnsToken = await adAuthTokenProvider.GetEnsAuthToken();

        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithStatusInActive_ThenSubscriptionHasBeenDeleted()
        {
            // Set the new subscriptionId for D365 payload
            string subscriptionId = Guid.NewGuid().ToString();
            D365Payload.PostEntityImages[0].Value.Attributes[0].Value = subscriptionId;
            D365Payload.InputParameters[0].Value.Attributes[10].Value = subscriptionId;
            DateTime requestTime = DateTime.UtcNow;
            //Clear return status
            HttpResponseMessage apiStubResponse = await EnsApiClient.PostStubCommandToFailAsync(TestConfig.StubBaseUri, subscriptionId, null);
            Assert.That(200, Is.EqualTo((int)apiStubResponse.StatusCode), $"Incorrect status code {apiStubResponse.StatusCode}  is  returned, instead of the expected 200.");

            await Task.Delay(30000);
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.That(202, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            while (DateTime.UtcNow - requestTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(1000);
            }

            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.StubBaseUri, subscriptionId);
            Assert.That(200, Is.EqualTo((int)callBackResponse.StatusCode), $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            IEnumerable<EnsCallbackResponseModel> callBackResponseBody = JsonSerializer.Deserialize<IEnumerable<EnsCallbackResponseModel>>(callBackResponse.Content.ReadAsStringAsync().Result);


            EnsCallbackResponseModel callBackResponseLatest = callBackResponseBody.Where(x => x.TimeStamp >= requestTime).OrderByDescending(a => a.TimeStamp).FirstOrDefault();

            Assert.That(subscriptionId, Is.EqualTo(callBackResponseLatest.SubscriptionId), $"Invalid subscriptionId {callBackResponseLatest.SubscriptionId} , Instead of expected subscriptionId {subscriptionId}.");
            Assert.That(TestConfig.SucceededStatusCode, Is.EqualTo(callBackResponseLatest.CallBackRequest.Ukho_lastresponse), $"Invalid last response {callBackResponseLatest.CallBackRequest.Ukho_lastresponse} , Instead of expected last response {TestConfig.SucceededStatusCode}.");
            Assert.That(callBackResponseLatest.CallBackRequest.Ukho_responsedetails.Contains("Successfully added subscription"), $"Last Response : {callBackResponseLatest.CallBackRequest.Ukho_responsedetails}");
            Assert.That(callBackResponseLatest.TimeStamp <= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second), $"Response body returned timestamp date {callBackResponseLatest.TimeStamp} , greater than the expected value.");

            //set the subscription state as InActive
            D365Payload.InputParameters[0].Value.FormattedValues[0].Value = "Inactive";
            requestTime = DateTime.UtcNow;

            await Task.Delay(30000);
            apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.That(202, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            while (DateTime.UtcNow - requestTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(1000);
            }

            callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.StubBaseUri, subscriptionId);
            Assert.That(200, Is.EqualTo((int)callBackResponse.StatusCode), $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            callBackResponseBody = JsonSerializer.Deserialize<IEnumerable<EnsCallbackResponseModel>>(callBackResponse.Content.ReadAsStringAsync().Result);


            callBackResponseLatest = callBackResponseBody.Where(x => x.TimeStamp >= requestTime).OrderByDescending(a => a.TimeStamp).FirstOrDefault();

            Assert.That(subscriptionId, Is.EqualTo(callBackResponseLatest.SubscriptionId), $"Invalid subscriptionId {callBackResponseLatest.SubscriptionId} , Instead of expected subscriptionId {subscriptionId}.");
            Assert.That(TestConfig.SucceededStatusCode, Is.EqualTo(callBackResponseLatest.CallBackRequest.Ukho_lastresponse), $"Invalid last response {callBackResponseLatest.CallBackRequest.Ukho_lastresponse} , Instead of expected last response {TestConfig.SucceededStatusCode}.");
            Assert.That(callBackResponseLatest.CallBackRequest.Ukho_responsedetails.Contains("Successfully removed subscription"), $"Last Response : {callBackResponseLatest.CallBackRequest.Ukho_responsedetails}");
            Assert.That(callBackResponseLatest.TimeStamp <= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second), $"Response body returned timestamp date {callBackResponseLatest.TimeStamp} , greater than the expected value.");

            //set the subscription state as Active

            D365Payload.InputParameters[0].Value.FormattedValues[0].Value = "Active";
            requestTime = DateTime.UtcNow;

            await Task.Delay(30000);
            apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.That(202, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            while (DateTime.UtcNow - requestTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(1000);
            }

            callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.StubBaseUri, subscriptionId);
            Assert.That(200, Is.EqualTo((int)callBackResponse.StatusCode), $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            callBackResponseBody = JsonSerializer.Deserialize<IEnumerable<EnsCallbackResponseModel>>(callBackResponse.Content.ReadAsStringAsync().Result);


            callBackResponseLatest = callBackResponseBody.Where(x => x.TimeStamp >= requestTime).OrderByDescending(a => a.TimeStamp).FirstOrDefault();

            Assert.That(subscriptionId, Is.EqualTo(callBackResponseLatest.SubscriptionId), $"Invalid subscriptionId {callBackResponseLatest.SubscriptionId} , Instead of expected subscriptionId {subscriptionId}.");
            Assert.That(TestConfig.SucceededStatusCode, Is.EqualTo(callBackResponseLatest.CallBackRequest.Ukho_lastresponse), $"Invalid last response {callBackResponseLatest.CallBackRequest.Ukho_lastresponse} , Instead of expected last response {TestConfig.SucceededStatusCode}.");
            Assert.That(callBackResponseLatest.CallBackRequest.Ukho_responsedetails.Contains("Successfully added subscription"), $"Last Response : {callBackResponseLatest.CallBackRequest.Ukho_responsedetails}");
            Assert.That(callBackResponseLatest.TimeStamp <= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second), $"Response body returned timestamp date {callBackResponseLatest.TimeStamp} , greater than the expected value.");


        }

    }
}
