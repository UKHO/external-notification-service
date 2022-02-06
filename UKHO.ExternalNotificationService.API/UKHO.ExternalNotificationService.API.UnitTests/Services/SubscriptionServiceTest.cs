using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentValidation.Results;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services
{
    [TestFixture]
    public class SubscriptionServiceTest
    {
        private ID365PayloadValidator _fakeD365PayloadValidator;
        private SubscriptionService _subscriptionService;
        private D365Payload _fakeD365PayloadDetails;
        private SubscriptionRequest _fakeSubscriptionRequest;

        [SetUp]
        public void Setup()
        {
            _fakeD365PayloadDetails = GetD365PayloadDetails();
            _fakeSubscriptionRequest = GetSubscriptionRequest();

            _fakeD365PayloadValidator = A.Fake<ID365PayloadValidator>();
            _subscriptionService = new SubscriptionService(_fakeD365PayloadValidator);
        }

        # region ValidateD365PayloadRequest
        [Test]
        public async Task WhenInvalidNullInputParametersInRequest_ThenValidateD365PayloadRequestReturnsBadrequest()
        {
            A.CallTo(() => _fakeD365PayloadValidator.Validate(A<D365Payload>.Ignored))
                .Returns(new ValidationResult(new List<ValidationFailure>
                    {new ValidationFailure("InputParameters", "D365Payload InputParameters cannot be blank or null.")}));

            var result = await _subscriptionService.ValidateD365PayloadRequest(new D365Payload());

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("D365Payload InputParameters cannot be blank or null.", result.Errors.Single().ErrorMessage);
        }

        [Test]
        public async Task WhenValidPayloadStructureInRequest_ThenValidateD365PayloadRequestReturnsOkResponse()
        {
            A.CallTo(() => _fakeD365PayloadValidator.Validate(A<D365Payload>.Ignored))
                .Returns(new ValidationResult(new List<ValidationFailure>()));

            var result = await _subscriptionService.ValidateD365PayloadRequest(_fakeD365PayloadDetails);

            Assert.IsTrue(result.IsValid);
        }
        #endregion

        #region ConvertToSubscriptionRequest

        [Test]
        public void WhenInvalidRequestWithoutStateCodeKey_ThenConvertToSubscriptionRequestReturnsFalseIsActive()
        {
            _fakeD365PayloadDetails.InputParameters[0].Value.FormattedValues= new FormattedValue[]{ new FormattedValue { Key = "ukho_subscriptiontype", Value = "Data test" }};
            _fakeD365PayloadDetails.PostEntityImages = new EntityImage[] { };
            var result = _subscriptionService.ConvertToSubscriptionRequestModel(_fakeD365PayloadDetails);

            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_fakeSubscriptionRequest.SubscriptionId, result.SubscriptionId);
            Assert.IsFalse(result.IsActive);
        }

        [Test]
        public void WhenValidRequestWithNullPostEntityImages_ThenConvertToSubscriptionRequestReturnsOkrequest()
        {
            _fakeD365PayloadDetails.PostEntityImages = new EntityImage[] { };
            var result = _subscriptionService.ConvertToSubscriptionRequestModel(_fakeD365PayloadDetails);

            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_fakeSubscriptionRequest.SubscriptionId, result.SubscriptionId);
            Assert.AreEqual(_fakeSubscriptionRequest.NotificationType, result.NotificationType);
        }

        [Test]
        public void WhenValidRequestWithNullPostEntityImagesValue_ThenConvertToSubscriptionRequestReturnsOkrequest()
        {
            _fakeD365PayloadDetails.PostEntityImages[0].ImageValue = null;
            var result = _subscriptionService.ConvertToSubscriptionRequestModel(_fakeD365PayloadDetails);

            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_fakeSubscriptionRequest.SubscriptionId, result.SubscriptionId);
            Assert.AreEqual(_fakeSubscriptionRequest.NotificationType, result.NotificationType);
        }

        [Test]
        public void WhenValidSubscriptionRequest_ThenConvertToSubscriptionRequestReturnsOkrequest()
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
                                                Attributes = new D365Attribute[] {  new D365Attribute { Key = "ukho_webhookurl", Value = "https://abc.com" },
                                                                                    new D365Attribute { Key = "ukho_externalnotificationid", Value = "246d71e7-1475-ec11-8943-002248818222" } },
                                                FormattedValues = new FormattedValue[] {new FormattedValue { Key = "ukho_subscriptiontype", Value = "Data test" },
                                                                                        new FormattedValue { Key = "statecode", Value = "Active" }}}}},
                PostEntityImages = new EntityImage[] { new EntityImage {
                                    Key= "SubscriptionImage",
                                    ImageValue = new EntityImageValue {
                                        Attributes = new D365Attribute[] { new D365Attribute { Key = "ukho_webhookurl", Value = "https://abc.com" },
                                                                           new D365Attribute { Key = "ukho_externalnotificationid", Value = "246d71e7-1475-ec11-8943-002248818222" } },
                                        FormattedValues = new FormattedValue[] { new FormattedValue { Key = "ukho_subscriptiontype", Value = "Data test" },
                                                                                 new FormattedValue { Key = "statecode", Value = "Active" }}}}},
                OperationCreatedOn = "/Date(1642149320000+0000)/"
            };

            return d365Payload;
        }

        private SubscriptionRequest GetSubscriptionRequest()
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
