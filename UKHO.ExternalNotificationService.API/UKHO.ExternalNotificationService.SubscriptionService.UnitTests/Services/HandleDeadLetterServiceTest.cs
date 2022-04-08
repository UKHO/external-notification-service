using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.SubscriptionService.Configuration;
using UKHO.ExternalNotificationService.SubscriptionService.Services;

namespace UKHO.ExternalNotificationService.Webjob.UnitTests.Services
{
    [TestFixture]
    public class HandleDeadLetterServiceTest
    {
        private ICallbackService _fakeCallbackService;
        private IAzureMessageQueueHelper _fakeAzureMessageQueueHelper;
        private IOptions<SubscriptionStorageConfiguration> _fakeEnsStorageConfiguration;
        private ILogger<HandleDeadLetterService> _fakeLogger;
        private IOptions<D365CallbackConfiguration> _fakeD365CallbackConfiguration;
        private HandleDeadLetterService _handleDeadLetterService;
        private const string FilePath = "test";
        private const string FileName = "f257e462-3a2e-452e-81af-ef58b6e3b9fb.json";

        [SetUp]
        public void Setup()
        {
            _fakeCallbackService = A.Fake<ICallbackService>();
            _fakeLogger = A.Fake<ILogger<HandleDeadLetterService>>();
            _fakeAzureMessageQueueHelper = A.Fake<IAzureMessageQueueHelper>();
            _fakeEnsStorageConfiguration = A.Fake<IOptions<SubscriptionStorageConfiguration>>();
            _fakeD365CallbackConfiguration = A.Fake<IOptions<D365CallbackConfiguration>>();

            _handleDeadLetterService = new HandleDeadLetterService(_fakeCallbackService, _fakeAzureMessageQueueHelper, _fakeEnsStorageConfiguration,
                                                                   _fakeLogger, _fakeD365CallbackConfiguration);
        }

        [Test]
        public void WhenDeadLetterCallbackToD365UsingDataverse()
        {
            string  subscriptionId = "9dbdec8b-7ff3-4792-b085-cf1e5a41ca5e";

            A.CallTo(() => _fakeCallbackService.CallbackToD365UsingDataverse(A<string>.Ignored, A<object>.Ignored, A<SubscriptionRequestMessage>.Ignored)).Returns( new HttpResponseMessage());

            Task response =  _handleDeadLetterService.ProcessDeadLetter(FilePath, subscriptionId, new SubscriptionRequestMessage(), FileName);

            A.CallTo(() => _fakeAzureMessageQueueHelper.CopyDeadLetterBlob(A<SubscriptionStorageConfiguration>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.IsTrue(response.IsCompleted);
        }
    }
}
