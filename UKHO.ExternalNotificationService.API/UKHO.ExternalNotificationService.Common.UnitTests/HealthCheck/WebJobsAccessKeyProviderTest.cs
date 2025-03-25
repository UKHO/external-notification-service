using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using UKHO.ExternalNotificationService.Common.HealthCheck;

namespace UKHO.ExternalNotificationService.Common.UnitTests.HealthCheck
{
    public class WebJobsAccessKeyProviderTest
    {
        private IConfiguration _configuration;
        private WebJobAccessKeyProvider _webJobsAccessKeyProvider;

        [SetUp]
        public void Setup()
        {
            Dictionary<string, string> inMemorySettings = new()
            {
                {"webjob1key", "webjob1value"},
                {"webjob2key", "webjob2value"}};

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _webJobsAccessKeyProvider = new WebJobAccessKeyProvider(_configuration);
        }

        [Test]
        public void GetWebJobAccessKey_ReturnsCorrectKeyWhenExists()
        {
            var actualAccessKey = _webJobsAccessKeyProvider.GetWebJobsAccessKey("webjob1key");
            var expectedAccessKey = _configuration.GetValue<string>("webjob1key");
            Assert.That(actualAccessKey, Is.EqualTo(expectedAccessKey));
        }

        [Test]
        public void GetWebJobAccessKey_ReturnsNullWhenNotExists()
        {
            var actualAccessKey = _webJobsAccessKeyProvider.GetWebJobsAccessKey("nonexistingkey");
            Assert.That(actualAccessKey, Is.Null);
        }
    }
}
