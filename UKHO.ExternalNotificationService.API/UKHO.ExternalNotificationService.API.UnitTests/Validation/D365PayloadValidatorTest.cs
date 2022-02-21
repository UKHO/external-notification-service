using FluentValidation.TestHelper;
using NUnit.Framework;
using System;
using System.Linq;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.UnitTests.Validation
{
    public class D365PayloadValidatorTest
    {
        private D365Payload _fakeD365Payload;
        private D365PayloadValidator _d365PayloadValidator;

        [SetUp]
        public void Setup()
        {
            _fakeD365Payload = GetD365PayloadDetails();

            _d365PayloadValidator = new D365PayloadValidator();
        }

        # region CorrelationId
        [Test]
        public void WhenNullCorrelationIdRequest_ThenReceiveBadRequest()
        {
            _fakeD365Payload.CorrelationId = null;

            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("CorrelationId");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "D365Payload CorrelationId cannot be blank or null."));
        }

        [Test]
        public void WhenEmptyCorrelationIdInRequest_ThenReceiveBadRequest()
        {
            _fakeD365Payload.CorrelationId = string.Empty;

            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("CorrelationId");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "D365Payload CorrelationId cannot be blank or null."));
        }
        #endregion

        #region PropertyNullValueCheck 
        [Test]
        public void WhenNullInputParametersInRequest_ThenReceiveBadRequest()
        {
            _fakeD365Payload.InputParameters = null;

            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("InputParameters");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "D365Payload InputParameters cannot be blank or null."));
        }

        [Test]
        public void WhenNullPostEntityImagesInRequest_ThenReceiveBadRequest()
        {
            _fakeD365Payload.PostEntityImages = null;

            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("PostEntityImages");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "D365Payload PostEntityImages cannot be blank or null."));
        }
        #endregion

        #region SubscriptionId
        [Test]
        public void WhenRequestWithoutSubscriptionIdKey_ThenReceiveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.Attributes = new D365Attribute[]
                                                                    { new D365Attribute {Key = D365PayloadKeyConstant.WebhookUrlKey,
                                                                                         Value = "https://input.com" } };
            _fakeD365Payload.PostEntityImages[0].Value.Attributes = null;

            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("SubscriptionId");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "SubscriptionId cannot be blank or null."));
        }

        [Test]
        public void WhenRequestWithNullSubscriptionId_ThenReceiveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.Attributes[1].Value = null;

            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("SubscriptionId");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "SubscriptionId cannot be blank or null."));
        }

        #endregion

        #region NotificationType
        [Test]
        public void WhenRequestWithoutNotificationTypeKey_ThenReceiveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.FormattedValues = new FormattedValue[]
                                                                        { new FormattedValue { Key = D365PayloadKeyConstant.IsActiveKey,
                                                                                               Value = "Active" } };
            _fakeD365Payload.PostEntityImages[0].Value.FormattedValues = null;

            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("NotificationType");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "NotificationType cannot be blank or null."));
        }

        [Test]
        public void WhenRequestWithNullNotificationType_ThenReceiveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.FormattedValues[0].Value = null;

            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("NotificationType");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "NotificationType cannot be blank or null."));
        }
        #endregion

        #region WebhookUrl
        [Test]
        public void WhenRequestWithoutWebhookUrlKey_ThenReceiveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.Attributes = new D365Attribute[]
                                                                    { new D365Attribute { Key = D365PayloadKeyConstant.SubscriptionIdKey,
                                                                                          Value = "246d71e7-1475-ec11-8943-002248818222" } };
            _fakeD365Payload.PostEntityImages[0].Value.Attributes = null;

            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("WebhookUrl");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "WebhookUrl cannot be blank or null."));
        }

        [Test]
        public void WhenRequestWithNullWebhookUrl_ThenReceiveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.Attributes[0].Value = null;

            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("WebhookUrl");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "WebhookUrl cannot be blank or null."));
        }
        #endregion

        #region StateCode
        [Test]
        public void WhenRequestWithoutStateCodeKey_ThenReceiveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.FormattedValues = new FormattedValue[]
                                                                        { new FormattedValue { Key = D365PayloadKeyConstant.NotificationTypeKey,
                                                                                               Value = "Data test" } };
            _fakeD365Payload.PostEntityImages[0].Value.FormattedValues = null;

            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("StateCode");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "StateCode cannot be blank or null."));
        }

        [Test]
        public void WhenRequestWithNullStateCode_ThenReceiveBadRequest()
        {
            _fakeD365Payload.InputParameters[0].Value.FormattedValues[1].Value = null;

            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);
            result.ShouldHaveValidationErrorFor("StateCode");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "StateCode cannot be blank or null."));
        }
        #endregion

        #region TestValidateD365Payload
        [Test]
        public void WhenValidRequestD365Payload_ThenReceiveSuccessfulResponse()
        {
            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);

            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        public void WhenValidRequestWithNullPostEntityImages_ThenReceiveSuccessfulResponse()
        {
            _fakeD365Payload.PostEntityImages = Array.Empty<EntityImage>();
            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);

            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        public void WhenValidRequestWithNullPostEntityImagesValue_ThenReceiveSuccessfulResponse()
        {
            _fakeD365Payload.PostEntityImages[0].Value = null;

            TestValidationResult<D365Payload> result = _d365PayloadValidator.TestValidate(_fakeD365Payload);

            Assert.AreEqual(0, result.Errors.Count);
        }
        #endregion

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
    }
}
