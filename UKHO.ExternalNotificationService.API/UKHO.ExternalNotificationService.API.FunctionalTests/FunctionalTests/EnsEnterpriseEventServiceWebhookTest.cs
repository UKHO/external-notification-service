using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        private JsonSerializerOptions JOptions { get; set; }

        // Test data object for SCS events
        public sealed record ScsEventTestData(JsonObject EventBody, string CloudEventType, string CloudEventSource);

        // TestCaseSource supplying SCS test data objects
        public static IEnumerable<TestCaseData> ScsEventTestCases()
        {
            var cfg = new TestConfiguration();
            yield return new TestCaseData(new ScsEventTestData(ScsEventDataBase.GetScsEventBodyData(cfg),
                    "uk.co.admiralty.avcsData.contentPublished.v1", cfg.ScsSource))
                .SetName("Valid Scs events for S63/S57 event");
            yield return new TestCaseData(new ScsEventTestData(ScsEventDataBase.GetScsS100EventBodyData(cfg),
                    "uk.co.admiralty.avcsData.s100ContentPublished.v1", cfg.ScsS100Source))
                .SetName("Valid Scs events for S100 event");
        }


        [SetUp]
        public async Task SetupAsync()
        {
            TestConfig = new TestConfiguration();
            StubApiClient = new(TestConfig.StubApiUri);
            FssEventBody = FssEventDataBase.GetFssEventBodyData(TestConfig);
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
            HttpResponseMessage apiResponse = await EnsApiClient.OptionEnsApiSubscriptionAsync("WebHook-Request-Origin", "eventemitter.example.com");

            Assert.That((int)apiResponse.StatusCode, Is.EqualTo(401), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 401.");
        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidOptionHeaderWithInvalidAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            string invalidToken = EnsToken.Remove(EnsToken.Length - 4).Insert(EnsToken.Length - 4, "ABAA");
            HttpResponseMessage apiResponse = await EnsApiClient.OptionEnsApiSubscriptionAsync("WebHook-Request-Origin", "eventemitter.example.com", invalidToken);

            Assert.That((int)apiResponse.StatusCode, Is.EqualTo(401), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 401.");
        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidOptionHeader_ThenOkStatusIsReturnedWithResponseHeader()
        {
            string requestHeader = "WebHook-Request-Origin";
            string requestHeaderValue = "eventemitter.example.com";
            HttpResponseMessage apiResponse = await EnsApiClient.OptionEnsApiSubscriptionAsync(requestHeader, requestHeaderValue, EnsToken);

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
            JsonObject ensWebhookJson = FssEventBody;

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson);

            Assert.That((int)apiResponse.StatusCode, Is.EqualTo(401), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 401.");
        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidFssJObjectBodyWithInvalidAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            string invalidToken = EnsToken.Remove(EnsToken.Length - 4).Insert(EnsToken.Length - 4, "ABAA");
            JsonObject ensWebhookJson = FssEventBody;

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson, invalidToken);

            Assert.That((int)apiResponse.StatusCode, Is.EqualTo(401), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 401.");
        }

        [TestCase("AVCSData", "fss-filesPublished-AvcsData", TestName = "Valid AVCSData Business Unit event")]
        [TestCase("ADP", "fss-filesPublished-AvcsData", TestName = "Valid ADP Business Unit event")]
        [TestCase("AENP", "fss-filesPublished-AvcsData", TestName = "Valid AENP Business Unit event")]
        [TestCase("ARCS", "fss-filesPublished-AvcsData", TestName = "Valid ARCS Business Unit event")]
        [TestCase("MaritimeSafetyInformation", "fss-filesPublished-MaritimeSafetyInformation", TestName = "Valid MaritimeSafetyInformation Business Unit event")]
        public async Task WhenICallTheEnsWebhookApiWithAValidFssJObjectBody_ThenOkStatusIsReturned(string businessUnit, string source)
        {
            const string subject = "83d08093-7a67-4b3a-b431-92ba42feaea0";
            const string addHttps = "https://";

            JsonObject ensWebhookJson = FssEventDataBase.GetFssEventBodyData(TestConfig, businessUnit);

            FssEventData publishDataFromFss = FssEventDataBase.GetFssEventData(TestConfig, businessUnit);

            await StubApiClient.PostStubApiCommandToReturnStatusAsync(ensWebhookJson, subject, null);

            DateTime startTime = DateTime.UtcNow;
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson, EnsToken);

            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(10000);
            }

            Assert.That((int)apiResponse.StatusCode, Is.EqualTo(200), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");

            HttpResponseMessage stubResponse = await StubApiClient.GetStubApiCacheReturnStatusAsync(subject, EnsToken);
            Assert.That(stubResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            string customerJsonString = await stubResponse.Content.ReadAsStringAsync();
            IEnumerable<DistributorRequest> deserialized = JsonSerializer.Deserialize<IEnumerable<DistributorRequest>>(custome‌​rJsonString, JOptions);
            DistributorRequest getMatchingData = deserialized.Where(x => x.TimeStamp >= startTime && x.StatusCode is HttpStatusCode.OK && x.CloudEvent.Source == source)
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
            Uri filesLinkHref = new(publishDataFromFss.Files.FirstOrDefault().Links.Get.Href);
            string data = JsonSerializer.Serialize(getMatchingData.CloudEvent.Data);
            FssEventData fssEventData = JsonSerializer.Deserialize<FssEventData>(data, JOptions);

            // Validating Files Link Href
            Uri filesLinkHrefReplace = new(addHttps + TestConfig.FssPublishHostName + filesLinkHref.AbsolutePath);
            Assert.That(filesLinkHrefReplace.ToString(), Is.Not.Null);
            Assert.That(filesLinkHrefReplace.ToString(), Is.EqualTo(fssEventData.Files.FirstOrDefault().Links.Get.Href));

            // Validating Files Batch Status Href
            Uri filesBatchStatusHref = new(publishDataFromFss.Links.BatchStatus.Href);
            Uri filesBatchStatusHrefReplace = new(addHttps + TestConfig.FssPublishHostName + filesBatchStatusHref.AbsolutePath);
            Assert.That(filesBatchStatusHrefReplace.ToString(), Is.Not.Null);
            Assert.That(filesBatchStatusHrefReplace.ToString(), Is.EqualTo(fssEventData.Links.BatchStatus.Href));

            // Validating Files Batch Detail Href
            Uri filesBatchDetailHref = new(publishDataFromFss.Links.BatchDetails.Href);
            Uri filesBatchDetailHrefReplace = new(addHttps + TestConfig.FssPublishHostName + filesBatchDetailHref.AbsolutePath);
            Assert.That(filesBatchDetailHrefReplace.ToString(), Is.Not.Null);
            Assert.That(filesBatchDetailHrefReplace.ToString(), Is.EqualTo(fssEventData.Links.BatchDetails.Href));
        }

        [TestCase("83d08093-7a67-4b3a-b431-92ba42feaea0", HttpStatusCode.InternalServerError, TestName = "InternalServerError for WebHook")]
        [TestCase("83d08093-7a67-4b3a-b431-92ba42feaea0", HttpStatusCode.BadRequest, TestName = "BadRequest for WebHook")]
        [TestCase("83d08093-7a67-4b3a-b431-92ba42feaea0", HttpStatusCode.NotFound, TestName = "NotFound for WebHook")]
        public async Task WhenICallTheEnsWebhookApiWithAValidFssJObjectBody_ThenNonOkStatusIsReturned(string subject, HttpStatusCode statusCode)
        {
            JsonObject ensWebhookJson = FssEventBody;
            await StubApiClient.PostStubApiCommandToReturnStatusAsync(ensWebhookJson, subject, statusCode);
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson, EnsToken);
            DateTime startTime = DateTime.UtcNow;

            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(10000);
            }

            Assert.That((int)apiResponse.StatusCode, Is.EqualTo(200), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");
            HttpResponseMessage stubResponse = await StubApiClient.GetStubApiCacheReturnStatusAsync(subject, EnsToken);
            Assert.That(stubResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            // Get the response
            string customerJsonString = await stubResponse.Content.ReadAsStringAsync();

            IEnumerable<DistributorRequest> deserialized = JsonSerializer.Deserialize<IEnumerable<DistributorRequest>>(custome‌​rJsonString, JOptions);
            IEnumerable<DistributorRequest> getMatchingData = deserialized.Where(x => x.TimeStamp >= startTime && x.StatusCode.HasValue && x.StatusCode.Value == statusCode).OrderByDescending(a => a.TimeStamp);
            Assert.That(getMatchingData, Is.Not.Null);
            Assert.That(getMatchingData.Count(), Is.GreaterThan(1));
        }

        [TestCaseSource(nameof(ScsEventTestCases))]
        public async Task WhenICallTheEnsWebhookApiWithAValidScsJObjectBody_ThenOkStatusIsReturned(ScsEventTestData scsEventTestData)
        {
            const string subject = "GB53496A";
            JsonObject ensWebhookJson = scsEventTestData.EventBody;
            
            await StubApiClient.PostStubApiCommandToReturnStatusAsync(ensWebhookJson, subject, null);
            DateTime startTime = DateTime.UtcNow;
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson, EnsToken);

            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(10000);
            }

            Assert.That((int)apiResponse.StatusCode, Is.EqualTo(200), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");
            HttpResponseMessage stubResponse = await StubApiClient.GetStubApiCacheReturnStatusAsync(subject, EnsToken);
            Assert.That(stubResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            string customerJsonString = await stubResponse.Content.ReadAsStringAsync();

            IEnumerable<DistributorRequest> deserialized = JsonSerializer.Deserialize<IEnumerable<DistributorRequest>>(custome‌​rJsonString, JOptions);

            DistributorRequest getMatchingData = deserialized.Where(x => x.TimeStamp >= startTime && x.StatusCode.HasValue && x.StatusCode.Value == HttpStatusCode.OK).OrderByDescending(a => a.TimeStamp).FirstOrDefault();
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
                Assert.That(getMatchingData.CloudEvent.Source, Is.EqualTo(scsEventTestData.CloudEventSource));

                // Validating Event Type
                Assert.That(getMatchingData.CloudEvent.Type, Is.EqualTo(scsEventTestData.CloudEventType));
            });
        }
    }
}
