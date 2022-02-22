using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
        private string EnsToken { get; set; }   

        [SetUp]
        public async Task SetupAsync()
        {
            TestConfig = new TestConfiguration();
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
            HttpResponseMessage apiResponse = await EnsApiClient.OptionEnsApiSubscriptionAsync(requestHeader,requestHeaderValue, EnsToken);  

            Assert.AreEqual(200, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");

            Assert.IsTrue(apiResponse.Headers.Contains("WebHook-Allowed-Origin"));

            Assert.AreEqual(requestHeaderValue,apiResponse.Headers.GetValues("WebHook-Allowed-Origin").FirstOrDefault());

        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidJObjectBodyWithoutAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            var ensWebhookJson = JObject.Parse(@"{""Type"":""uk.gov.UKHO.FileShareService.NewFilesPublished.v1""}");
            ensWebhookJson["Source"] = "https://files.admiralty.co.uk";
            ensWebhookJson["Id"] = "49c67cca-9cca-4655-a38e-583693af55ea";
            ensWebhookJson["Subject"] = "83d08093-7a67-4b3a-b431-92ba42feaea0";
            ensWebhookJson["DataContentType"] = "application/json";
            ensWebhookJson["Data"] = JObject.FromObject(GetEnterpriseEventServiceRequestData());

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebookNewEventPublishedAsync(ensWebhookJson);

            Assert.AreEqual(401, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 401.");

        }

        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidJObjectBodyWitInvalidAuthToken_ThenAnUnauthorisedResponseIsReturned()
        {
            string invalidToken = EnsToken.Remove(EnsToken.Length - 4).Insert(EnsToken.Length - 4, "ABAA");
            var ensWebhookJson = JObject.Parse(@"{""Type"":""uk.gov.UKHO.FileShareService.NewFilesPublished.v1""}");
            ensWebhookJson["Source"] = "https://files.admiralty.co.uk";
            ensWebhookJson["Id"] = "49c67cca-9cca-4655-a38e-583693af55ea";
            ensWebhookJson["Subject"] = "83d08093-7a67-4b3a-b431-92ba42feaea0";
            ensWebhookJson["DataContentType"] = "application/json";
            ensWebhookJson["Data"] = JObject.FromObject(GetEnterpriseEventServiceRequestData());

            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebookNewEventPublishedAsync(ensWebhookJson, invalidToken);

            Assert.AreEqual(401, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 401.");

        }


        [Test]
        public async Task WhenICallTheEnsWebhookApiWithAValidJObjectBody_ThenOkStatusIsReturned()
        {
            var ensWebhookJson = JObject.Parse(@"{""Type"":""uk.gov.UKHO.FileShareService.NewFilesPublished.v1""}");
            ensWebhookJson["Source"] = "https://files.admiralty.co.uk";
            ensWebhookJson["Id"] = "49c67cca-9cca-4655-a38e-583693af55ea";
            ensWebhookJson["Subject"] = "83d08093-7a67-4b3a-b431-92ba42feaea0";
            ensWebhookJson["DataContentType"] = "application/json";
            ensWebhookJson["Data"]= JObject.FromObject(GetEnterpriseEventServiceRequestData());


            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsWebookNewEventPublishedAsync(ensWebhookJson, EnsToken);
           
            Assert.AreEqual(200, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode} is returned, instead of the expected 200.");

        }

        private EnterpriseEventServiceDataRequest GetEnterpriseEventServiceRequestData()
        {
            BatchDetails linkBatchDetails = new()
            {
                href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0"
            };
            BatchStatus linkBatchStatus = new()
            {
                href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/status"
            };
            GetUrl linkGet = new()
            {
                href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/AVCS_S631-1_Update_Wk45_21_Only.zip",
            };
            Links links = new()
            {
                batchDetails = linkBatchDetails,
                batchStatus = linkBatchStatus,
                getUrl = linkGet
            };
            return new EnterpriseEventServiceDataRequest

            {
                links = links,
                businessUnit = "AVCSData",
                batchAttributes = new List<BatchAttribute> { new BatchAttribute { key= "Exchange Set Type", value= "Update" } ,
                                                           new BatchAttribute { key= "Media Type", value= "Zip" },
                                                           new BatchAttribute { key= "Product Type", value= "AVCS" } ,
                                                           new BatchAttribute { key= "S63 Version", value= "1.2" },
                                                           new BatchAttribute { key= "Week Number", value= "45" },
                                                           new BatchAttribute { key = "Year", value = "2021" }},
                batchId = "83d08093-7a67-4b3a-b431-92ba42feaea0",
                batchPublishedDate = DateTime.UtcNow,
                files =new List<BatchFile> { new() { mimeType= "application/zip",
                    filename= "AVCS_S631-1_Update_Wk45_21_Only.zip",
                    fileSize=99073923,
                    hash="yNpJTWFKhD3iasV8B/ePKw==",
                    attributes=new List<BatchAttribute> { },
                    links = links
                }
                }
            };
        }
    }
}
