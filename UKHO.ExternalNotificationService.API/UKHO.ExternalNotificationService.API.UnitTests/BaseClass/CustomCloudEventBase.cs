using System;
using System.Collections.Generic;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;
using Attribute = UKHO.ExternalNotificationService.Common.Models.EventModel.Attribute;
using File = UKHO.ExternalNotificationService.Common.Models.EventModel.File;

namespace UKHO.ExternalNotificationService.API.UnitTests.BaseClass
{
    public static class CustomCloudEventBase
    {
        public static CustomCloudEvent GetCustomCloudEvent()
        {
            return new CustomCloudEvent
            {
                Type = "uk.gov.UKHO.FileShareService.NewFilesPublished.v1",
                Source = "https://files.admiralty.co.uk",
                Id = "49c67cca-9cca-4655-a38e-583693af55ea",
                Subject = "83d08093-7a67-4b3a-b431-92ba42feaea0",
                DataContentType = "application/json",
                Data = GetFssEventData(),
                Time = "2021-11-09T14:52:28+00:00"
            };
        }

        public static FssEventData GetFssEventData()
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
                Get = new Link() { Href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/AVCS_S631-1_Update_Wk45_21_Only.zip" },
            };

            BatchLinks links = new()
            {
                BatchDetails = linkBatchDetails,
                BatchStatus = linkBatchStatus
            };

            return new FssEventData
            {
                Links = links,
                BusinessUnit = "AVCSData",
                Attributes = new List<Attribute>(),
                BatchId = "83d08093-7a67-4b3a-b431-92ba42feaea0",
                BatchPublishedDate = DateTime.UtcNow,
                Files = new File[] {
                    new() {
                        MIMEType= "application/zip",
                        FileName= "AVCS_S631-1_Update_Wk45_21_Only.zip",
                        FileSize=99073923,
                        Hash="yNpJTWFKhD3iasV8B/ePKw==",
                        Attributes=new List<Attribute>(),
                        Links = fileLinks
                    }
                }
            };
        }

        public static ScsEventData GetScsEventData()
        {
            return new ScsEventData
            {
                ProductType = "ENC S57",
                ProductName = "GB53496A",
                EditionNumber = 15,
                UpdateNumber = 18,
                BoundingBox = new ProductBoundingBox
                {
                    NorthLimit = 53.7485,
                    SouthLimit = 53.7197583,
                    EastLimit = -0.2199796,
                    WestLimit = -0.3640183
                },
                Status = new ProductUpdateStatus
                {
                    StatusDate = DateTime.UtcNow,
                    IsNewCell = false,
                    StatusName = "Update"
                },
                FileSize = 1584,
                IsPermitUpdateRequired = false
            };
        }
    }
}
