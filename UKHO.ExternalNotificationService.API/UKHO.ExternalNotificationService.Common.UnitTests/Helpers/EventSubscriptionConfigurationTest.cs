using System;
using Azure.ResourceManager.EventGrid;
using Azure.ResourceManager.EventGrid.Models;
using FakeItEasy;
using Microsoft.Extensions.Options;
using NUnit.Framework;
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
        private const string SubscriptionId = "EventDomainSubscriptionId";
        private const string ResourceGroup = "rg";
        private const string StorageAccountName = "StorageAccountName";
        private const string StorageContainerName = "StorageContainerName";
        private const string WebhookUrl = "https://abc.com/";
        private const string ExpectedDeadLetterDestinationResourceId = $"/subscriptions/{SubscriptionId}/resourceGroups/{ResourceGroup}/providers/Microsoft.Storage/storageAccounts/{StorageAccountName}";

        [SetUp]
        public void Setup()
        {
            _fakeEventGridDomainConfig = A.Fake<IOptions<EventGridDomainConfiguration>>();
            _fakeSubscriptionStorageConfiguration = A.Fake<IOptions<SubscriptionStorageConfiguration>>();
            _fakeEventGridDomainConfig.Value.MaxDeliveryAttempts = 10;
            _fakeEventGridDomainConfig.Value.EventTimeToLiveInMinutes = 10;
            _fakeEventGridDomainConfig.Value.SubscriptionId = SubscriptionId;
            _fakeEventGridDomainConfig.Value.ResourceGroup = ResourceGroup;
            _fakeSubscriptionStorageConfiguration.Value.StorageAccountName = StorageAccountName;
            _fakeSubscriptionStorageConfiguration.Value.StorageContainerName = StorageContainerName;
            _eventSubscriptionConfiguration = new EventSubscriptionConfiguration(_fakeEventGridDomainConfig, _fakeSubscriptionStorageConfiguration);
        }

        [Test]
        public void WhenCallSetEventSubscription_ThenReturnEventSubscription()
        {
            var subscriptionRequestMessage = new SubscriptionRequestMessage
            {
                CorrelationId = Guid.NewGuid().ToString(),
                D365CorrelationId = Guid.NewGuid().ToString(),
                IsActive = true,
                NotificationType = "ADDS Data Pipeline",
                NotificationTypeTopicName = "avcs-contentPublished",
                SubscriptionId = Guid.NewGuid().ToString(),
                WebhookUrl = WebhookUrl
            };

            var result = _eventSubscriptionConfiguration.SetEventSubscription(subscriptionRequestMessage);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<EventGridSubscriptionData>());
            Assert.Multiple(() =>
            {
                Assert.That(result.Destination, Is.InstanceOf<EventSubscriptionDestination>());
                Assert.That(((WebHookEventSubscriptionDestination)result.Destination).Endpoint.AbsoluteUri, Is.EqualTo(WebhookUrl));
                Assert.That(result.EventDeliverySchema, Is.EqualTo(EventDeliverySchema.CloudEventSchemaV1_0));
                Assert.That(result.RetryPolicy, Is.InstanceOf<EventSubscriptionRetryPolicy>());
                Assert.That(result.RetryPolicy.MaxDeliveryAttempts, Is.EqualTo(_fakeEventGridDomainConfig.Value.MaxDeliveryAttempts));
                Assert.That(result.RetryPolicy.EventTimeToLiveInMinutes, Is.EqualTo(_fakeEventGridDomainConfig.Value.EventTimeToLiveInMinutes));
                Assert.That(result.DeadLetterDestination, Is.InstanceOf<DeadLetterDestination>());
                Assert.That(((StorageBlobDeadLetterDestination)result.DeadLetterDestination).ResourceId.ToString(), Is.EqualTo(ExpectedDeadLetterDestinationResourceId));
                Assert.That(((StorageBlobDeadLetterDestination)result.DeadLetterDestination).BlobContainerName, Is.EqualTo(StorageContainerName));
            });
        }
    }
}
