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
    class D365HttpPayloadValidationTest
    {
        private EnsApiClient EnsApiClient { get; set; }
        private TestConfiguration TestConfig { get; set; }
        private D365Payload D365FssAvcsPayload { get; set; }
        private D365Payload D365FssMsiPayload { get; set; }
        private D365Payload D365ScsPayload { get; set; }
        private string EnsToken { get; set; }

        [SetUp]
        public async Task SetupAsync()
        {
            TestConfig = new TestConfiguration();
            EnsApiClient = new EnsApiClient(TestConfig.EnsApiBaseUrl);

            string filePathFssAvcs = Path.Combine(Directory.GetCurrentDirectory(), TestConfig.PayloadFolder, TestConfig.FssAvcsPayloadFileName);
            string filePathFssMsi = Path.Combine(Directory.GetCurrentDirectory(), TestConfig.PayloadFolder, TestConfig.FssMsiPayloadFileName);
            string filePathScs = Path.Combine(Directory.GetCurrentDirectory(), TestConfig.PayloadFolder, TestConfig.ScsPayloadFileName);

            D365FssAvcsPayload = JsonSerializer.Deserialize<D365Payload>(await File.ReadAllTextAsync(filePathFssAvcs));
            D365FssMsiPayload = JsonSerializer.Deserialize<D365Payload>(await File.ReadAllTextAsync(filePathFssMsi));
            D365ScsPayload = JsonSerializer.Deserialize<D365Payload>(await File.ReadAllTextAsync(filePathScs));
            D365FssAvcsPayload.InputParameters[0].Value.Attributes[9].Value = string.Concat(TestConfig.StubBaseUri, TestConfig.WebhookUrlExtension);
            D365FssMsiPayload.InputParameters[0].Value.Attributes[9].Value = string.Concat(TestConfig.StubBaseUri, TestConfig.WebhookUrlExtension);
            D365ScsPayload.InputParameters[0].Value.Attributes[9].Value = string.Concat(TestConfig.StubBaseUri, TestConfig.WebhookUrlExtension);

            ADAuthTokenProvider adAuthTokenProvider = new();
            EnsToken = await adAuthTokenProvider.GetEnsAuthToken();
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidD365PayloadWithoutAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365FssAvcsPayload);
            Assert.That(401, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 401.");
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidD365PayloadWithInvalidAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            string invalidToken = EnsToken.Remove(EnsToken.Length - 4).Insert(EnsToken.Length - 4, "ABAA");
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365FssAvcsPayload, invalidToken);
            Assert.That(401, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 401.");
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidD365FssAvcsPayload_ThenAcceptedStatusIsReturned()
        {
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365FssAvcsPayload, EnsToken);
            Assert.That(202, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidD365FssMsiPayload_ThenAcceptedStatusIsReturned()
        {
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365FssMsiPayload, EnsToken);
            Assert.That(202, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidD365ScsPayload_ThenAcceptedStatusIsReturned()
        {
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365ScsPayload, EnsToken);
            Assert.That(202, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithEmptyD365Payload_ThenABadRequestStatusIsReturned()
        {
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(null, EnsToken);
            Assert.That(400, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 400.");

            ErrorDescriptionModel errorMessage = await apiResponse.ReadAsTypeAsync<ErrorDescriptionModel>();

            Assert.That(errorMessage.Errors.Any(e => e.Source == "requestBody"));
            Assert.That(errorMessage.Errors.Any(e => e.Description == "Either body is null or malformed."));
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithMissingInputParametersInD365Payload_ThenABadRequestStatusIsReturned()
        {
            D365FssAvcsPayload.InputParameters = null;

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365FssAvcsPayload, EnsToken);
            Assert.That(400, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 400.");

            ErrorDescriptionModel errorMessage = await apiResponse.ReadAsTypeAsync<ErrorDescriptionModel>();

            Assert.That(errorMessage.Errors.Any(e => e.Source == "inputParameters"));
            Assert.That(errorMessage.Errors.Any(e => e.Description == "D365Payload InputParameters cannot be blank or null."));
        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithMissingPostEntityImagesInD365Payload_ThenABadRequestStatusIsReturned()
        {
            D365FssAvcsPayload.PostEntityImages = null;

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365FssAvcsPayload, EnsToken);
            Assert.That(400, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 400.");

            ErrorDescriptionModel errorMessage = await apiResponse.ReadAsTypeAsync<ErrorDescriptionModel>();

            Assert.That(errorMessage.Errors.Any(e => e.Source == "postEntityImages"));
            Assert.That(errorMessage.Errors.Any(e => e.Description == "D365Payload PostEntityImages cannot be blank or null."));
        }
    }
}
