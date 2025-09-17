
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using UKHO.ExternalNotificationService.Common.Models.EventModel;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Helper
{
    public static class ScsEventDataBase
    {
        public static JsonObject GetScsEventBodyData(TestConfiguration testConfigure)
        {
            JsonNode ensWebhookJson = JsonObject.Parse(@"{""type"":""uk.gov.UKHO.catalogue.productUpdated.v1""}");
            ensWebhookJson["source"] = $"{testConfigure.ScsSource}";
            ensWebhookJson["id"] = "0d2f05f5-3691-476a-9011-6007bcaa9cbf";
            ensWebhookJson["subject"] = "GB53496A";
            ensWebhookJson["dataContentType"] = "application/json";
            ensWebhookJson["data"] = JsonObject.Parse(JsonSerializer.Serialize(GetScsEventData()));

            return (JsonObject)ensWebhookJson;
        }

        public static JsonObject GetScsS100EventBodyData(TestConfiguration testConfigure)
        {
            JsonNode ensWebhookJson = JsonObject.Parse(@"{""type"":""uk.gov.ukho.catalogue.s100ProductUpdated.v1""}");
            ensWebhookJson["source"] = $"{testConfigure.ScsS100Source}";
            ensWebhookJson["id"] = "0d2f05f5-3691-476a-9011-6007bcaa9cbf";
            ensWebhookJson["subject"] = "GB53496A";
            ensWebhookJson["dataContentType"] = "application/json";
            ensWebhookJson["data"] = JsonObject.Parse(JsonSerializer.Serialize(GetScsEventData("ENC S100")));

            return (JsonObject)ensWebhookJson;
        }

        private static ScsEventData GetScsEventData(string productType = "ENC S57")
        {
            return new ScsEventData()
            {
                ProductType = "ENC S100",
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
