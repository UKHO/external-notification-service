﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.FunctionalTests.Helper;
using UKHO.ExternalNotificationService.API.FunctionalTests.Model;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.FunctionalTests
{
    class DeadLetterCallbackToD365Test
    {
        private EnsApiClient EnsApiClient { get; set; }
        private TestConfiguration TestConfig { get; set; }
        private D365Payload D365Payload { get; set; }
        private string EnsToken { get; set; }
        private StubApiClient StubApiClient { get; set; }
        private JObject FssEventBody { get; set; }


        [SetUp]
        public async Task SetupAsync()
        {
            TestConfig = new TestConfiguration();
            EnsApiClient = new EnsApiClient(TestConfig.EnsApiBaseUrl);
            StubApiClient = new(TestConfig.StubApiUri);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), TestConfig.PayloadFolder, TestConfig.FssAvcsPayloadFileName);
            D365Payload = JsonConvert.DeserializeObject<D365Payload>(await File.ReadAllTextAsync(filePath));

            ADAuthTokenProvider adAuthTokenProvider = new();
            EnsToken = await adAuthTokenProvider.GetEnsAuthToken();
            FssEventBody = FssEventDataBase.GetFssEventBodyData(TestConfig);

        }

        [TestCase("83d08093-7a67-4b3a-b431-92ba42feaea0", HttpStatusCode.BadRequest, TestName = "BadRequest for WebHook")]
        public async Task WhenICallTheEnsWebhookApiWithAINValidFssJObjectBodyForDeadLetterCallbackToD365UsingDataverse_ThenNonOkStatusIsReturned(string subject, HttpStatusCode statusCode)
        {
            string subscriptionId = D365Payload.PostEntityImages[0].Value.Attributes[0].Value.ToString();
            JObject ensWebhookJson = FssEventBody;
            await StubApiClient.PostStubApiCommandToReturnStatusAsync(ensWebhookJson, subject, statusCode);

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebhookNewEventPublishedAsync(ensWebhookJson, EnsToken);
            DateTime startTime = DateTime.UtcNow;

            //while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            //{
            //    await Task.Delay(10000);
            //}

            await Task.Delay(TestConfig.WaitingTimeForQueueInSeconds * 1000);

            Assert.That(200, Is.EqualTo((int)apiResponse.StatusCode), $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");
            HttpResponseMessage stubResponse = await StubApiClient.GetStubApiCacheReturnStatusAsync(subject, EnsToken);

            // Get the response
            string customerJsonString = await stubResponse.Content.ReadAsStringAsync();
            IEnumerable<DistributorRequest> deserialized = JsonConvert.DeserializeObject<IEnumerable<DistributorRequest>>(custome‌​rJsonString);
            IEnumerable<DistributorRequest> getMatchingData = deserialized.Where(x => x.TimeStamp >= startTime && x.statusCode.HasValue && x.statusCode.Value == statusCode)
                .OrderByDescending(a => a.TimeStamp);
            Assert.That(getMatchingData, Is.Not.Null);
            Assert.That(getMatchingData.Count() > 1);

            DateTime requestTime = DateTime.UtcNow;
            await Task.Delay(420000);
            //await Task.Delay(TestConfig.WaitingTimeForQueueInSeconds * 3000);

            HttpResponseMessage callBackResponse = await EnsApiClient.GetEnsCallBackAsync(TestConfig.StubBaseUri, subscriptionId.ToUpper());

            Assert.That(200, Is.EqualTo((int)callBackResponse.StatusCode), $"Incorrect status code {callBackResponse.StatusCode}  is  returned, instead of the expected 200.");

            IEnumerable<EnsCallbackResponseModel> callBackResponseBody = JsonConvert.DeserializeObject<IEnumerable<EnsCallbackResponseModel>>(callBackResponse.Content.ReadAsStringAsync().Result);

            EnsCallbackResponseModel callBackResponseLatest = callBackResponseBody.Where(x => x.TimeStamp >= requestTime).OrderByDescending(a => a.TimeStamp).FirstOrDefault();

            Assert.That(TestConfig.FailedStatusCode, Is.EqualTo(callBackResponseLatest.CallBackRequest.Ukho_lastresponse), $"Invalid last response {callBackResponseLatest.CallBackRequest.Ukho_lastresponse} , Instead of expected last response {TestConfig.FailedStatusCode}.");
            Assert.That(callBackResponseLatest.CallBackRequest.Ukho_responsedetails.Contains("Failed to deliver notification therefore subscription marked as inactive"), $"Last Response : {callBackResponseLatest.CallBackRequest.Ukho_responsedetails}");
            Assert.That(callBackResponseLatest.TimeStamp <= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second), $"Response body returned timestamp date {callBackResponseLatest.TimeStamp} , greater than the expected value.");
        }
    }
}
