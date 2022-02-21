using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.FunctionalTests.Helper;
using UKHO.ExternalNotificationService.API.FunctionalTests.Model;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.FunctionalTests
{
    class EnsEnterpriseEventServiceWebhookTest
    {
        private EnsApiClient EnsApiClient { get; set; }
        private TestConfiguration TestConfig { get; set; }
      

        [SetUp]
        public void Setup()
        {
            TestConfig = new TestConfiguration();
            EnsApiClient = new EnsApiClient(TestConfig.EnsApiBaseUrl);

        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidOptionHeader_ThenOkStatusIsReturnedWithResponseHeader()
        {
            string requestHeader = "WebHook-Request-Origin";
            string requestHeaderValue = "eventemitter.example.com";
            HttpResponseMessage apiResponse = await EnsApiClient.OptionEnsApiSubscriptionAsync(requestHeader,requestHeaderValue);  

            Assert.AreEqual(200, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");

            Assert.IsTrue(apiResponse.Headers.Contains("WebHook-Allowed-Origin"));

            Assert.AreEqual(requestHeaderValue,apiResponse.Headers.GetValues("WebHook-Allowed-Origin").FirstOrDefault());


        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidpost_ThenAcceptedStatusIsReturned()
        {
            JObject request = new JObject();

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebookNewEventPublishedAsync(request);
            string response = apiResponse.ToString();
            Assert.AreEqual(200, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");

        }
       


    }
}
