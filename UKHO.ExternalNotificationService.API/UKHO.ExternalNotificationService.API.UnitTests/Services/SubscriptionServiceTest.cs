using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helper;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services
{
    [TestFixture]
    public class SubscriptionServiceTest
    {
        private ID365PayloadValidator _fakeD365PayloadValidator;
        private IOptions<D365PayloadKeyConfiguration> _fakeD365PayloadKeyConfiguration;
        private D365Payload _fakeD365PayloadDetails;
        private SubscriptionRequest _fakeSubscriptionRequest;        
        private NotificationType _fakeNotificationType;
        private SubscriptionService _fakeSubscriptionService;
        private IAzureMessageQueueHelper _fakeAzureMessageQueueHelper;
        private IOptions<SubscriptionStorageConfiguration> _fakeEnsStorageConfiguration;
        private ILogger<SubscriptionService> _fakeLogger;

        [SetUp]
        public void Setup()
        {
            _fakeD365PayloadKeyConfiguration = A.Fake<IOptions<D365PayloadKeyConfiguration>>();
            _fakeD365PayloadKeyConfiguration.Value.PostEntityImageKey = "SubscriptionImage";
            _fakeD365PayloadKeyConfiguration.Value.IsActiveKey = "statecode";
            _fakeD365PayloadKeyConfiguration.Value.NotificationTypeKey = "ukho_subscriptiontype";
            _fakeD365PayloadKeyConfiguration.Value.SubscriptionIdKey = "ukho_externalnotificationid";
            _fakeD365PayloadKeyConfiguration.Value.WebhookUrlKey = "ukho_webhookurl";
            _fakeD365PayloadDetails = GetD365PayloadDetails();
            _fakeSubscriptionRequest = GetSubscriptionRequest();            
            _fakeNotificationType = GetNotificationType();
            _fakeD365PayloadValidator = A.Fake<ID365PayloadValidator>();
            _fakeAzureMessageQueueHelper = A.Fake<IAzureMessageQueueHelper>();
            _fakeEnsStorageConfiguration = A.Fake<IOptions<SubscriptionStorageConfiguration>>();
            _fakeLogger = A.Fake<ILogger<SubscriptionService>>();

        _fakeSubscriptionService = new SubscriptionService(_fakeD365PayloadValidator, _fakeD365PayloadKeyConfiguration, _fakeAzureMessageQueueHelper, _fakeEnsStorageConfiguration, _fakeLogger);
        }

        # region ValidateD365PayloadRequest
        [Test]
        public async Task WhenInvalidPayloadWithNullInputParameters_ThenReceiveBadrequest()
        {
            A.CallTo(() => _fakeD365PayloadValidator.Validate(A<D365Payload>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure>
                    {new ValidationFailure("InputParameters", "D365Payload InputParameters cannot be blank or null.")}));

            ValidationResult result = await _fakeSubscriptionService.ValidateD365PayloadRequest(new D365Payload());

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("D365Payload InputParameters cannot be blank or null.", result.Errors.Single().ErrorMessage);
        }

        [Test]
        public async Task WhenValidPayloadInRequest_ThenReceiveOkResponse()
        {
            A.CallTo(() => _fakeD365PayloadValidator.Validate(A<D365Payload>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure>()));

            ValidationResult result = await _fakeSubscriptionService.ValidateD365PayloadRequest(_fakeD365PayloadDetails);

            Assert.IsTrue(result.IsValid);
        }
        #endregion

        #region ConvertToSubscriptionRequest

        [Test]
        public void WhenInvalidPayloadWithoutStateCodeKey_ThenReceiveSubscriptionRequestWithFalseIsActive()
        {
            _fakeD365PayloadDetails.InputParameters[0].Value.FormattedValues= new FormattedValue[]
                                                                            { new FormattedValue { Key = _fakeD365PayloadKeyConfiguration.Value.NotificationTypeKey,
                                                                                                   Value = "Data test" }};
            _fakeD365PayloadDetails.PostEntityImages = Array.Empty<EntityImage>();

            SubscriptionRequest result = _fakeSubscriptionService.ConvertToSubscriptionRequestModel(_fakeD365PayloadDetails);

            Assert.IsFalse(result.IsActive);
            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_fakeSubscriptionRequest.SubscriptionId, result.SubscriptionId);
        }

        [Test]
        public void WhenValidPayloadWithNullPostEntityImages_ThenReceiveSuccessfulSubscriptionRequest()
        {
            _fakeD365PayloadDetails.PostEntityImages = Array.Empty<EntityImage>();
            SubscriptionRequest result = _fakeSubscriptionService.ConvertToSubscriptionRequestModel(_fakeD365PayloadDetails);

            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_fakeSubscriptionRequest.SubscriptionId, result.SubscriptionId);
            Assert.AreEqual(_fakeSubscriptionRequest.NotificationType, result.NotificationType);
        }

        [Test]
        public void WhenValidPayloadWithNullPostEntityImagesValue_ThenReceiveSuccessfulSubscriptionRequest()
        {
            _fakeD365PayloadDetails.PostEntityImages[0].Value = null;
            SubscriptionRequest result = _fakeSubscriptionService.ConvertToSubscriptionRequestModel(_fakeD365PayloadDetails);

            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_fakeSubscriptionRequest.SubscriptionId, result.SubscriptionId);
            Assert.AreEqual(_fakeSubscriptionRequest.NotificationType, result.NotificationType);
        }

        [Test]
        public void WhenValidPayloadInRequest_ThenReceiveSubscriptionRequest()
        {
            SubscriptionRequest result = _fakeSubscriptionService.ConvertToSubscriptionRequestModel(_fakeD365PayloadDetails);

            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_fakeSubscriptionRequest.SubscriptionId, result.SubscriptionId);
            Assert.AreEqual(_fakeSubscriptionRequest.NotificationType, result.NotificationType);
            Assert.AreEqual(_fakeSubscriptionRequest.IsActive, result.IsActive);
            Assert.AreEqual(_fakeSubscriptionRequest.WebhookUrl, result.WebhookUrl);
        }
        #endregion

        [Test]
        public void WhenValidSubscriptionRequestDetailsPassed_ThenGetSubcriptionRequestMessage()
        {
            string fakeCorrelationid = "6ea03f10-2672-46fb-92a1-5200f6a4faaa";            
            A.CallTo(() => _fakeAzureMessageQueueHelper.AddQueueMessage(A<SubscriptionStorageConfiguration>.Ignored, A<SubscriptionRequestMessage>.Ignored, A<string>.Ignored));
            var result = _fakeSubscriptionService.AddSubscriptionRequest(_fakeSubscriptionRequest, _fakeNotificationType, fakeCorrelationid);

            Assert.IsTrue(result.IsCompleted);
           
        }

        private D365Payload GetD365PayloadDetails()
        {
            var d365Payload = new D365Payload()
            {
                CorrelationId = "6ea03f10-2672-46fb-92a1-5200f6a4faaa",
                InputParameters = new InputParameter[] { new InputParameter {
                                    Value = new InputParameterValue {
                                                Attributes = new D365Attribute[] {  new D365Attribute { Key = _fakeD365PayloadKeyConfiguration.Value.WebhookUrlKey, Value = "https://abc.com" },
                                                                                    new D365Attribute { Key = _fakeD365PayloadKeyConfiguration.Value.SubscriptionIdKey, Value = "246d71e7-1475-ec11-8943-002248818222" } },
                                                FormattedValues = new FormattedValue[] {new FormattedValue { Key = _fakeD365PayloadKeyConfiguration.Value.NotificationTypeKey, Value = "Data test" },
                                                                                        new FormattedValue { Key =  _fakeD365PayloadKeyConfiguration.Value.IsActiveKey, Value = "Active" }}}}},
                PostEntityImages = new EntityImage[] { new EntityImage {
                                    Key= _fakeD365PayloadKeyConfiguration.Value.PostEntityImageKey,
                                    Value = new EntityImageValue {
                                        Attributes = new D365Attribute[] { new D365Attribute { Key = _fakeD365PayloadKeyConfiguration.Value.WebhookUrlKey, Value = "https://abc.com" },
                                                                           new D365Attribute { Key = _fakeD365PayloadKeyConfiguration.Value.SubscriptionIdKey, Value = "246d71e7-1475-ec11-8943-002248818222" } },
                                        FormattedValues = new FormattedValue[] { new FormattedValue { Key = _fakeD365PayloadKeyConfiguration.Value.NotificationTypeKey, Value = "Data test" },
                                                                                 new FormattedValue { Key =  _fakeD365PayloadKeyConfiguration.Value.IsActiveKey, Value = "Active" }}}}},
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
