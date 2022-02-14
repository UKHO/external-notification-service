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
    class D365HttpPayloadValidationTest
    {
        private EnsApiClient EnsApiClient { get; set; }
        private TestConfiguration TestConfig { get; set; }
        private D365Payload D365Payload { get; set; }

        [SetUp]
        public void Setup()
        {
            TestConfig = new TestConfiguration();
            EnsApiClient = new EnsApiClient(TestConfig.EnsApiBaseUrl);

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), TestConfig.PayloadFolder, TestConfig.PayloadFileName);

            D365Payload = JsonConvert.DeserializeObject<D365Payload>(File.ReadAllText(filePath));

        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidD365Payload_ThenAcceptedStatusIsReturned()
        {

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithEmptyD365Payload_ThenABadRequestStatusIsReturned()
        {
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(null);
            Assert.AreEqual(400, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 400.");

            ErrorDescriptionModel errorMessage =await apiResponse.ReadAsTypeAsync<ErrorDescriptionModel>();

            Assert.IsTrue(errorMessage.Errors.Any(e => e.Source == "requestBody"));
            Assert.IsTrue(errorMessage.Errors.Any(e => e.Description == "Either body is null or malformed."));
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithMissingInputParametersInD365Payload_ThenABadRequestStatusIsReturned()
        {
            D365Payload.InputParameters = null;

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload);
            Assert.AreEqual(400, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 400.");

            ErrorDescriptionModel errorMessage = await apiResponse.ReadAsTypeAsync<ErrorDescriptionModel>();

            Assert.IsTrue(errorMessage.Errors.Any(e => e.Source == "inputParameters"));
            Assert.IsTrue(errorMessage.Errors.Any(e => e.Description == "D365Payload InputParameters cannot be blank or null."));
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithMissingPostEntityImagesInD365Payload_ThenABadRequestStatusIsReturned()
        {
            D365Payload.PostEntityImages = null;

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload);
            Assert.AreEqual(400, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 400.");

            ErrorDescriptionModel errorMessage = await apiResponse.ReadAsTypeAsync<ErrorDescriptionModel>();

            Assert.IsTrue(errorMessage.Errors.Any(e => e.Source == "postEntityImages"));
            Assert.IsTrue(errorMessage.Errors.Any(e => e.Description == "D365Payload PostEntityImages cannot be blank or null."));
        }
    }
}
