using Microsoft.Extensions.Configuration;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Helper
{
    public class TestConfiguration
    {
        protected IConfigurationRoot ConfigurationRoot;
        public string ensApiBaseUrl;
        public string payloadFolder;
        public string payloadFileName;



        public TestConfiguration()
        {
            ConfigurationRoot = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false)
                                .Build();

            ensApiBaseUrl = ConfigurationRoot.GetSection("EnsApiBaseUrl").Value;
            payloadFolder = ConfigurationRoot.GetSection("PayloadFolder").Value;
            payloadFileName = ConfigurationRoot.GetSection("PayloadFileName").Value;
            
        }
    }
}
