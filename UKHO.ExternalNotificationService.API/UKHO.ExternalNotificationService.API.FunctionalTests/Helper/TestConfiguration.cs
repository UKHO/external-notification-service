using Microsoft.Extensions.Configuration;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Helper
{
    public class TestConfiguration
    {
        protected IConfigurationRoot ConfigurationRoot;
        public string EnsApiBaseUrl { get; set; }
        public string PayloadFolder { get; set; }
        public string PayloadFileName { get; set; }
        public string EnsStorageConnectionString { get; set; }
        public string EnsStorageQueueName { get; set; }
        public int WaitingTimeForQueueInSeconds { get; set; }
        public int WaitingTimeForTopicCreationInSeconds { get; set; }
        public EventGridDomainConfiguration EventGridDomainConfig { get; set; }


        public class EventGridDomainConfiguration
        {
            public string SubscriptionId { get; set; }
            public string ResourceGroup { get; set; }
            public string EventGridDomainName { get; set; }
            public string NotificationTypeTopicName { get; set; }

        }

        public TestConfiguration()
        {
            ConfigurationRoot = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false)
                                .Build();

            EnsApiBaseUrl = ConfigurationRoot.GetSection("EnsApiUrl").Value;
            PayloadFolder = ConfigurationRoot.GetSection("PayloadFolder").Value;
            PayloadFileName = ConfigurationRoot.GetSection("PayloadFileName").Value;
            EnsStorageConnectionString = ConfigurationRoot.GetSection("EnsStorageConnectionString").Value;
            EnsStorageQueueName = ConfigurationRoot.GetSection("EnsStorageQueueName").Value;
            WaitingTimeForQueueInSeconds = int.Parse(ConfigurationRoot.GetSection("WaitingTimeForQueueInSeconds").Value);
            WaitingTimeForTopicCreationInSeconds = int.Parse(ConfigurationRoot.GetSection("WaitingTimeForTopicCreationInSeconds").Value);
            EventGridDomainConfig = new();
            EventGridDomainConfig.SubscriptionId = ConfigurationRoot.GetSection("EventGridDomainConfiguration:SubscriptionId").Value;
            EventGridDomainConfig.ResourceGroup = ConfigurationRoot.GetSection("EventGridDomainConfiguration:ResourceGroup").Value;
            EventGridDomainConfig.EventGridDomainName = ConfigurationRoot.GetSection("EventGridDomainConfiguration:EventGridDomainName").Value;
            EventGridDomainConfig.NotificationTypeTopicName = ConfigurationRoot.GetSection("EventGridDomainConfiguration:NotificationTypeTopicName").Value;

        }
    }
}
