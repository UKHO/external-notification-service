using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
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
        public async Task WhenICallTheEnsSubscriptionApiWithAValidSubscriptionId_ThenSuccessStatusResponseIsRetruns()
        {
            //Get the subscriptionId from D365 payload

            string subscriptionId = D365Payload.PostEntityImages[0].Value.Attributes[0].Value.ToString();

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.D365ApiUri, subscriptionId);
            Assert.AreEqual(200, (int)callBackResponse.StatusCode, $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            EnsCallbackResponseDetailsModel callBackResponseBody = await callBackResponse.ReadAsTypeAsync<EnsCallbackResponseDetailsModel>();

            foreach (EnsCallbackResponseModel entry in callBackResponseBody.CallBackRequests)
            {
                Assert.AreEqual(subscriptionId, entry.subscriptionId, $"Invalid subscriptionId {entry.subscriptionId} , Instead of expected subscriptionId {subscriptionId}.");
                Assert.AreEqual(TestConfig.SucceededStatusCode, entry.callBackRequest.ukho_lastresponse, $"Invalid last response {TestConfig.SucceededStatusCode} , Instead of expected last response {entry.callBackRequest.ukho_lastresponse}.");
            }

        }
        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAInValidSubscriptionId_ThenFailedStatusResponseIsRetruns()
        {

            //Get the subscriptionId from D365 payload 

            string subscriptionId = D365Payload.PostEntityImages[0].Value.Attributes[0].Value.ToString();

            string newSubscriptionId = subscriptionId.Remove(subscriptionId.Length - 2).Insert(subscriptionId.Length - 2, "AA");

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.D365ApiUri, newSubscriptionId);
            Assert.AreEqual(404, (int)callBackResponse.StatusCode, $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            EnsCallbackResponseDetailsModel callBackResponseBody = await callBackResponse.ReadAsTypeAsync<EnsCallbackResponseDetailsModel>();

            foreach (EnsCallbackResponseModel entry in callBackResponseBody.CallBackRequests)
            {
                Assert.AreEqual(newSubscriptionId, entry.subscriptionId, $"Invalid subscriptionId {entry.subscriptionId} , Instead of expected subscriptionId {newSubscriptionId}.");
                Assert.AreEqual(TestConfig.SucceededStatusCode, entry.callBackRequest.ukho_lastresponse, $"Invalid last response {TestConfig.SucceededStatusCode} , Instead of expected last response {entry.callBackRequest.ukho_lastresponse}.");
            }

        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAMultipleSubscriptionId_ThenSucessStatusResponseIsRetruns()
        {
          
           // Get the subscriptionId from D365 payload for first subcription id
            string subscriptionId = D365Payload.PostEntityImages[0].Value.Attributes[0].Value.ToString();

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            HttpResponseMessage newapiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload, EnsToken);
            Assert.AreEqual(202, (int)newapiResponse.StatusCode, $"Incorrect status code {newapiResponse.StatusCode}  is  returned, instead of the expected 202.");

            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.D365ApiUri, subscriptionId);
            Assert.AreEqual(200, (int)callBackResponse.StatusCode, $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            EnsCallbackResponseDetailsModel callBackResponseBody = await callBackResponse.ReadAsTypeAsync<EnsCallbackResponseDetailsModel>();

            foreach (EnsCallbackResponseModel entry in callBackResponseBody.CallBackRequests)
            {
                Assert.AreEqual(subscriptionId, entry.subscriptionId, $"Invalid subscriptionId {entry.subscriptionId} , Instead of expected subscriptionId {subscriptionId}.");
                Assert.AreEqual(TestConfig.SucceededStatusCode, entry.callBackRequest.ukho_lastresponse, $"Invalid last response {TestConfig.SucceededStatusCode} , Instead of expected last response {entry.callBackRequest.ukho_lastresponse}.");
            }
        }
    }
}
