using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.FunctionalTests.Helper;
using UKHO.ExternalNotificationService.API.FunctionalTests.Model;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.FunctionalTests
{
    class D365HttpPayloadValidation
    {
        private EnsApiClient ensApiClient { get; set; }
        private TestConfiguration testConfig { get; set; }

        [OneTimeSetUp]
        public void Setup()
        {
            testConfig = new TestConfiguration();
            ensApiClient = new EnsApiClient(testConfig.ensApiBaseUrl);
           
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidD365Payload_ThenAcceptedStatusIsReturned()
        {
           
            string filePath = Path.Combine(Directory.GetCurrentDirectory(),testConfig.payloadFolder, testConfig.payloadFileName);
            
            D365Payload d365Payload = JsonConvert.DeserializeObject<D365Payload>(File.ReadAllText(filePath));
            
            var apiResponse = await ensApiClient.PostEnsApiSubcriptionAsync(d365Payload);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithEmptyD365Payload_ThenABadRequestStatusIsReturned()
        {
            D365Payload d365Payload = new D365Payload();

            var apiResponse = await ensApiClient.PostEnsApiSubcriptionAsync(d365Payload);
            Assert.AreEqual(400, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 400.");

            var errorMessage =await apiResponse.ReadAsTypeAsync<ErrorDescriptionModel>();

            Assert.IsTrue(errorMessage.Errors.Any(e => e.Source == "requestBody"));
            Assert.IsTrue(errorMessage.Errors.Any(e => e.Description == "Either body is null or malformed."));
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithMissingInputParametersInD365Payload_ThenABadRequestStatusIsReturned()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), testConfig.payloadFolder, testConfig.payloadFileName);

            D365Payload d365Payload = JsonConvert.DeserializeObject<D365Payload>(File.ReadAllText(filePath));

            d365Payload.InputParameters = null;

            var apiResponse = await ensApiClient.PostEnsApiSubcriptionAsync(d365Payload);
            Assert.AreEqual(400, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 400.");

            var errorMessage = await apiResponse.ReadAsTypeAsync<ErrorDescriptionModel>();

            Assert.IsTrue(errorMessage.Errors.Any(e => e.Source == "inputParameters"));
            Assert.IsTrue(errorMessage.Errors.Any(e => e.Description == "inputParameters cannot be null."));
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithMissingPostEntityImagesInD365Payload_ThenABadRequestStatusIsReturned()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), testConfig.payloadFolder, testConfig.payloadFileName);

            D365Payload d365Payload = JsonConvert.DeserializeObject<D365Payload>(File.ReadAllText(filePath));

            d365Payload.PostEntityImages = null;

            var apiResponse = await ensApiClient.PostEnsApiSubcriptionAsync(d365Payload);
            Assert.AreEqual(400, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 400.");

            var errorMessage = await apiResponse.ReadAsTypeAsync<ErrorDescriptionModel>();

            Assert.IsTrue(errorMessage.Errors.Any(e => e.Source == "postEntityImages"));
            Assert.IsTrue(errorMessage.Errors.Any(e => e.Description == "postEntityImages cannot be null."));
        }
    }
}
