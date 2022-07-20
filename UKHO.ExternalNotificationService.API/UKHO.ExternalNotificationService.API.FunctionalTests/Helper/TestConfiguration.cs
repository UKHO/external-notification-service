using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Helper
{
    public class TestConfiguration
    {
        protected IConfigurationRoot ConfigurationRoot;
        public string EnsApiBaseUrl { get; set; }
        public string PayloadFolder { get; set; }
        public string FssPayloadFileName { get; set; }
        public string ScsPayloadFileName { get; set; }
        public string EnsStorageConnectionString { get; set; }
        public string EnsStorageQueueName { get; set; }
        public int WaitingTimeForQueueInSeconds { get; set; }
        public string MicrosoftOnlineLoginUrl { get; set; }
        public string D365TenantId { get; set; }
        public string D365ClientId { get; set; }
        public string D365Secret { get; set; }
        public string ClientId { get; set; }
        public string StubApiUri { get; set; }
        public ICollection<SourceConfiguration> FssSources { get; set; }
        public string FssEventHostName { get; set; }
        public string FssPublishHostName { get; set; }
        public int SucceededStatusCode { get; set; }
        public int FailedStatusCode { get; set; }
        public string StubBaseUri { get; set; }
        public string ScsSource { get; set; }
        public string WebhookUrlExtension { get; set; }

        public TestConfiguration()
        {
            ConfigurationRoot = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false)
                                .Build();

            EnsApiBaseUrl = ConfigurationRoot.GetSection("EnsApiUrl").Value;
            PayloadFolder = ConfigurationRoot.GetSection("PayloadFolder").Value;
            FssPayloadFileName = ConfigurationRoot.GetSection("FssPayloadFileName").Value;
            ScsPayloadFileName = ConfigurationRoot.GetSection("ScsPayloadFileName").Value;
            EnsStorageConnectionString = ConfigurationRoot.GetSection("EnsStorageConnectionString").Value;
            EnsStorageQueueName = ConfigurationRoot.GetSection("EnsStorageQueueName").Value;
            WaitingTimeForQueueInSeconds = int.Parse(ConfigurationRoot.GetSection("WaitingTimeForQueueInSeconds").Value);
            MicrosoftOnlineLoginUrl = ConfigurationRoot.GetSection("D365AuthConfiguration:MicrosoftOnlineLoginUrl").Value;
            D365TenantId = ConfigurationRoot.GetSection("D365AuthConfiguration:TenantId").Value;
            D365ClientId = ConfigurationRoot.GetSection("D365AuthConfiguration:ClientId").Value;
            D365Secret = ConfigurationRoot.GetSection("D365AuthConfiguration:ClientSecret").Value;
            ClientId = ConfigurationRoot.GetSection("EnsAuthConfiguration:ClientId").Value;
            StubApiUri = ConfigurationRoot.GetSection("StubConfiguration:BaseUri").Value;
            FssSources = JsonSerializer.Deserialize<ICollection<SourceConfiguration>>(ConfigurationRoot.GetSection("FssDataMappingConfiguration:Sources").Value);
            FssEventHostName = ConfigurationRoot.GetSection("FssDataMappingConfiguration:EventHostName").Value;
            FssPublishHostName = ConfigurationRoot.GetSection("FssDataMappingConfiguration:PublishHostName").Value;
            SucceededStatusCode=int.Parse(ConfigurationRoot.GetSection("D365CallbackConfiguration:SucceededStatusCode").Value);
            FailedStatusCode= int.Parse(ConfigurationRoot.GetSection("D365CallbackConfiguration:FailedStatusCode").Value);
            StubBaseUri = ConfigurationRoot.GetSection("StubConfiguration:BaseUri").Value;
            ScsSource = ConfigurationRoot.GetSection("ScsDataMappingConfiguration:Source").Value;
            WebhookUrlExtension = ConfigurationRoot.GetSection("WebhookUrlExtension").Value;
        }

        public class SourceConfiguration
        {
            public string BusinessUnit { get; set; }
            public string Source { get; set; }
        }
    }
}
