
using Newtonsoft.Json.Linq;
using System;
using UKHO.ExternalNotificationService.Common.Models.EventModel;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Helper
{
    public static class ScsEventDataBase
    {
        public static JObject GetScsEventBodyData(TestConfiguration testConfigure)
        {
            var ensWebhookJson = JObject.Parse(@"{""Type"":""uk.gov.UKHO.catalogue.productUpdated.v1""}");
            ensWebhookJson["Source"] = $"{testConfigure.ScsSource}";
            ensWebhookJson["Id"] = "0d2f05f5-3691-476a-9011-6007bcaa9cbf";
            ensWebhookJson["Subject"] = "GB53496A";
            ensWebhookJson["DataContentType"] = "application/json";
            ensWebhookJson["Data"] = JObject.FromObject(GetScsEventData());

            return ensWebhookJson;
        }

        private static ScsEventData GetScsEventData()
        {
            return new ScsEventData()
            {
                ProductType = "ENC S57",
                ProductName = "GB53496A",
                EditionNumber = 15,
                UpdateNumber = 18,
                BoundingBox = new ProductBoundingBox {
                                    NorthLimit = 53.7485,
                                    SouthLimit = 53.7197583,
                                    EastLimit = -0.2199796,
                                    WestLimit= -0.3640183
                },
                Status = new ProductUpdateStatus {
                                    StatusDate = DateTime.UtcNow,
                                    IsNewCell = false,
                                    StatusName = "Update" },
                IsPermitUpdateRequired = false,
                FileSize = 1584,
            };
        }
    }
}
