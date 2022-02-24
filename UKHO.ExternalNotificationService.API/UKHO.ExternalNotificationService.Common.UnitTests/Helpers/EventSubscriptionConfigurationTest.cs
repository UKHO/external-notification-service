using FakeItEasy;
using Microsoft.Azure.Management.EventGrid.Models;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.Common.UnitTests.Helpers
{
    [TestFixture]
    public class EventSubscriptionConfigurationTest
    {
        private IOptions<EventGridDomainConfiguration> _fakeEventGridDomainConfig;
        private IOptions<SubscriptionStorageConfiguration> _fakeSubscriptionStorageConfiguration;
        private EventSubscriptionConfiguration _eventSubscriptionConfiguration;

        [SetUp]
        public void Setup()
        {
            _fakeEventGridDomainConfig = A.Fake<IOptions<EventGridDomainConfiguration>>();
            _fakeSubscriptionStorageConfiguration = A.Fake<IOptions<SubscriptionStorageConfiguration>>();
            _fakeEventGridDomainConfig.Value.MaxDeliveryAttempts = 10;
            _fakeEventGridDomainConfig.Value.EventTimeToLiveInMinutes = 10;
            _fakeEventGridDomainConfig.Value.SubscriptionId = "EventDomainSubscriptionId";
            _fakeEventGridDomainConfig.Value.ResourceGroup = "rg";
            _fakeSubscriptionStorageConfiguration.Value.StorageAccountName = "StorageAcountName";
            _fakeSubscriptionStorageConfiguration.Value.StorageContainerName = "StorageContainerName";

            _eventSubscriptionConfiguration = new EventSubscriptionConfiguration(_fakeEventGridDomainConfig, _fakeSubscriptionStorageConfiguration);
        }

        [Test]
        public void WhenCallSetEventSubscription_ThenReturnEventSubscription()
        {
            SubscriptionRequestMessage subscriptionRequestMessage = new() { 
                CorrelationId = Guid.NewGuid().ToString(),
                D365CorrelationId = Guid.NewGuid().ToString(),
                IsActive = true,
                NotificationType = "ADDS Data Pipeline",
                NotificationTypeTopicName = "avcs-contentPublished",
                SubscriptionId = Guid.NewGuid().ToString(),
                WebhookUrl = "https://abc.com/"
            };

            EventSubscription result = _eventSubscriptionConfiguration.SetEventSubscription(subscriptionRequestMessage);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<EventSubscription>(result);
            Assert.IsInstanceOf<EventSubscriptionDestination>(result.Destination);
            Assert.IsInstanceOf<RetryPolicy>(result.RetryPolicy);
            Assert.AreEqual(_fakeEventGridDomainConfig.Value.MaxDeliveryAttempts, result.RetryPolicy.MaxDeliveryAttempts);
            Assert.AreEqual(_fakeEventGridDomainConfig.Value.EventTimeToLiveInMinutes, result.RetryPolicy.EventTimeToLiveInMinutes);
            Assert.IsInstanceOf<DeadLetterDestination>(result.DeadLetterDestination);
            Assert.AreEqual(EventDeliverySchema.CloudEventSchemaV10, result.EventDeliverySchema);
        }
    }
}
