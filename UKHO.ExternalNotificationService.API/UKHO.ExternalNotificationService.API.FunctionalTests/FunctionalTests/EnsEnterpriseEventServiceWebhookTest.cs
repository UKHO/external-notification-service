using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.FunctionalTests.Helper;
using UKHO.ExternalNotificationService.API.FunctionalTests.Model;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.FunctionalTests
{
    class EnsEnterpriseEventServiceWebhookTest
    {
        private EnsApiClient EnsApiClient { get; set; }
        private StubApiClient StubApiClient { get; set; }
        private TestConfiguration TestConfig { get; set; }
        private string EnsToken { get; set; }
        private JObject FssEventBody { get; set; }
        private FssEventData FssEventData { get; set; }
        private JObject ScsEventBody { get; set; }

        [SetUp]
        public async Task SetupAsync()
        {
            TestConfig = new TestConfiguration();
            StubApiClient = new(TestConfig.StubApiUri);
            FssEventBody = FssEventDataBase.GetFssEventBodyData(TestConfig);
            FssEventData = FssEventDataBase.GetFssEventData(TestConfig);
            ScsEventBody = ScsEventDataBase.GetScsEventBodyData(TestConfig);
            EnsApiClient = new EnsApiClient(TestConfig.EnsApiBaseUrl);
            ADAuthTokenProvider adAuthTokenProvider = new();
            EnsToken = await adAuthTokenProvider.GetEnsAuthToken();
        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidOptionHeaderWithoutAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            HttpResponseMessage apiResponse = await EnsApiClient.OptionEnsApiSubscriptionAsync("WebHook-Request-Origin", "eventemitter.example.com");

            Assert.AreEqual(401, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 401.");

        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidOptionHeaderWithInvalidAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            string invalidToken = EnsToken.Remove(EnsToken.Length - 4).Insert(EnsToken.Length - 4, "ABAA");
            HttpResponseMessage apiResponse = await EnsApiClient.OptionEnsApiSubscriptionAsync("WebHook-Request-Origin", "eventemitter.example.com", invalidToken);

            Assert.AreEqual(401, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 401.");

        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidOptionHeader_ThenOkStatusIsReturnedWithResponseHeader()
        {
            string requestHeader = "WebHook-Request-Origin";
            string requestHeaderValue = "eventemitter.example.com";
            HttpResponseMessage apiResponse = await EnsApiClient.OptionEnsApiSubscriptionAsync(requestHeader, requestHeaderValue, EnsToken);

            Assert.AreEqual(200, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");

            Assert.IsTrue(apiResponse.Headers.Contains("WebHook-Allowed-Origin"));

            Assert.AreEqual(requestHeaderValue, apiResponse.Headers.GetValues("WebHook-Allowed-Origin").FirstOrDefault());

        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidFssJObjectBodyWithoutAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            JObject ensWebhookJson = FssEventBody;

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson);

            Assert.AreEqual(401, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 401.");

        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidFssJObjectBodyWithInvalidAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            string invalidToken = EnsToken.Remove(EnsToken.Length - 4).Insert(EnsToken.Length - 4, "ABAA");
            JObject ensWebhookJson = FssEventBody;

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson, invalidToken);

            Assert.AreEqual(401, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 401.");

        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidFssJObjectBody_ThenOkStatusIsReturned()
        {
            const string subject = "83d08093-7a67-4b3a-b431-92ba42feaea0";
            const string addHttps = "https://";
            JObject ensWebhookJson = FssEventBody;

            FssEventData publishDataFromFss = FssEventData;
            await StubApiClient.PostStubApiCommandToReturnStatusAsync(ensWebhookJson, subject, null);

            DateTime startTime = DateTime.UtcNow;
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson, EnsToken);
           
            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(10000);
            }

            Assert.AreEqual(200, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");
            HttpResponseMessage stubResponse = await StubApiClient.GetStubApiCacheReturnStatusAsync(subject, EnsToken);
            string customerJsonString = await stubResponse.Content.ReadAsStringAsync();
            IEnumerable<DistributorRequest> deserialized = JsonConvert.DeserializeObject<IEnumerable<DistributorRequest>>(custome‌​rJsonString);
            DistributorRequest getMatchingData = deserialized.Where(x => x.TimeStamp >= startTime && x.statusCode.HasValue && x.statusCode.Value == HttpStatusCode.OK).OrderByDescending(a => a.TimeStamp).FirstOrDefault();
            Assert.NotNull(getMatchingData);
            Assert.AreEqual(HttpStatusCode.OK, getMatchingData.statusCode);

            // Validating Event Subject
            Assert.AreEqual(subject, getMatchingData.Subject);
            Assert.IsInstanceOf<CustomCloudEvent>(getMatchingData.CloudEvent);

            // Validating Event Source
            Assert.AreEqual(TestConfig.FssSource, getMatchingData.CloudEvent.Source);

            // Validating Event Type
            Assert.AreEqual("uk.co.admiralty.fss.filesPublished.v1", getMatchingData.CloudEvent.Type);
            Uri filesLinkHref = new(publishDataFromFss.Files.FirstOrDefault().Links.Get.Href);
            string data = JsonConvert.SerializeObject(getMatchingData.CloudEvent.Data);
            FssEventData fssEventData = JsonConvert.DeserializeObject<FssEventData>(data);

            // Validating Files Link Href
            Uri filesLinkHrefReplace = new(addHttps + TestConfig.FssPublishHostName + filesLinkHref.AbsolutePath);
            Assert.IsNotNull(filesLinkHrefReplace.ToString());
            Assert.AreEqual(filesLinkHrefReplace.ToString(), fssEventData.Files.FirstOrDefault().Links.Get.Href);

            // Validating Files Batch Status Href
            Uri filesBatchStatusHref = new(publishDataFromFss.Links.BatchStatus.Href);
            Uri filesBatchStatusHrefReplace = new(addHttps + TestConfig.FssPublishHostName + filesBatchStatusHref.AbsolutePath);
            Assert.IsNotNull(filesBatchStatusHrefReplace.ToString());
            Assert.AreEqual(filesBatchStatusHrefReplace.ToString(), fssEventData.Links.BatchStatus.Href);

            // Validating Files Batch Detail Href
            Uri filesBatchDetailHref = new(publishDataFromFss.Links.BatchDetails.Href);
            Uri filesBatchDetailHrefReplace = new(addHttps + TestConfig.FssPublishHostName + filesBatchDetailHref.AbsolutePath);
            Assert.IsNotNull(filesBatchDetailHrefReplace.ToString());
            Assert.AreEqual(filesBatchDetailHrefReplace.ToString(), fssEventData.Links.BatchDetails.Href);
        }

        [TestCase("83d08093-7a67-4b3a-b431-92ba42feaea0", HttpStatusCode.InternalServerError, TestName = "InternalServerError for WebHook")]
        [TestCase("83d08093-7a67-4b3a-b431-92ba42feaea0", HttpStatusCode.BadRequest, TestName = "BadRequest for WebHook")]
        [TestCase("83d08093-7a67-4b3a-b431-92ba42feaea0", HttpStatusCode.NotFound, TestName = "NotFound for WebHook")]
        public async Task WhenICallTheEnsWebhookApiWithAValidFssJObjectBody_ThenNonOkStatusIsReturned(string subject, HttpStatusCode statusCode)
        {
            JObject ensWebhookJson = FssEventBody;
            await StubApiClient.PostStubApiCommandToReturnStatusAsync(ensWebhookJson, subject, statusCode);

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson, EnsToken);
            DateTime startTime = DateTime.UtcNow;

            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(10000);
            }

            Assert.AreEqual(200, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");
            HttpResponseMessage stubResponse = await StubApiClient.GetStubApiCacheReturnStatusAsync(subject, EnsToken);
            // Get the response
            string customerJsonString = await stubResponse.Content.ReadAsStringAsync();
            IEnumerable<DistributorRequest> deserialized = JsonConvert.DeserializeObject<IEnumerable<DistributorRequest>>(custome‌​rJsonString);
            IEnumerable<DistributorRequest> getMatchingData = deserialized.Where(x => x.TimeStamp >= startTime && x.statusCode.HasValue && x.statusCode.Value == statusCode)
                .OrderByDescending(a => a.TimeStamp);
            Assert.NotNull(getMatchingData);
            Assert.Greater(getMatchingData.Count(), 1);
        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidScsJObjectBody_ThenOkStatusIsReturned()
        {
            const string subject = "NO4F1617";
            JObject ensWebhookJson = ScsEventBody;

            await StubApiClient.PostStubApiCommandToReturnStatusAsync(ensWebhookJson, subject, null);

            DateTime startTime = DateTime.UtcNow;
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson, EnsToken);
            
            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(10000);
            }

            Assert.AreEqual(200, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");
            HttpResponseMessage stubResponse = await StubApiClient.GetStubApiCacheReturnStatusAsync(subject, EnsToken);
            string customerJsonString = await stubResponse.Content.ReadAsStringAsync();
            IEnumerable<DistributorRequest> deserialized = JsonConvert.DeserializeObject<IEnumerable<DistributorRequest>>(custome‌​rJsonString);
            DistributorRequest getMatchingData = deserialized.Where(x => x.TimeStamp >= startTime && x.statusCode.HasValue && x.statusCode.Value == HttpStatusCode.OK).OrderByDescending(a => a.TimeStamp).FirstOrDefault();
            Assert.NotNull(getMatchingData);
            Assert.AreEqual(HttpStatusCode.OK, getMatchingData.statusCode);

            // Validating Event Subject
            Assert.AreEqual(subject, getMatchingData.Subject);
            Assert.IsInstanceOf<CustomCloudEvent>(getMatchingData.CloudEvent);

            // Validating Event Source
            Assert.AreEqual(TestConfig.ScsSource, getMatchingData.CloudEvent.Source);

            // Validating Event Type
            Assert.AreEqual("uk.co.admiralty.avcsData.contentPublished.v1", getMatchingData.CloudEvent.Type);

        }
    }
}
