
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Storage;

namespace UKHO.ExternalNotificationService.Common.UnitTests.Storage
{
    [TestFixture]
    public class StorageServiceTest
    {
        private IOptions<SubscriptionStorageConfiguration> _fakeSubscriptionStorageConfiguration;
        private StorageService _storageService;

        [SetUp]
        public void Setup()
        {
            _fakeSubscriptionStorageConfiguration = Options.Create(new SubscriptionStorageConfiguration()
            {
                QueueName = "",
                StorageAccountKey = "",
                StorageAccountName = "test",
                StorageContainerName = "test",
            });

            _storageService = new StorageService(_fakeSubscriptionStorageConfiguration);
        }

        [Test]
        public void WhenValidStorageConfiguration_ThenValidStorageAccountConnectionStringRequest()
        {
            _fakeSubscriptionStorageConfiguration.Value.StorageAccountKey = "Test";
            string response = _storageService.GetStorageAccountConnectionString();

            Assert.NotNull(response);
            Assert.AreEqual("DefaultEndpointsProtocol=https;AccountName=test;AccountKey=Test;EndpointSuffix=core.windows.net", response);
        }
    }
}
