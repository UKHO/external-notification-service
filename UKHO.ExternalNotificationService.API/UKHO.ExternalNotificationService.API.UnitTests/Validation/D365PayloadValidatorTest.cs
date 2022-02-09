using System.Linq;
using FakeItEasy;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.UnitTests.Validation
{
    public class D365PayloadValidatorTest
    {
        private D365Payload _fakeD365Payload;
        private IOptions<D365PayloadKeyConfiguration> _fakeD365PayloadKeyConfiguration;
        private D365PayloadValidator _d365PayloadValidator;

        [SetUp]
        public void Setup()
        {
            _fakeD365PayloadKeyConfiguration = A.Fake<IOptions<D365PayloadKeyConfiguration>>();
            _fakeD365PayloadKeyConfiguration.Value.PostEntityImageKey = "SubscriptionImage";
            _fakeD365PayloadKeyConfiguration.Value.IsActiveKey = "statecode";
            _fakeD365PayloadKeyConfiguration.Value.NotificationTypeKey = "ukho_subscriptiontype";
            _fakeD365PayloadKeyConfiguration.Value.SubscriptionIdKey = "ukho_externalnotificationid";
            _fakeD365PayloadKeyConfiguration.Value.WebhookUrlKey = "ukho_webhookurl";
            _fakeD365Payload = GetD365PayloadDetails();

            _d365PayloadValidator = new D365PayloadValidator(_fakeD365PayloadKeyConfiguration);
        }

        # region CorrelationId
        [Test]
        public void WhenNullCorrelationIdRequest_ThenRecieveBadRequest()
        {
            _fakeD365Payload.CorrelationId = null;

            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("CorrelationId");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "D365Payload CorrelationId cannot be blank or null."));
        }

        [Test]
        public void WhenEmptyCorrelationIdInRequest_ThenRecieveBadRequest()
        {
            _fakeD365Payload.CorrelationId = string.Empty;

            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("CorrelationId");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "D365Payload CorrelationId cannot be blank or null."));
        }
        #endregion

        #region PropertyNullValueCheck 
        [Test]
        public void WhenNullInputParametersInRequest_ThenRecieveBadRequest()
        {
            _fakeD365Payload.InputParameters = null;

            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("InputParameters");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "D365Payload InputParameters cannot be blank or null."));
        }

        [Test]
        public void WhenNullPostEntityImagesInRequest_ThenRecieveBadRequest()
        {
            _fakeD365Payload.PostEntityImages = null;

            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("PostEntityImages");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "D365Payload PostEntityImages cannot be blank or null."));
        }
        #endregion

        #region SubscriptionId
        [Test]
        public void WhenRequestWithoutSubscriptionIdKey_ThenRecieveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.Attributes = new D365Attribute[]
                                                                    { new D365Attribute {Key = _fakeD365PayloadKeyConfiguration.Value.WebhookUrlKey,
                                                                                         Value = "https://input.com" } };
            _fakeD365Payload.PostEntityImages[0].Value.Attributes = null;

            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("SubscriptionId");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "SubscriptionId cannot be blank or null."));
        }

        [Test]
        public void WhenRequestWithNullSubscriptionId_ThenRecieveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.Attributes[1].Value = null;

            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("SubscriptionId");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "SubscriptionId cannot be blank or null."));
        }

        #endregion

        #region NotificationType
        [Test]
        public void WhenRequestWithoutNotificationTypeKey_ThenRecieveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.FormattedValues = new FormattedValue[]
                                                                        { new FormattedValue { Key = _fakeD365PayloadKeyConfiguration.Value.IsActiveKey,
                                                                                               Value = "Active" } };
            _fakeD365Payload.PostEntityImages[0].Value.FormattedValues = null;

            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("NotificationType");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "NotificationType cannot be blank or null."));
        }

        [Test]
        public void WhenRequestWithNullNotificationType_ThenRecieveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.FormattedValues[0].Value = null;

            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("NotificationType");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "NotificationType cannot be blank or null."));
        }
        #endregion

        #region WebhookUrl
        [Test]
        public void WhenRequestWithoutWebhookUrlKey_ThenRecieveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.Attributes = new D365Attribute[]
                                                                    { new D365Attribute { Key = _fakeD365PayloadKeyConfiguration.Value.SubscriptionIdKey,
                                                                                          Value = "246d71e7-1475-ec11-8943-002248818222" } };
            _fakeD365Payload.PostEntityImages[0].Value.Attributes = null;

            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("WebhookUrl");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "WebhookUrl cannot be blank or null."));
        }

        [Test]
        public void WhenRequestWithNullWebhookUrl_ThenRecieveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.Attributes[0].Value = null;

            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("WebhookUrl");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "WebhookUrl cannot be blank or null."));
        }
        #endregion

        #region StateCode
        [Test]
        public void WhenRequestWithoutStateCodeKey_ThenRecieveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.FormattedValues = new FormattedValue[]
                                                                        { new FormattedValue { Key = _fakeD365PayloadKeyConfiguration.Value.NotificationTypeKey,
                                                                                               Value = "Data test" } };
            _fakeD365Payload.PostEntityImages[0].Value.FormattedValues = null;

            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("StateCode");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "StateCode cannot be blank or null."));
        }

        [Test]
        public void WhenRequestWithNullStateCode_ThenRecieveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.FormattedValues[1].Value = null;

            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("StateCode");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "StateCode cannot be blank or null."));
        }
        #endregion

        #region TestValidateD365Payload
        [Test]
        public void WhenValidRequestD365Payload_ThenRecieveSuccessfulResponse()
        {
            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);

            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        public void WhenValidRequestWithNullPostEntityImages_ThenRecieveSuccessfulResponse()
        {
            _fakeD365Payload.PostEntityImages = new EntityImage[] { };
            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);

            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        public void WhenValidRequestWithNullPostEntityImagesValue_ThenRecieveSuccessfulResponse()
        {
            _fakeD365Payload.PostEntityImages[0].Value = null;

            var result = _d365PayloadValidator.TestValidate(_fakeD365Payload);

            Assert.AreEqual(0, result.Errors.Count);
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
    }
}
