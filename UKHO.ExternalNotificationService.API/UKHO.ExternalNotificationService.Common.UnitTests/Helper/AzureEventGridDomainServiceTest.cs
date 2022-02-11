using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using FakeItEasy;
using Microsoft.Azure.Management.EventGrid;
using Microsoft.Azure.Management.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using NUnit.Framework;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.Request;

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


        [Test]
        public async Task WhenCreateOrUpdateSubscriptionThenCreateSubscription()
        {
            _fakeEventGridDomainConfig.Value.SubscriptionId = "246d71e7-1475-ec11-8943-002248818222";
            _fakeEventGridDomainConfig.Value.ResourceGroup = "test-rg";
            _fakeEventGridDomainConfig.Value.EventGridDomainName = "test-event-domain";
            AccessToken fakeToken = new("azsvygcyscvhucs", System.DateTimeOffset.UtcNow.AddHours(1));
            DomainTopic fakeDomainTopic = new("123", "abc");
            CancellationToken cancellationToken = CancellationToken.None;

            DefaultAzureCredential fakeAzureCredential = A.Fake<DefaultAzureCredential>(o => new DefaultAzureCredential());

            A.CallTo(() => fakeAzureCredential.GetTokenAsync(A<TokenRequestContext>.Ignored, A<CancellationToken>.Ignored)).Returns(fakeToken);

            TokenCredentials fakeCredential = A.Fake<TokenCredentials>(o => o.WithArgumentsForConstructor(() => new TokenCredentials(fakeToken.Token)));

            EventGridManagementClient fakeEventGridManagementClient = A.Fake<EventGridManagementClient>(o => new EventGridManagementClient(fakeCredential));
            /////  A.CallToSet(() => fakeEventGridManagementClient.DomainTopics).Invokes(TimeRange newTimes) =>
            /////{
                // have the getter return the new times when called
                /////  A.CallTo(() => fakeShop.OpeningHours).Returns(newTimes);
                A.CallTo(() => fakeEventGridManagementClient.DomainTopics.CreateOrUpdateAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored)).Returns(fakeDomainTopic);
                
                string response = await _azureEventGridDomainService.CreateOrUpdateSubscription(GetSubscriptionRequestMessage(), cancellationToken);
                Assert.IsInstanceOf<string>(response);
            }

            private static SubscriptionRequestMessage GetSubscriptionRequestMessage()
            {
                return new SubscriptionRequestMessage()
                {
                    D365CorrelationId = "6ea03f10-2672-46fb-92a1-5200f6a4faaa",
                    IsActive = true,
                    NotificationType = "Data test",
                    NotificationTypeTopicName = "acc",
                    SubscriptionId = "246d71e7-1475-ec11-8943-002248818222",
                    WebhookUrl = "https://abc.com"
                };
            }
            //////////private static string GetFakeToken()
            //////////{
            //////////    return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ0ZXN0IHNlcnZlciIsImlhdCI6MTU1ODMyOTg2MCwiZXhwIjoxNTg5OTUyMjYwLCJhdWQiOiJ3d3cudGVzdC5jb20iLCJzdWIiOiJ0ZXN0dXNlckB0ZXN0LmNvbSIsIm9pZCI6IjE0Y2I3N2RjLTFiYTUtNDcxZC1hY2Y1LWEwNDBkMTM4YmFhOSJ9.uOPTbf2Tg6M2OIC6bPHsBAOUuFIuCIzQL_MV3qV6agc";
            //////////}
        }
    }

