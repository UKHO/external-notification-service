using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.FunctionalTests.Helper;
using UKHO.ExternalNotificationService.API.FunctionalTests.Model;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.FunctionalTests
{
    class EnsCallBackToD365Tests
    {
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
            D365Payload = JsonConvert.DeserializeObject<D365Payload>(await File.ReadAllTextAsync(filePath));
            D365Payload.InputParameters[0].Value.Attributes[9].Value = string.Concat(TestConfig.StubBaseUri, TestConfig.WebhookUrlExtension);

            ADAuthTokenProvider adAuthTokenProvider = new();
            EnsToken = await adAuthTokenProvider.GetEnsAuthToken();

        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidSubscriptionId_ThenSuccessStatusResponseIsReturned()
        {
            //Get the subscriptionId from D365 payload            
            string subscriptionId = D365Payload.PostEntityImages[0].Value.Attributes[0].Value.ToString();

            //Clear return status
            HttpResponseMessage apiStubResponse = await EnsApiClient.PostStubCommandToFailAsync(TestConfig.StubBaseUri, subscriptionId,null);
            Assert.That(200, Is.EqualTo((int)apiStubResponse.StatusCode), $"Incorrect status code {apiStubResponse.StatusCode}  is  returned, instead of the expected 200.");

            DateTime requestTime = DateTime.UtcNow;
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.That(202, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            while (DateTime.UtcNow - requestTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(1000);
            }

            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.StubBaseUri, subscriptionId);
            Assert.That(200, Is.EqualTo((int)callBackResponse.StatusCode), $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            IEnumerable<EnsCallbackResponseModel> callBackResponseBody =JsonConvert.DeserializeObject<IEnumerable<EnsCallbackResponseModel>>(callBackResponse.Content.ReadAsStringAsync().Result);

            EnsCallbackResponseModel callBackResponseLatest = callBackResponseBody.Where(x => x.TimeStamp >= requestTime).OrderByDescending(a => a.TimeStamp).FirstOrDefault();

            Assert.That(subscriptionId, Is.EqualTo(callBackResponseLatest.SubscriptionId), $"Invalid subscriptionId {callBackResponseLatest.SubscriptionId} , Instead of expected subscriptionId {subscriptionId}.");
            Assert.That(TestConfig.SucceededStatusCode, Is.EqualTo(callBackResponseLatest.CallBackRequest.Ukho_lastresponse), $"Invalid last response {callBackResponseLatest.CallBackRequest.Ukho_lastresponse} , Instead of expected last response {TestConfig.SucceededStatusCode}.");
            Assert.That(callBackResponseLatest.CallBackRequest.Ukho_responsedetails.Contains("Successfully added subscription"), $"Last Response : {callBackResponseLatest.CallBackRequest.Ukho_responsedetails}");            
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAInValidSubscriptionId_ThenNotFoundStatusResponseIsReturned()
        {

            string subscriptionId = D365Payload.PostEntityImages[0].Value.Attributes[0].Value.ToString();

            string newSubscriptionId = subscriptionId.Remove(subscriptionId.Length - 2).Insert(subscriptionId.Length - 2, "AA");

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.That(202, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.StubBaseUri, newSubscriptionId);
            Assert.That(404, Is.EqualTo((int)callBackResponse.StatusCode), $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAnInvalidWebhookUrl_ThenSuccessStatusResponseIsReturnedWithFailedResponseDetails()
        {
             D365Payload.InputParameters[0].Value.Attributes[9].Value = D365Payload.InputParameters[0].Value.Attributes[9].Value + "Failed";
            // Get the new subscriptionId for D365 payload
            string subscriptionId = Guid.NewGuid().ToString();
            D365Payload.PostEntityImages[0].Value.Attributes[0].Value = subscriptionId;            
            D365Payload.InputParameters[0].Value.Attributes[10].Value = subscriptionId;
            DateTime requestTime = DateTime.UtcNow;           
            
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.That(202, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            while (DateTime.UtcNow - requestTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(1000);
            }

            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.StubBaseUri, subscriptionId);
            Assert.That(200, Is.EqualTo((int)callBackResponse.StatusCode), $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            IEnumerable<EnsCallbackResponseModel> callBackResponseBody = JsonConvert.DeserializeObject<IEnumerable<EnsCallbackResponseModel>>(callBackResponse.Content.ReadAsStringAsync().Result);


            EnsCallbackResponseModel callBackResponseLatest = callBackResponseBody.Where(x => x.TimeStamp >= requestTime).OrderByDescending(a => a.TimeStamp).FirstOrDefault();

            Assert.That(subscriptionId, Is.EqualTo(callBackResponseLatest.SubscriptionId), $"Invalid subscriptionId {callBackResponseLatest.SubscriptionId} , Instead of expected subscriptionId {subscriptionId}.");
            Assert.That(callBackResponseLatest.CallBackRequest.Ukho_responsedetails.Contains("Failed to add subscription"),$"Last Response : {callBackResponseLatest.CallBackRequest.Ukho_responsedetails}, Time : {callBackResponseLatest.TimeStamp}");
            Assert.That(TestConfig.FailedStatusCode, Is.EqualTo(callBackResponseLatest.CallBackRequest.Ukho_lastresponse), $"Invalid last response {callBackResponseLatest.CallBackRequest.Ukho_lastresponse} , Instead of expected last response {TestConfig.FailedStatusCode}.");            
        }
        
        [TestCase(400, TestName = "CallBack Stub Returns StatusCode As Bad Request")]       
        public async Task WhenICallTheCallBackStubUrlToFailWithValidSubscriptionId_ThenValidResponseIsReturned(int statusCode)
        {
            //Get the subscriptionId from D365 payload            
            string subscriptionId = D365Payload.PostEntityImages[0].Value.Attributes[0].Value.ToString();

            HttpResponseMessage apiStubResponse = await EnsApiClient.PostStubCommandToFailAsync(TestConfig.StubBaseUri, subscriptionId, statusCode);
            Assert.That(200, Is.EqualTo((int)apiStubResponse.StatusCode), $"Incorrect status code {apiStubResponse.StatusCode}  is  returned, instead of the expected 200.");

            DateTime requestTime = DateTime.UtcNow;
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);            
            Assert.That(202, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");
            while (DateTime.UtcNow - requestTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(1000);
            }
            
            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.StubBaseUri, subscriptionId);
            Assert.That(200, Is.EqualTo((int)callBackResponse.StatusCode), $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            IEnumerable<EnsCallbackResponseModel> callBackResponseBody = JsonConvert.DeserializeObject<IEnumerable<EnsCallbackResponseModel>>(callBackResponse.Content.ReadAsStringAsync().Result);

            EnsCallbackResponseModel callBackResponseLatest = callBackResponseBody.Where(x => x.TimeStamp >= requestTime).OrderByDescending(a => a.TimeStamp).FirstOrDefault();

            Assert.That(subscriptionId, Is.EqualTo(callBackResponseLatest.SubscriptionId), $"Invalid subscriptionId {callBackResponseLatest.SubscriptionId} , Instead of expected subscriptionId {subscriptionId}.");
            Assert.That(statusCode, Is.EqualTo(callBackResponseLatest.HttpStatusCode), $"Invalid statusCode {callBackResponseLatest.HttpStatusCode} , Instead of expected statusCode {statusCode}.");            
        }
        
        [TestCase(500, TestName = "CallBack Stub Returns StatusCode As Internal Server Error")]
        public async Task WhenICallTheCallBackStubUrlToFailWithValidSubscriptionId_ThenValidResponseIsReturnedWithRetryCount(int statusCode)
        {
            //Get the subscriptionId from D365 payload            
            string subscriptionId = D365Payload.PostEntityImages[0].Value.Attributes[0].Value.ToString();

            HttpResponseMessage apiStubResponse = await EnsApiClient.PostStubCommandToFailAsync(TestConfig.StubBaseUri, subscriptionId, statusCode);
            Assert.That(200, Is.EqualTo((int)apiStubResponse.StatusCode), $"Incorrect status code {apiStubResponse.StatusCode}  is  returned, instead of the expected 200.");

            DateTime requestTime = DateTime.UtcNow;
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);            
            Assert.That(202, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            while (DateTime.UtcNow - requestTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(1000);
            }

            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.StubBaseUri, subscriptionId);
            Assert.That(200, Is.EqualTo((int)callBackResponse.StatusCode), $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            IEnumerable<EnsCallbackResponseModel> callBackResponseBody = JsonConvert.DeserializeObject<IEnumerable<EnsCallbackResponseModel>>(callBackResponse.Content.ReadAsStringAsync().Result);

            IEnumerable<EnsCallbackResponseModel> callBackResponseFailure = callBackResponseBody.Where(x => x.TimeStamp >= requestTime).OrderByDescending(a => a.TimeStamp);

            //Verify Retry Count
            Assert.That(callBackResponseFailure.Count() > 1);

            EnsCallbackResponseModel callBackResponseLatest = callBackResponseBody.Where(x => x.TimeStamp >= requestTime).OrderByDescending(a => a.TimeStamp).FirstOrDefault();

            Assert.That(subscriptionId, Is.EqualTo(callBackResponseLatest.SubscriptionId), $"Invalid subscriptionId {callBackResponseLatest.SubscriptionId} , Instead of expected subscriptionId {subscriptionId}.");
            Assert.That(statusCode, Is.EqualTo(callBackResponseLatest.HttpStatusCode), $"Invalid statusCode {callBackResponseLatest.HttpStatusCode} , Instead of expected statusCode {statusCode}.");            
                        
        }       
    }
}
