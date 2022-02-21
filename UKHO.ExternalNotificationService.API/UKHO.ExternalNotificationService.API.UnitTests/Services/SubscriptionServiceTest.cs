using FakeItEasy;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services
{
    [TestFixture]
    public class SubscriptionServiceTest
    {
        private ID365PayloadValidator _fakeD365PayloadValidator;
        private D365Payload _d365PayloadDetails;
        private SubscriptionRequest _subscriptionRequest;        
        private NotificationType _notificationType;
        private ISubscriptionService _subscriptionService;
        private IAzureMessageQueueHelper _fakeAzureMessageQueueHelper;
        private IOptions<SubscriptionStorageConfiguration> _fakeEnsStorageConfiguration;
        private ILogger<SubscriptionService> _fakeLogger;

        [SetUp]
        public void Setup()
        {
            _d365PayloadDetails = GetD365PayloadDetails();
            _subscriptionRequest = GetSubscriptionRequest();            
            _notificationType = GetNotificationType();
            _fakeD365PayloadValidator = A.Fake<ID365PayloadValidator>();
            _fakeAzureMessageQueueHelper = A.Fake<IAzureMessageQueueHelper>();
            _fakeEnsStorageConfiguration = A.Fake<IOptions<SubscriptionStorageConfiguration>>();
            _fakeEnsStorageConfiguration.Value.StorageAccountName = "testaccount";
            _fakeEnsStorageConfiguration.Value.StorageAccountKey = "testaccountkey";
            _fakeEnsStorageConfiguration.Value.QueueName = "test-queue-name";
            _fakeLogger = A.Fake<ILogger<SubscriptionService>>();

            _subscriptionService = new SubscriptionService(_fakeD365PayloadValidator, _fakeAzureMessageQueueHelper, _fakeEnsStorageConfiguration, _fakeLogger);
        }

        # region ValidateD365PayloadRequest
        [Test]
        public async Task WhenInvalidPayloadWithNullInputParameters_ThenReceiveBadrequest()
        {
            A.CallTo(() => _fakeD365PayloadValidator.Validate(A<D365Payload>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure>
                    {new ValidationFailure("InputParameters", "D365Payload InputParameters cannot be blank or null.")}));

            ValidationResult result = await _subscriptionService.ValidateD365PayloadRequest(new D365Payload());

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("D365Payload InputParameters cannot be blank or null.", result.Errors.Single().ErrorMessage);
        }

        [Test]
        public async Task WhenValidPayloadInRequest_ThenReceiveOkResponse()
        {
            A.CallTo(() => _fakeD365PayloadValidator.Validate(A<D365Payload>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure>()));

            ValidationResult result = await _subscriptionService.ValidateD365PayloadRequest(_d365PayloadDetails);

            Assert.IsTrue(result.IsValid);
        }
        #endregion

        #region ConvertToSubscriptionRequest

        [Test]
        public void WhenInvalidPayloadWithoutStateCodeKey_ThenReceiveSubscriptionRequestWithFalseIsActive()
        {
            _d365PayloadDetails.InputParameters[0].Value.FormattedValues= new FormattedValue[]
                                                                            { new FormattedValue { Key = D365PayloadKeyConstant.NotificationTypeKey,
                                                                                                   Value = "Data test" }};
            _d365PayloadDetails.PostEntityImages = Array.Empty<EntityImage>();

            SubscriptionRequest result = _subscriptionService.ConvertToSubscriptionRequestModel(_d365PayloadDetails);

            Assert.IsFalse(result.IsActive);
            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_subscriptionRequest.SubscriptionId, result.SubscriptionId);
        }

        [Test]
        public void WhenValidPayloadWithNullPostEntityImages_ThenReceiveSuccessfulSubscriptionRequest()
        {
            _d365PayloadDetails.PostEntityImages = Array.Empty<EntityImage>();
            SubscriptionRequest result = _subscriptionService.ConvertToSubscriptionRequestModel(_d365PayloadDetails);

            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_subscriptionRequest.SubscriptionId, result.SubscriptionId);
            Assert.AreEqual(_subscriptionRequest.NotificationType, result.NotificationType);
        }

        [Test]
        public void WhenValidPayloadWithNullPostEntityImagesValue_ThenReceiveSuccessfulSubscriptionRequest()
        {
            _d365PayloadDetails.PostEntityImages[0].Value = null;
            SubscriptionRequest result = _subscriptionService.ConvertToSubscriptionRequestModel(_d365PayloadDetails);

            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_subscriptionRequest.SubscriptionId, result.SubscriptionId);
            Assert.AreEqual(_subscriptionRequest.NotificationType, result.NotificationType);
        }

        [Test]
        public void WhenValidPayloadInRequest_ThenReceiveSubscriptionRequest()
        {
            SubscriptionRequest result = _subscriptionService.ConvertToSubscriptionRequestModel(_d365PayloadDetails);

            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_subscriptionRequest.SubscriptionId, result.SubscriptionId);
            Assert.AreEqual(_subscriptionRequest.NotificationType, result.NotificationType);
            Assert.AreEqual(_subscriptionRequest.IsActive, result.IsActive);
            Assert.AreEqual(_subscriptionRequest.WebhookUrl, result.WebhookUrl);
        }
        #endregion

        [Test]
        public void WhenValidSubscriptionRequestDetailsPassed_ThenAddMessageInQueue()
        {
            const string correlationId = "6ea03f10-2672-46fb-92a1-5200f6a4faaa";

            Task result = _subscriptionService.AddSubscriptionRequest(_subscriptionRequest, _notificationType, correlationId);
            
            A.CallTo(() => _fakeAzureMessageQueueHelper.AddQueueMessage(_fakeEnsStorageConfiguration.Value, A<SubscriptionRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.IsTrue(result.IsCompleted);           
        }

        private static D365Payload GetD365PayloadDetails()
        {
            var d365Payload = new D365Payload()
            {
                CorrelationId = "6ea03f10-2672-46fb-92a1-5200f6a4faaa",
                InputParameters = new InputParameter[] { new InputParameter {
                                    Value = new InputParameterValue {
                                                Attributes = new D365Attribute[] {  new D365Attribute { Key = D365PayloadKeyConstant.WebhookUrlKey, Value = "https://abc.com" },
                                                                                    new D365Attribute { Key = D365PayloadKeyConstant.SubscriptionIdKey, Value = "246d71e7-1475-ec11-8943-002248818222" } },
                                                FormattedValues = new FormattedValue[] {new FormattedValue { Key = D365PayloadKeyConstant.NotificationTypeKey, Value = "Data test" },
                                                                                        new FormattedValue { Key =  D365PayloadKeyConstant.IsActiveKey, Value = "Active" }}}}},
                PostEntityImages = new EntityImage[] { new EntityImage {
                                    Key= D365PayloadKeyConstant.PostEntityImageKey,
                                    Value = new EntityImageValue {
                                        Attributes = new D365Attribute[] { new D365Attribute { Key = D365PayloadKeyConstant.WebhookUrlKey, Value = "https://abc.com" },
                                                                           new D365Attribute { Key = D365PayloadKeyConstant.SubscriptionIdKey, Value = "246d71e7-1475-ec11-8943-002248818222" } },
                                        FormattedValues = new FormattedValue[] { new FormattedValue { Key = D365PayloadKeyConstant.NotificationTypeKey, Value = "Data test" },
                                                                                 new FormattedValue { Key =  D365PayloadKeyConstant.IsActiveKey, Value = "Active" }}}}},
                OperationCreatedOn = "/Date(1642149320000+0000)/"
            };

            return d365Payload;
        }

        private static SubscriptionRequest GetSubscriptionRequest()
        {
            return new SubscriptionRequest()
            {
                D365CorrelationId = "6ea03f10-2672-46fb-92a1-5200f6a4faaa",
                IsActive = true,
                NotificationType = "Data test",
                SubscriptionId = "246d71e7-1475-ec11-8943-002248818222",
                WebhookUrl = "https://abc.com"
            };
        }        

        private static NotificationType GetNotificationType()
        {
            return new NotificationType()
            {
                Name = "Test - AVCS Data",
                TopicName = "test-file-published"
            };
        }
    }
}
