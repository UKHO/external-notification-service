using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;

namespace UKHO.ExternalNotificationService.Common.UnitTests.Helper
{
    [TestFixture]
    public class AzureEventGridDomainServiceTest
    {
        private IOptions<EventGridDomainConfiguration> _fakeEventGridDomainConfig;
        private ILogger<AzureEventGridDomainService> _fakeLogger;
        private AzureEventGridDomainService _azureEventGridDomainService;

        [SetUp]
       public void Setup()
        {
            _fakeEventGridDomainConfig = A.Fake<IOptions<EventGridDomainConfiguration>>();
            _fakeLogger = A.Fake<ILogger<AzureEventGridDomainService>>();

            _azureEventGridDomainService = new AzureEventGridDomainService(_fakeEventGridDomainConfig, _fakeLogger);
        }


        ////////[Test]
        ////////public async Task WhenCreateOrUpdateSubscriptionThenCreateSubscription()
        ////////{
        ////////    CancellationToken cancellationToken = CancellationToken.None;
        ////////    ///  A.CallTo(() => _azureEventGridDomainService.CreateOrUpdateSubscription(SubscriptionRequestMessage>.Ignored, A<CancellationToken>.Ignored));

        ////////    /// var response = await _azureEventGridDomainService.CreateOrUpdateSubscription(, cancellationToken);
        ////////    Assert.IsInstanceOf<string>(response);
        ////////}
    }
}
