﻿
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
            ensWebhookJson["Source"] = $"https://{testConfigure.ScsEventHostName}";
            ensWebhookJson["Id"] = "0d2f05f5-3691-476a-9011-6007bcaa9cbf";
            ensWebhookJson["Subject"] = "NO4F1615";
            ensWebhookJson["DataContentType"] = "application/json";
            ensWebhookJson["Data"] = JObject.FromObject(GetScsEventData(testConfigure));

            return ensWebhookJson;
        }

        public static ScsEventData GetScsEventData(TestConfiguration testConfigure)
        {
            return new ScsEventData()
            {
                ProductType = "ENC S57",
                ProductName = "NO4F1615",
                EditionNumber = 15,
                UpdateNumber = 18,
                BoundingBox =
                {
                    NorthLimit =63.25,
                    SouthLimit =63,
                    EastLimit =8,
                    WestLimit= 7.75

                },
                Status =
                {
                    StatusDate = DateTime.UtcNow,
                    IsNewCell = false,
                    StatusName = "Update",
                },
                IsPermitUpdateRequired = false,
                FileSize = 1584,
            };
        }
    }
}
