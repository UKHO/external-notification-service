using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.FunctionalTests.Helper;
using UKHO.ExternalNotificationService.API.FunctionalTests.Model;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.FunctionalTests
{
    [TestFixture]
    public class EnsEnterpriseEventServiceWebhookTest
    {
        private EnsApiClient EnsApiClient { get; set; }
        private StubApiClient StubApiClient { get; set; }
        private TestConfiguration TestConfig { get; set; }
        private string EnsToken { get; set; }
        private JsonObject FssEventBody { get; set; }
        private JsonObject ScsEventBody { get; set; }
        private JsonSerializerOptions JOptions { get; set; }

        [SetUp]
        public async Task SetupAsync()
        {
            TestConfig = new TestConfiguration();
            StubApiClient = new(TestConfig.StubApiUri);
            FssEventBody = FssEventDataBase.GetFssEventBodyData(TestConfig);
            ScsEventBody = ScsEventDataBase.GetScsEventBodyData(TestConfig);
            EnsApiClient = new EnsApiClient(TestConfig.EnsApiBaseUrl);
            ADAuthTokenProvider adAuthTokenProvider = new();
            EnsToken = await adAuthTokenProvider.GetEnsAuthToken();
            JOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidOptionHeaderWithoutAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            var apiResponse = await EnsApiClient.OptionEnsApiSubscriptionAsync("WebHook-Request-Origin", "eventemitter.example.com");

            Assert.That((int)apiResponse.StatusCode, Is.EqualTo(401), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 401.");
        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidOptionHeaderWithInvalidAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            var invalidToken = EnsToken.Remove(EnsToken.Length - 4).Insert(EnsToken.Length - 4, "ABAA");
            var apiResponse = await EnsApiClient.OptionEnsApiSubscriptionAsync("WebHook-Request-Origin", "eventemitter.example.com", invalidToken);

            Assert.That((int)apiResponse.StatusCode, Is.EqualTo(401), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 401.");
        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidOptionHeader_ThenOkStatusIsReturnedWithResponseHeader()
        {
            var requestHeader = "WebHook-Request-Origin";
            var requestHeaderValue = "eventemitter.example.com";
            var apiResponse = await EnsApiClient.OptionEnsApiSubscriptionAsync(requestHeader, requestHeaderValue, EnsToken);

            Assert.Multiple(() =>
            {
                Assert.That((int)apiResponse.StatusCode, Is.EqualTo(200), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");
                Assert.That(apiResponse.Headers.Contains("WebHook-Allowed-Origin"));
                Assert.That(requestHeaderValue, Is.EqualTo(apiResponse.Headers.GetValues("WebHook-Allowed-Origin").FirstOrDefault()));
            });
        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidFssJObjectBodyWithoutAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            var ensWebhookJson = FssEventBody;

            var apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson);

            Assert.That((int)apiResponse.StatusCode, Is.EqualTo(401), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 401.");
        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidFssJObjectBodyWithInvalidAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            var invalidToken = EnsToken.Remove(EnsToken.Length - 4).Insert(EnsToken.Length - 4, "ABAA");
            var ensWebhookJson = FssEventBody;

            var apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson, invalidToken);

            Assert.That((int)apiResponse.StatusCode, Is.EqualTo(401), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 401.");
        }

        [TestCase("AVCSData", "fss-filesPublished-AvcsData", TestName = "Valid AVCSData Business Unit event")]
        [TestCase("MaritimeSafetyInformation", "fss-filesPublished-MaritimeSafetyInformation", TestName = "Valid MaritimeSafetyInformation Business Unit event")]
        public async Task WhenICallTheEnsWebhookApiWithAValidFssJObjectBody_ThenOkStatusIsReturned(string businessUnit, string source)
        {
            const string subject = "83d08093-7a67-4b3a-b431-92ba42feaea0";
            const string addHttps = "https://";

            var ensWebhookJson = FssEventDataBase.GetFssEventBodyData(TestConfig, businessUnit);

            var publishDataFromFss = FssEventDataBase.GetFssEventData(TestConfig, businessUnit);

            await StubApiClient.PostStubApiCommandToReturnStatusAsync(ensWebhookJson, subject, null);

            var startTime = DateTime.UtcNow;
            var apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson, EnsToken);

            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(10000);
            }

            Assert.That((int)apiResponse.StatusCode, Is.EqualTo(200), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");

            var stubResponse = await StubApiClient.GetStubApiCacheReturnStatusAsync(subject, EnsToken);
            Assert.That(stubResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var customerJsonString = await stubResponse.Content.ReadAsStringAsync();
            var deserialized = JsonSerializer.Deserialize<IEnumerable<DistributorRequest>>(custome‌​rJsonString, JOptions);
            var getMatchingData = deserialized.Where(x => x.TimeStamp >= startTime && x.StatusCode is HttpStatusCode.OK && x.CloudEvent.Source == source)
                .OrderByDescending(a => a.TimeStamp)
                .FirstOrDefault();
            Assert.That(getMatchingData, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(getMatchingData.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                // Validating Event Subject
                Assert.That(getMatchingData.Subject, Is.EqualTo(subject));
                Assert.That(getMatchingData.CloudEvent, Is.InstanceOf<CustomCloudEvent>());

                // Validating Event Source
                Assert.That(getMatchingData.CloudEvent.Source, Is.EqualTo(TestConfig.FssSources.Single(x => x.BusinessUnit == businessUnit).Source));

                // Validating Event Type
                Assert.That(getMatchingData.CloudEvent.Type, Is.EqualTo("uk.co.admiralty.fss.filesPublished.v1"));
            });
            var filesLinkHref = new Uri(publishDataFromFss.Files.FirstOrDefault().Links.Get.Href);
            var data = JsonSerializer.Serialize(getMatchingData.CloudEvent.Data);
            var fssEventData = JsonSerializer.Deserialize<FssEventData>(data, JOptions);

            // Validating Files Link Href
            var filesLinkHrefReplace = new Uri(addHttps + TestConfig.FssPublishHostName + filesLinkHref.AbsolutePath);
            Assert.That(filesLinkHrefReplace.ToString(), Is.Not.Null);
            Assert.That(filesLinkHrefReplace.ToString(), Is.EqualTo(fssEventData.Files.FirstOrDefault().Links.Get.Href));

            // Validating Files Batch Status Href
            Uri filesBatchStatusHref = new(publishDataFromFss.Links.BatchStatus.Href);
            Uri filesBatchStatusHrefReplace = new(addHttps + TestConfig.FssPublishHostName + filesBatchStatusHref.AbsolutePath);
            Assert.That(filesBatchStatusHrefReplace.ToString(), Is.Not.Null);
            Assert.That(filesBatchStatusHrefReplace.ToString(), Is.EqualTo(fssEventData.Links.BatchStatus.Href));

            // Validating Files Batch Detail Href
            var filesBatchDetailHref = new Uri(publishDataFromFss.Links.BatchDetails.Href);
            var filesBatchDetailHrefReplace = new Uri(addHttps + TestConfig.FssPublishHostName + filesBatchDetailHref.AbsolutePath);
            Assert.That(filesBatchDetailHrefReplace.ToString(), Is.Not.Null);
            Assert.That(filesBatchDetailHrefReplace.ToString(), Is.EqualTo(fssEventData.Links.BatchDetails.Href));
        }

        [TestCase("83d08093-7a67-4b3a-b431-92ba42feaea0", HttpStatusCode.InternalServerError, TestName = "InternalServerError for WebHook")]
        [TestCase("83d08093-7a67-4b3a-b431-92ba42feaea0", HttpStatusCode.BadRequest, TestName = "BadRequest for WebHook")]
        [TestCase("83d08093-7a67-4b3a-b431-92ba42feaea0", HttpStatusCode.NotFound, TestName = "NotFound for WebHook")]
        public async Task WhenICallTheEnsWebhookApiWithAValidFssJObjectBody_ThenNonOkStatusIsReturned(string subject, HttpStatusCode statusCode)
        {
            var ensWebhookJson = FssEventBody;
            await StubApiClient.PostStubApiCommandToReturnStatusAsync(ensWebhookJson, subject, statusCode);
            var apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson, EnsToken);
            var startTime = DateTime.UtcNow;

            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(10000);
            }

            Assert.That((int)apiResponse.StatusCode, Is.EqualTo(200), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");
            var stubResponse = await StubApiClient.GetStubApiCacheReturnStatusAsync(subject, EnsToken);
            Assert.That(stubResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            // Get the response
            var customerJsonString = await stubResponse.Content.ReadAsStringAsync();

            var deserialized = JsonSerializer.Deserialize<IEnumerable<DistributorRequest>>(custome‌​rJsonString, JOptions);
            var getMatchingData = deserialized.Where(x => x.TimeStamp >= startTime && x.StatusCode.HasValue && x.StatusCode.Value == statusCode).OrderByDescending(a => a.TimeStamp);
            Assert.That(getMatchingData, Is.Not.Null);
            Assert.That(getMatchingData.Count(), Is.GreaterThan(1));
        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidScsJObjectBody_ThenOkStatusIsReturned()
        {
            const string subject = "GB53496A";
            var ensWebhookJson = ScsEventBody;
            await StubApiClient.PostStubApiCommandToReturnStatusAsync(ensWebhookJson, subject, null);
            var startTime = DateTime.UtcNow;
            var apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson, EnsToken);

            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(10000);
            }

            Assert.That((int)apiResponse.StatusCode, Is.EqualTo(200), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");
            var stubResponse = await StubApiClient.GetStubApiCacheReturnStatusAsync(subject, EnsToken);
            Assert.That(stubResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var customerJsonString = await stubResponse.Content.ReadAsStringAsync();

            var deserialized = JsonSerializer.Deserialize<IEnumerable<DistributorRequest>>(custome‌​rJsonString, JOptions);

            var getMatchingData = deserialized.Where(x => x.TimeStamp >= startTime && x.StatusCode.HasValue && x.StatusCode.Value == HttpStatusCode.OK).OrderByDescending(a => a.TimeStamp).FirstOrDefault();
            Assert.That(getMatchingData, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(getMatchingData.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                // Validating Event Subject
                Assert.That(getMatchingData.Subject, Is.EqualTo(subject));
                Assert.That(getMatchingData.CloudEvent, Is.InstanceOf<CustomCloudEvent>());
            });

            Assert.Multiple(() =>
            {
                // Validating Event Source
                Assert.That(getMatchingData.CloudEvent.Source, Is.EqualTo(TestConfig.ScsSource));

                // Validating Event Type
                Assert.That(getMatchingData.CloudEvent.Type, Is.EqualTo("uk.co.admiralty.avcsData.contentPublished.v1"));
            });
        }
    }
}
