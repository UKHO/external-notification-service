using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UKHO.ExternalNotificationService.API.FunctionalTests.Model;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Helper
{
    public static class FssEventDataBase
    {
        public static JObject GetFssEventBodyData(TestConfiguration testConfigure, string businessUnit = "AVCSData")
        {
            var ensWebhookJson = JObject.Parse(@"{""Type"":""uk.gov.UKHO.FileShareService.NewFilesPublished.v1""}");
            ensWebhookJson["Source"] = $"https://{testConfigure.FssEventHostName}";
            ensWebhookJson["Id"] = "49c67cca-9cca-4655-a38e-583693af55ea";
            ensWebhookJson["Subject"] = "83d08093-7a67-4b3a-b431-92ba42feaea0";
            ensWebhookJson["DataContentType"] = "application/json";
            ensWebhookJson["Data"] = JObject.FromObject(GetFssEventData(testConfigure, businessUnit));

            return ensWebhookJson;
        }
        public static FssEventData GetFssEventData(TestConfiguration testConfigure, string businessUnit)
        {
            Link linkBatchDetails = new()
            {
                Href = $"https://{testConfigure.FssEventHostName}/batch/83d08093-7a67-4b3a-b431-92ba42feaea0"
            };
            Link linkBatchStatus = new()
            {
                Href = $"https://{testConfigure.FssEventHostName}/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/status"
            };

            FileLinks fileLinks = new()
            {
                Get = new Link() { Href = $"https://{testConfigure.FssEventHostName}/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/AVCS_S631-1_Update_Wk45_21_Only.zip" },
            };

            BatchLinks links = new()
            {
                BatchDetails = linkBatchDetails,
                BatchStatus = linkBatchStatus
            };

            return new FssEventData()
            {
                Links = links,
                BusinessUnit = businessUnit,
                Attributes = new List<Attribute> { },
                BatchId = "83d08093-7a67-4b3a-b431-92ba42feaea0",
                BatchPublishedDate = DateTime.UtcNow,
                Files = new BatchFile[] {new() { MIMEType= "application/zip",
                                                                    FileName= "AVCS_S631-1_Update_Wk45_21_Only.zip",
                                                                    FileSize=99073923,
                                                                    Hash="yNpJTWFKhD3iasV8B/ePKw==",
                                                                    Attributes=new List<Attribute> {},
                                                                    Links = fileLinks   }}
            };
        }
    }
}
