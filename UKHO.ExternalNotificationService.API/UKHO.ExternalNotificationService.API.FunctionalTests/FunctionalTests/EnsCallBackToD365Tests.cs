using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), TestConfig.PayloadFolder, TestConfig.PayloadFileName);
            D365Payload = JsonConvert.DeserializeObject<D365Payload>(await File.ReadAllTextAsync(filePath));

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
            Assert.AreEqual(200, (int)apiStubResponse.StatusCode, $"Incorrect status code {apiStubResponse.StatusCode}  is  returned, instead of the expected 200.");

            DateTime requestTime = DateTime.UtcNow;
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            while (DateTime.UtcNow - requestTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(10000);
            }

            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.StubBaseUri, subscriptionId);
            Assert.AreEqual(200, (int)callBackResponse.StatusCode, $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            IEnumerable<EnsCallbackResponseModel> callBackResponseBody =JsonConvert.DeserializeObject<IEnumerable<EnsCallbackResponseModel>>(callBackResponse.Content.ReadAsStringAsync().Result);

            EnsCallbackResponseModel callBackResponseLatest = callBackResponseBody.Where(x => x.TimeStamp >= requestTime).OrderByDescending(a => a.TimeStamp).FirstOrDefault();

            Assert.AreEqual(subscriptionId, callBackResponseLatest.SubscriptionId, $"Invalid subscriptionId {callBackResponseLatest.SubscriptionId} , Instead of expected subscriptionId {subscriptionId}.");
            Assert.AreEqual(TestConfig.SucceededStatusCode, callBackResponseLatest.CallBackRequest.Ukho_lastresponse, $"Invalid last response {callBackResponseLatest.CallBackRequest.Ukho_lastresponse} , Instead of expected last response {TestConfig.SucceededStatusCode}.");
            Assert.IsTrue(callBackResponseLatest.CallBackRequest.Ukho_responsedetails.Contains("Successfully added subscription"), $"Last Response : {callBackResponseLatest.CallBackRequest.Ukho_responsedetails}");
            Assert.IsTrue(callBackResponseLatest.TimeStamp <= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second), $"Response body returned timestamp date {callBackResponseLatest.TimeStamp} , greater than the expected value.");
            
        }
        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAInValidSubscriptionId_ThenNotFoundStatusResponseIsReturned()
        {

            string subscriptionId = D365Payload.PostEntityImages[0].Value.Attributes[0].Value.ToString();

            string newSubscriptionId = subscriptionId.Remove(subscriptionId.Length - 2).Insert(subscriptionId.Length - 2, "AA");

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.StubBaseUri, newSubscriptionId);
            Assert.AreEqual(200, (int)callBackResponse.StatusCode, $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

        
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAnInvalidWebhookUrl_ThenSucessStatusResponseIsReturnedWithFailedResponseDetails()
        {
             D365Payload.InputParameters[0].Value.Attributes[9].Value = D365Payload.InputParameters[0].Value.Attributes[9].Value + "Failed";
           // Get the subscriptionId from D365 payload for first subcription id
            string subscriptionId = D365Payload.PostEntityImages[0].Value.Attributes[0].Value.ToString();

            //Clear return status
            HttpResponseMessage apiStubResponse = await EnsApiClient.PostStubCommandToFailAsync(TestConfig.StubBaseUri, subscriptionId, null);
            Assert.AreEqual(200, (int)apiStubResponse.StatusCode, $"Incorrect status code {apiStubResponse.StatusCode}  is  returned, instead of the expected 200.");

            DateTime requestTime = DateTime.UtcNow;
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            while (DateTime.UtcNow - requestTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(10000);
            }

            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.StubBaseUri, subscriptionId);
            Assert.AreEqual(200, (int)callBackResponse.StatusCode, $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            IEnumerable<EnsCallbackResponseModel> callBackResponseBody = JsonConvert.DeserializeObject<IEnumerable<EnsCallbackResponseModel>>(callBackResponse.Content.ReadAsStringAsync().Result);

            EnsCallbackResponseModel callBackResponseLatest = callBackResponseBody.Where(x => x.TimeStamp >= requestTime).OrderByDescending(a => a.TimeStamp).FirstOrDefault();

            Assert.AreEqual(subscriptionId, callBackResponseLatest.SubscriptionId, $"Invalid subscriptionId {callBackResponseLatest.SubscriptionId} , Instead of expected subscriptionId {subscriptionId}.");
            Assert.IsTrue(callBackResponseLatest.CallBackRequest.Ukho_responsedetails.Contains("Failed to add subscription"),$"Last Response : {callBackResponseLatest.CallBackRequest.Ukho_responsedetails}, Time : {callBackResponseLatest.TimeStamp}");
            Assert.AreEqual(TestConfig.FailedStatusCode, callBackResponseLatest.CallBackRequest.Ukho_lastresponse, $"Invalid last response {callBackResponseLatest.CallBackRequest.Ukho_lastresponse} , Instead of expected last response {TestConfig.FailedStatusCode}.");
            Assert.IsTrue(callBackResponseLatest.TimeStamp <= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second), $"Response body returned timestamp date {callBackResponseLatest.TimeStamp} , greater than the expected value.");
        }

        
        [TestCase(404, TestName = "CallBack Stub Returns StausCode As Not Found")]
        [TestCase(400, TestName = "CallBack Stub Returns StausCode As Bad Request")]
        public async Task WhenICallTheCallBackStubUrlToFailWithValidSubscriptionId_ThenValidResponseIsReturned(int statusCode)
        {
            //Get the subscriptionId from D365 payload

            string subscriptionId = D365Payload.PostEntityImages[0].Value.Attributes[0].Value.ToString();            
            
            HttpResponseMessage apiStubResponse = await EnsApiClient.PostStubCommandToFailAsync(TestConfig.StubBaseUri, subscriptionId, statusCode);
            Assert.AreEqual(200, (int)apiStubResponse.StatusCode, $"Incorrect status code {apiStubResponse.StatusCode}  is  returned, instead of the expected 200.");

            DateTime requestTime = DateTime.UtcNow;
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");
            while (DateTime.UtcNow - requestTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(10000);
            }

            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.StubBaseUri, subscriptionId);
            Assert.AreEqual(200, (int)callBackResponse.StatusCode, $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            IEnumerable<EnsCallbackResponseModel> callBackResponseBody = JsonConvert.DeserializeObject<IEnumerable<EnsCallbackResponseModel>>(callBackResponse.Content.ReadAsStringAsync().Result);

            EnsCallbackResponseModel callBackResponseLatest = callBackResponseBody.Where(x => x.TimeStamp >= requestTime).OrderByDescending(a => a.TimeStamp).FirstOrDefault();

            Assert.AreEqual(subscriptionId, callBackResponseLatest.SubscriptionId, $"Invalid subscriptionId {callBackResponseLatest.SubscriptionId} , Instead of expected subscriptionId {subscriptionId}.");
            Assert.AreEqual(statusCode, callBackResponseLatest.HttpStatusCode, $"Invalid statusCode {callBackResponseLatest.HttpStatusCode} , Instead of expected statusCode {statusCode}.");
            Assert.IsTrue(callBackResponseLatest.TimeStamp <= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second), $"Response body returned timestamp date {callBackResponseLatest.TimeStamp} , greater than the expected value.");


        }
        [TestCase(500, TestName = "CallBack Stub Returns StausCode As Internal Server Error")]
        [TestCase(503, TestName = "CallBack Stub Returns StausCode As Service UnAvailable")]
        public async Task WhenICallTheCallBackStubUrlToFailWithValidSubscriptionId_ThenValidResponseIsReturnedWithRetryCount(int statusCode)
        {
            //Get the subscriptionId from D365 payload

            string subscriptionId = D365Payload.PostEntityImages[0].Value.Attributes[0].Value.ToString();          
                       
            HttpResponseMessage apiStubResponse = await EnsApiClient.PostStubCommandToFailAsync(TestConfig.StubBaseUri, subscriptionId, statusCode);
            Assert.AreEqual(200, (int)apiStubResponse.StatusCode, $"Incorrect status code {apiStubResponse.StatusCode}  is  returned, instead of the expected 200.");

            DateTime requestTime = DateTime.UtcNow;
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            while (DateTime.UtcNow - requestTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(10000);
            }

            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.StubBaseUri, subscriptionId);
            Assert.AreEqual(200, (int)callBackResponse.StatusCode, $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            IEnumerable<EnsCallbackResponseModel> callBackResponseBody = JsonConvert.DeserializeObject<IEnumerable<EnsCallbackResponseModel>>(callBackResponse.Content.ReadAsStringAsync().Result);

            IEnumerable<EnsCallbackResponseModel> callBackResponseFailure = callBackResponseBody.Where(x => x.TimeStamp >= requestTime).OrderByDescending(a => a.TimeStamp);

            //Verify Retry Count
            Assert.Greater(callBackResponseFailure.Count(), 1);

            EnsCallbackResponseModel callBackResponseLatest = callBackResponseBody.Where(x => x.TimeStamp >= requestTime).OrderByDescending(a => a.TimeStamp).FirstOrDefault();

            Assert.AreEqual(subscriptionId, callBackResponseLatest.SubscriptionId, $"Invalid subscriptionId {callBackResponseLatest.SubscriptionId} , Instead of expected subscriptionId {subscriptionId}.");
            Assert.AreEqual(statusCode, callBackResponseLatest.HttpStatusCode, $"Invalid statusCode {callBackResponseLatest.HttpStatusCode} , Instead of expected statusCode {statusCode}.");
            Assert.IsTrue(callBackResponseLatest.TimeStamp <= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second), $"Response body returned timestamp date {callBackResponseLatest.TimeStamp} , greater than the expected value.");
                        
        }       
    }
}
