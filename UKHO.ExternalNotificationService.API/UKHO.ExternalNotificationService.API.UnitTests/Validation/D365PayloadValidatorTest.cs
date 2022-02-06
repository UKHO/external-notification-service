using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Models.Request;
using FluentValidation.TestHelper;
using System.Linq;

namespace UKHO.ExternalNotificationService.API.UnitTests.Validation
{
    public class D365PayloadValidatorTest
    {
        private D365Payload _d365Payload;
        private D365PayloadValidator _d365PayloadValidator;

        [SetUp]
        public void Setup()
        {
            _d365Payload = GetD365Payload();

            _d365PayloadValidator = new D365PayloadValidator();
        }

        # region CorrelationId
        [Test]
        public void WhenNullCorrelationIdRequest_ThenReturnBadRequest()
        {
            _d365Payload.CorrelationId = null;

            var result = _d365PayloadValidator.TestValidate(_d365Payload);
            result.ShouldHaveValidationErrorFor("CorrelationId");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "D365Payload CorrelationId cannot be blank or null."));
        }

        [Test]
        public void WhenEmptyCorrelationIdInRequest_ThenReturnBadRequest()
        {
            _d365Payload.CorrelationId = string.Empty;

            var result = _d365PayloadValidator.TestValidate(_d365Payload);
            result.ShouldHaveValidationErrorFor("CorrelationId");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "D365Payload CorrelationId cannot be blank or null."));
        }
        #endregion

        #region PropertyNullValueCheck 
        [Test]
        public void WhenNullInputParametersInRequest_ThenReturnBadRequest()
        {
            _d365Payload.InputParameters = null;

            var result = _d365PayloadValidator.TestValidate(_d365Payload);
            result.ShouldHaveValidationErrorFor("InputParameters");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "D365Payload InputParameters cannot be blank or null."));
        }

        [Test]
        public void WhenNullPostEntityImagesInRequest_ThenReturnBadRequest()
        {
            _d365Payload.PostEntityImages = null;

            var result = _d365PayloadValidator.TestValidate(_d365Payload);
            result.ShouldHaveValidationErrorFor("PostEntityImages");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "D365Payload PostEntityImages cannot be blank or null."));
        }
        #endregion

        #region SubscriptionId
        [Test]
        public void WhenRequestWithoutSubscriptionIdKey_ThenReturnBadRequest()
        {
            _d365Payload.InputParameters[0].value.Attributes = new D365Attribute[] { new D365Attribute { key = "ukho_webhookurl", value = "https://input.com" } };
            _d365Payload.PostEntityImages[0].value.Attributes = null;

            var result = _d365PayloadValidator.TestValidate(_d365Payload);
            result.ShouldHaveValidationErrorFor("SubscriptionId");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "SubscriptionId cannot be blank or null."));
        }

        [Test]
        public void WhenRequesWithNullSubscriptionId_ThenReturnBadRequest()
        {
            _d365Payload.InputParameters[0].value.Attributes[1].value = null;

            var result = _d365PayloadValidator.TestValidate(_d365Payload);
            result.ShouldHaveValidationErrorFor("SubscriptionId");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "SubscriptionId cannot be blank or null."));
        }

        #endregion

        #region NotificationType
        [Test]
        public void WhenRequestWithoutNotificationTypeKey_ThenReturnBadRequest()
        {
            _d365Payload.InputParameters[0].value.FormattedValues = new FormattedValue[] { new FormattedValue { key = "statecode", value = "Active" } };
            _d365Payload.PostEntityImages[0].value.FormattedValues = null;

            var result = _d365PayloadValidator.TestValidate(_d365Payload);
            result.ShouldHaveValidationErrorFor("NotificationType");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "NotificationType cannot be blank or null."));
        }

        [Test]
        public void WhenRequestWithNullNotificationType_ThenReturnBadRequest()
        {
            _d365Payload.InputParameters[0].value.FormattedValues[0].value = null;

            var result = _d365PayloadValidator.TestValidate(_d365Payload);
            result.ShouldHaveValidationErrorFor("NotificationType");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "NotificationType cannot be blank or null."));
        }
        #endregion

        #region WebhookUrl
        [Test]
        public void WhenRequestWithoutWebhookUrlKey_ThenReturnBadRequest()
        {
            _d365Payload.InputParameters[0].value.Attributes = new D365Attribute[] { new D365Attribute { key = "ukho_externalnotificationid", value = "246d71e7-1475-ec11-8943-002248818222" } };
            _d365Payload.PostEntityImages[0].value.Attributes = null;

            var result = _d365PayloadValidator.TestValidate(_d365Payload);
            result.ShouldHaveValidationErrorFor("WebhookUrl");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "WebhookUrl cannot be blank or null."));
        }

        [Test]
        public void WhenRequestWithNullWebhookUrl_ThenReturnBadRequest()
        {
            _d365Payload.InputParameters[0].value.Attributes[0].value = null;

            var result = _d365PayloadValidator.TestValidate(_d365Payload);
            result.ShouldHaveValidationErrorFor("WebhookUrl");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "WebhookUrl cannot be blank or null."));
        }
        #endregion

        #region StateCode
        [Test]
        public void WhenRequestWithoutStateCodeKey_ThenReturnBadRequest()
        {
            _d365Payload.InputParameters[0].value.FormattedValues = new FormattedValue[] { new FormattedValue { key = "ukho_subscriptiontype", value = "Data test" } };
            _d365Payload.PostEntityImages[0].value.FormattedValues = null;

            var result = _d365PayloadValidator.TestValidate(_d365Payload);
            result.ShouldHaveValidationErrorFor("StateCode");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "StateCode cannot be blank or null."));
        }

        [Test]
        public void WhenRequestWithNullStateCode_ThenReturnBadRequest()
        {
            _d365Payload.InputParameters[0].value.FormattedValues[1].value = null;

            var result = _d365PayloadValidator.TestValidate(_d365Payload);
            result.ShouldHaveValidationErrorFor("StateCode");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "StateCode cannot be blank or null."));
        }
        #endregion

        #region TestValidateD365Payload
        [Test]
        public void WhenValidRequestD365Payload_ThenTestValidateReturnSuccess()
        {
            var result = _d365PayloadValidator.TestValidate(_d365Payload);

            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        public void WhenValidRequestWithNullPostEntityImages_ThenTestValidateReturnSuccess()
        {
            _d365Payload.PostEntityImages = new EntityImage[] { };
            var result = _d365PayloadValidator.TestValidate(_d365Payload);

            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        public void WhenValidRequestWithNullPostEntityImagesValue_ThenTestValidateReturnSuccess()
        {
            _d365Payload.PostEntityImages[0].value = null;

            var result = _d365PayloadValidator.TestValidate(_d365Payload);

            Assert.AreEqual(0, result.Errors.Count);
        }

        #endregion

        private D365Payload GetD365Payload()
        {
            return new D365Payload
            {
                CorrelationId = "6ea03f10-2672-46fb-92a1-5200f6a4faaa",
                InputParameters = new InputParameter[] { new InputParameter {
                                                            value = new InputParameterValue {
                                                                Attributes =   new D365Attribute[] { new D365Attribute { key = "ukho_webhookurl", value = "https://input.com" },
                                                                               new D365Attribute { key = "ukho_externalnotificationid", value = "246d71e7-1475-ec11-8943-002248818222" } },
                                                                FormattedValues = new FormattedValue[] { new FormattedValue { key = "ukho_subscriptiontype", value = "Data test" },
                                                                                  new FormattedValue { key = "statecode", value = "Active" } }} } },
                PostEntityImages = new EntityImage[] { new EntityImage {
                                                            key= "SubscriptionImage",
                                                            value = new EntityImageValue {
                                                                Attributes =   new D365Attribute[] { new D365Attribute { key = "ukho_webhookurl", value = "https://abc.com" },
                                                                               new D365Attribute { key = "ukho_externalnotificationid", value = "246d71e7-1475-ec11-8943-002248818222" } },
                                                                FormattedValues = new FormattedValue[] { new FormattedValue { key = "ukho_subscriptiontype", value = "Data test" },
                                                                                  new FormattedValue { key = "statecode", value = "Active" } }} } },
                OperationCreatedOn = "/Date(1642149320000+0000)/"
            };
        }
    }
}
