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
    class EnsNotificationConfigurationTest
    {
        private EnsApiClient _ensApiClient { get; set; }
        private TestConfiguration _testConfig { get; set; }
        private D365Payload _d365Payload { get; set; }

        [SetUp]
        public void Setup()
        {
            _testConfig = new TestConfiguration();
            _ensApiClient = new EnsApiClient(_testConfig.EnsApiBaseUrl);

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), _testConfig.PayloadFolder, _testConfig.PayloadFileName);

            _d365Payload = JsonConvert.DeserializeObject<D365Payload>(File.ReadAllText(filePath));

        }

        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidNotificationType_ThenAcceptedStatusIsReturned()
        {
            HttpResponseMessage apiResponse = await _ensApiClient.PostEnsApiSubscriptionAsync(_d365Payload);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 202.");

        }

        [TestCase("", "NotificationType cannot be blank or null.", TestName = "Notification Type Value Is Blank")]
        [TestCase(null, "NotificationType cannot be blank or null.", TestName = "Notification Type Value Is Null")]
        [TestCase("ABC", "Invalid Notification Type 'ABC'", TestName = "Notification Type Value Is Invalid And Not Exist In The Configuration")]
        public async Task WhenICallTheEnsSubscriptionApiWithAnInValidNotificationType_ThenABadRequestStatusIsReturned(string notificationType,string validationMessage)
        {
            _d365Payload.InputParameters[0].Value.FormattedValues[4].Value = notificationType;

            HttpResponseMessage apiResponse = await _ensApiClient.PostEnsApiSubscriptionAsync(_d365Payload);
            Assert.AreEqual(400, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 400.");

            ErrorDescriptionModel errorMessage = await apiResponse.ReadAsTypeAsync<ErrorDescriptionModel>();

            Assert.IsTrue(errorMessage.Errors.Any(e => e.Source == "notificationType"));
            Assert.IsTrue(errorMessage.Errors.Any(e => e.Description == validationMessage));
        }
    }
}
