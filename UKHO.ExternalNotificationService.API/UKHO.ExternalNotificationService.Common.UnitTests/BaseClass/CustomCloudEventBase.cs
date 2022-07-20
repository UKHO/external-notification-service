using System;
using System.Collections.Generic;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using Attribute = UKHO.ExternalNotificationService.Common.Models.EventModel.Attribute;
using File = UKHO.ExternalNotificationService.Common.Models.EventModel.File;

namespace UKHO.ExternalNotificationService.Common.UnitTests.BaseClass
{
    public static class CustomCloudEventBase
    {
        public static FssEventData GetFssEventData(string businessUnit = "AVCSData")
        {
            Link linkBatchDetails = new()
            {
                Href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0"
            };

            Link linkBatchStatus = new()
            {
                Href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/status"
            };

            FileLinks fileLinks = new()
            {
                Get = new Link { Href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/AVCS_S631-1_Update_Wk45_21_Only.zip" },
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
                Attributes = new List<Attribute>(),
                BatchId = "83d08093-7a67-4b3a-b431-92ba42feaea0",
                BatchPublishedDate = DateTime.UtcNow,
                Files = new File[]
                {
                    new()
                    {
                        MIMEType = "application/zip",
                        FileName = "AVCS_S631-1_Update_Wk45_21_Only.zip",
                        FileSize = 99073923,
                        Hash = "yNpJTWFKhD3iasV8B/ePKw==",
                        Attributes = new List<Attribute>(),
                        Links = fileLinks
                    }
                }
            };
        }
    }
}
