﻿using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;
using FakeItEasy;
using NUnit.Framework;
using UKHO.ExternalNotificationService.Common.BaseClass;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.EventModel;

namespace UKHO.ExternalNotificationService.Common.UnitTests.BaseClass
{
    [TestFixture]
    public class EventProcessorBaseTest
    {
        private IAzureEventGridDomainService _fakeAzureEventGridDomainService;
        private EventProcessorBase _eventProcessorBase;
        private FssEventData _fssEventData;
        public const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeAzureEventGridDomainService = A.Fake<IAzureEventGridDomainService>();
            _fssEventData = CustomCloudEventBase.GetFssEventData();

            _eventProcessorBase = new EventProcessorBase(_fakeAzureEventGridDomainService);
        }

        [Test]
        public void WhenPostFssValidEventRequest_ThenReturnsFssEventData()
        {
            A.CallTo(() => _fakeAzureEventGridDomainService.JsonDeserialize<FssEventData>(A<object>.Ignored)).Returns(_fssEventData);
            object data = (object)_fssEventData;

            FssEventData response =  _eventProcessorBase.GetEventData<FssEventData>(data);

            Assert.That(_fssEventData.BatchId, Is.EqualTo(response.BatchId));
            Assert.That(_fssEventData.Links.BatchDetails, Is.EqualTo(response.Links.BatchDetails));
        }

        [Test]
        public void WhenPostFssValidEventRequest_ThenEventPublishedSuccessfully()
        {
            CancellationToken cancellationToken = CancellationToken.None;
            CloudEvent cloudEvent = new("test", "test", new object());

            A.CallTo(() => _fakeAzureEventGridDomainService.PublishEventAsync(cloudEvent, CorrelationId, cancellationToken));

            Task response = _eventProcessorBase.PublishEventAsync(cloudEvent, CorrelationId, cancellationToken);

            Assert.That(response.IsCompleted);
        }
    }
}
