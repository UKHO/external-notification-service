using Azure.Messaging;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using UKHO.ExternalNotificationService.Common.BaseClass;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using Attribute = UKHO.ExternalNotificationService.Common.Models.EventModel.Attribute;
using File = UKHO.ExternalNotificationService.Common.Models.EventModel.File;

namespace UKHO.ExternalNotificationService.Common.UnitTests.BaseClass
{
    [TestFixture]
    public class EventProcessorBaseTest
    {
        private IAzureEventGridDomainService _fakeAzureEventGridDomainService;
        private EventProcessorBase _eventProcessorBase;
        private FssEventData _fssEventData;
        public const string correlationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeAzureEventGridDomainService = A.Fake<IAzureEventGridDomainService>();
            _fssEventData = GetFssEventData();

            _eventProcessorBase = new EventProcessorBase(_fakeAzureEventGridDomainService);
        }

        [Test]
        public void WhenPostFssValidEventRequest_ThenReturnsFssEventData()
        {
            A.CallTo(() => _fakeAzureEventGridDomainService.JsonDeserialize<FssEventData>(A<object>.Ignored)).Returns(_fssEventData);
            object data = (object)_fssEventData;

            FssEventData response =  _eventProcessorBase.GetEventData<FssEventData>(data);

            Assert.AreEqual(_fssEventData.BatchId, response.BatchId);
            Assert.AreEqual(_fssEventData.Links.BatchDetails, response.Links.BatchDetails);
        }

        [Test]
        public void WhenPostFssValidEventRequest_ThenEventPublishedSuccessfully()
        {
            CancellationToken cancellationToken = CancellationToken.None;
            CloudEvent cloudEvent = new("test", "test", new object());

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(cloudEvent, correlationId, cancellationToken));

            var response = _eventProcessorBase.PublishEventAsync(cloudEvent, correlationId, cancellationToken);

            Assert.IsTrue(response.IsCompleted);
        }

        private static FssEventData GetFssEventData()
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
                BusinessUnit = "AVCSData",
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
    }
}
