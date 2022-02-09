using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Configuration;
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
        private SubscriptionService _subscriptionService;

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
            _fakeD365PayloadValidator = A.Fake<ID365PayloadValidator>();
            
            _subscriptionService = new SubscriptionService(_fakeD365PayloadValidator, _fakeD365PayloadKeyConfiguration);
        }

        # region ValidateD365PayloadRequest
        [Test]
        public async Task WhenInvalidPayloadWithNullInputParameters_ThenRecieveBadrequest()
        {
            A.CallTo(() => _fakeD365PayloadValidator.Validate(A<D365Payload>.Ignored))
                .Returns(new ValidationResult(new List<ValidationFailure>
                    {new ValidationFailure("InputParameters", "D365Payload InputParameters cannot be blank or null.")}));

            var result = await _subscriptionService.ValidateD365PayloadRequest(new D365Payload());

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("D365Payload InputParameters cannot be blank or null.", result.Errors.Single().ErrorMessage);
        }

        [Test]
        public async Task WhenValidPayloadInRequest_ThenRecieveOkResponse()
        {
            A.CallTo(() => _fakeD365PayloadValidator.Validate(A<D365Payload>.Ignored))
                .Returns(new ValidationResult(new List<ValidationFailure>()));

            var result = await _subscriptionService.ValidateD365PayloadRequest(_fakeD365PayloadDetails);

            Assert.IsTrue(result.IsValid);
        }
        #endregion

        #region ConvertToSubscriptionRequest

        [Test]
        public void WhenInvalidPayloadWithoutStateCodeKey_ThenRecieveSubscriptionRequestWithFalseIsActive()
        {
            _fakeD365PayloadDetails.InputParameters[0].Value.FormattedValues= new FormattedValue[]
                                                                            { new FormattedValue { Key = _fakeD365PayloadKeyConfiguration.Value.NotificationTypeKey,
                                                                                                   Value = "Data test" }};
            _fakeD365PayloadDetails.PostEntityImages = new EntityImage[] { };
            var result = _subscriptionService.ConvertToSubscriptionRequestModel(_fakeD365PayloadDetails);

            Assert.IsFalse(result.IsActive);
            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_fakeSubscriptionRequest.SubscriptionId, result.SubscriptionId);
        }

        [Test]
        public void WhenValidPayloadWithNullPostEntityImages_ThenRecieveSuccessfulSubscriptionRequest()
        {
            _fakeD365PayloadDetails.PostEntityImages = new EntityImage[] { };
            var result = _subscriptionService.ConvertToSubscriptionRequestModel(_fakeD365PayloadDetails);

            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_fakeSubscriptionRequest.SubscriptionId, result.SubscriptionId);
            Assert.AreEqual(_fakeSubscriptionRequest.NotificationType, result.NotificationType);
        }

        [Test]
        public void WhenValidPayloadWithNullPostEntityImagesValue_ThenRecieveSuccessfulSubscriptionRequest()
        {
            _fakeD365PayloadDetails.PostEntityImages[0].Value = null;
            var result = _subscriptionService.ConvertToSubscriptionRequestModel(_fakeD365PayloadDetails);

            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_fakeSubscriptionRequest.SubscriptionId, result.SubscriptionId);
            Assert.AreEqual(_fakeSubscriptionRequest.NotificationType, result.NotificationType);
        }

        [Test]
        public void WhenValidPayloadInRequest_ThenRecieveSubscriptionRequest()
        {
            var result = _subscriptionService.ConvertToSubscriptionRequestModel(_fakeD365PayloadDetails);

            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_fakeSubscriptionRequest.SubscriptionId, result.SubscriptionId);
            Assert.AreEqual(_fakeSubscriptionRequest.NotificationType, result.NotificationType);
        }
        #endregion

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
    }
}
