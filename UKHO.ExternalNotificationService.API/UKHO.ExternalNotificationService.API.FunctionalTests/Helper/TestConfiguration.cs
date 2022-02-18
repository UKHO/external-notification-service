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
           
        }
    }
}
