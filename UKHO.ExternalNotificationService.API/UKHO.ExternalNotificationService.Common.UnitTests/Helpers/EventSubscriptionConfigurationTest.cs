using FakeItEasy;
using Microsoft.Azure.Management.EventGrid.Models;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;

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
        public void WhenCallSetWebHookEventSubscriptionDestination_ThenReturnWebHookEventSubscriptionDestination()
        {
            string uri = "https://abc.com/";
            WebHookEventSubscriptionDestination result = _eventSubscriptionConfiguration.SetWebHookEventSubscriptionDestination(uri);
            Assert.IsInstanceOf<WebHookEventSubscriptionDestination>(result);
            Assert.IsNotNull(result);
            bool assertUriResult = Uri.TryCreate(result.EndpointUrl, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            Assert.IsTrue(assertUriResult);
            Assert.AreEqual(uri, uriResult.AbsoluteUri);
        }

        [Test]
        public void WhenCallSetEventDeliverySchema_ThenReturnEventDeliverySchema()
        {
            string result = _eventSubscriptionConfiguration.SetEventDeliverySchema;
            Assert.IsInstanceOf<string>(result);
            Assert.AreEqual(EventDeliverySchema.CloudEventSchemaV10, result);
            Assert.IsNotEmpty(EventDeliverySchema.EventGridSchema, result);
        }

        [Test]
        public void WhenCallSetSetRetryPolicy_ThenReturnRetryPolicy()
        {
            RetryPolicy result = _eventSubscriptionConfiguration.SetRetryPolicy();
            Assert.IsInstanceOf<RetryPolicy>(result);
            Assert.AreEqual(true, result.MaxDeliveryAttempts.HasValue);
            Assert.AreEqual(true, result.EventTimeToLiveInMinutes.HasValue);
        }

        [Test]
        public void WhenCallSetStorageBlobDeadLetterDestination_ThenStorageBlobDeadLetterDestination()
        {
            string deadLetterDestinationResourceId = $"/subscriptions/{_fakeEventGridDomainConfig.Value.SubscriptionId}/resourceGroups/{_fakeEventGridDomainConfig.Value.ResourceGroup}/providers/Microsoft.Storage/storageAccounts/{_fakeSubscriptionStorageConfiguration.Value.StorageAccountName}";
            StorageBlobDeadLetterDestination result = _eventSubscriptionConfiguration.SetStorageBlobDeadLetterDestination();
            Assert.IsInstanceOf<StorageBlobDeadLetterDestination>(result);
            Assert.AreEqual(_fakeSubscriptionStorageConfiguration.Value.StorageContainerName, result.BlobContainerName);
            Assert.AreEqual(deadLetterDestinationResourceId, result.ResourceId);
        }
    }
}
