﻿using System;
using System.Collections.Generic;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;
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
                Get = new Link() { Href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/AVCS_S631-1_Update_Wk45_21_Only.zip" },
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
                Files = new File[] {new() { MIMEType= "application/zip",
                                                                    FileName= "AVCS_S631-1_Update_Wk45_21_Only.zip",
                                                                    FileSize=99073923,
                                                                    Hash="yNpJTWFKhD3iasV8B/ePKw==",
                                                                    Attributes=new List<Attribute> {},
                                                                    Links = fileLinks   }}
            };
        }

        public static ScsEventData GetScsEventData()
        {
            return new ScsEventData()
            {
                ProductType = "ENC S57",
                ProductName = "NO4F1615",
                EditionNumber = 15,
                UpdateNumber = 18,
                BoundingBox = new ProductBoundingBox()
                {
                    NorthLimit = 63.25,
                    SouthLimit = 63,
                    EastLimit = 8,
                    WestLimit = 7.75
                },
                Status = new ProductUpdateStatus()
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
