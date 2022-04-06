using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.FunctionalTests.Helper;
using UKHO.ExternalNotificationService.API.FunctionalTests.Model;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.FunctionalTests
{
    class D365HttpPayloadValidationTest
    {
        private EnsApiClient EnsApiClient { get; set; }
        private TestConfiguration TestConfig { get; set; }
        private D365Payload D365FssPayload { get; set; }
        private D365Payload D365ScsPayload { get; set; }
        private string EnsToken { get; set; }

        [SetUp]
        public async Task SetupAsync()
        {
            TestConfig = new TestConfiguration();
            EnsApiClient = new EnsApiClient(TestConfig.EnsApiBaseUrl);

            string filePathFss = Path.Combine(Directory.GetCurrentDirectory(), TestConfig.PayloadFolder, TestConfig.FssPayloadFileName);
            string filePathScs = Path.Combine(Directory.GetCurrentDirectory(), TestConfig.PayloadFolder, TestConfig.ScsPayloadFileName);

            D365FssPayload = JsonConvert.DeserializeObject<D365Payload>(await File.ReadAllTextAsync(filePathFss));
            D365ScsPayload = JsonConvert.DeserializeObject<D365Payload>(await File.ReadAllTextAsync(filePathScs));
            D365FssPayload.InputParameters[0].Value.Attributes[9].Value = TestConfig.StubBaseUri;
            D365ScsPayload.InputParameters[0].Value.Attributes[9].Value = TestConfig.StubBaseUri;

            ADAuthTokenProvider adAuthTokenProvider = new();
            EnsToken = await adAuthTokenProvider.GetEnsAuthToken();

        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidD365PayloadWithoutAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365FssPayload);
            Assert.AreEqual(401, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 401.");

        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidD365PayloadWithInvalidAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            string invalidToken = EnsToken.Remove(EnsToken.Length - 4).Insert(EnsToken.Length - 4, "ABAA");
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365FssPayload, invalidToken);
            Assert.AreEqual(401, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 401.");

        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidD365FssPayload_ThenAcceptedStatusIsReturned()
        {

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365FssPayload, EnsToken);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidD365ScsPayload_ThenAcceptedStatusIsReturned()
        {

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365ScsPayload, EnsToken);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithEmptyD365Payload_ThenABadRequestStatusIsReturned()
        {
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(null, EnsToken);
            Assert.AreEqual(400, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 400.");

            ErrorDescriptionModel errorMessage =await apiResponse.ReadAsTypeAsync<ErrorDescriptionModel>();

            Assert.IsTrue(errorMessage.Errors.Any(e => e.Source == "requestBody"));
            Assert.IsTrue(errorMessage.Errors.Any(e => e.Description == "Either body is null or malformed."));
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithMissingInputParametersInD365Payload_ThenABadRequestStatusIsReturned()
        {
            D365FssPayload.InputParameters = null;

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365FssPayload, EnsToken);
            Assert.AreEqual(400, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 400.");

            ErrorDescriptionModel errorMessage = await apiResponse.ReadAsTypeAsync<ErrorDescriptionModel>();

            Assert.IsTrue(errorMessage.Errors.Any(e => e.Source == "inputParameters"));
            Assert.IsTrue(errorMessage.Errors.Any(e => e.Description == "D365Payload InputParameters cannot be blank or null."));
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithMissingPostEntityImagesInD365Payload_ThenABadRequestStatusIsReturned()
        {
            D365FssPayload.PostEntityImages = null;

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365FssPayload, EnsToken);
            Assert.AreEqual(400, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 400.");

            ErrorDescriptionModel errorMessage = await apiResponse.ReadAsTypeAsync<ErrorDescriptionModel>();

            Assert.IsTrue(errorMessage.Errors.Any(e => e.Source == "postEntityImages"));
            Assert.IsTrue(errorMessage.Errors.Any(e => e.Description == "D365Payload PostEntityImages cannot be blank or null."));
        }
    }
}
