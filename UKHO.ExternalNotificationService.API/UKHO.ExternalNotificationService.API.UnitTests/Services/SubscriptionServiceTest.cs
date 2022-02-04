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
        private D365Payload _fakeD365PayloadDetailsWithInputParameters, _fakeD365PayloadDetailsWithPostEntityImages;
        private D365PayloadValidation _fakeD365PayloadValidation;
        private SubscriptionRequest _fakeSubscriptionRequest;

        [SetUp]
        public void Setup()
        {
            _fakeD365PayloadDetailsWithInputParameters = GetD365PayloadDetailsWithInputParameters();
            _fakeD365PayloadDetailsWithPostEntityImages = GetD365PayloadDetailsWithPostEntityImages();
            _fakeSubscriptionRequest = GetSubscriptionRequest();
            _fakeD365PayloadValidation = new D365PayloadValidation { D365Payload = GetD365PayloadDetailsWithInputParameters()};

            _fakeD365PayloadValidator = A.Fake<ID365PayloadValidator>();
            _subscriptionService = new SubscriptionService(_fakeD365PayloadValidator);
        }
        [Test]
        public async Task WhenInvalidNullInputParametersInRequest_ThenValidateD365PayloadRequestReturnsBadrequest()
        {
            A.CallTo(() => _fakeD365PayloadValidator.Validate(A<D365PayloadValidation>.Ignored))
                .Returns(new ValidationResult(new List<ValidationFailure>
                    {new ValidationFailure("InputParameters", "D365Payload InputParameters cannot be blank or null.")}));

            var result = await _subscriptionService.ValidateD365PayloadRequest(new D365PayloadValidation());

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("D365Payload InputParameters cannot be blank or null.", result.Errors.Single().ErrorMessage);
        }

        [Test]
        public async Task WhenValidPayloadStructureInRequest_ThenValidateD365PayloadRequestReturnsOkResponse()
        {
            A.CallTo(() => _fakeD365PayloadValidator.Validate(A<D365PayloadValidation>.Ignored))
                .Returns(new ValidationResult(new List<ValidationFailure>()));

            var result = await _subscriptionService.ValidateD365PayloadRequest(_fakeD365PayloadValidation);

            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void WhenValidSubscriptionRequestWithInputParameters_ThenConvertToSubscriptionRequestModelRequestReturnsOkrequest()
        {
            var result =  _subscriptionService.ConvertToSubscriptionRequestModel(_fakeD365PayloadDetailsWithInputParameters);

            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_fakeSubscriptionRequest.SubscriptionId, result.SubscriptionId);
            Assert.AreEqual(_fakeSubscriptionRequest.NotificationType, result.NotificationType); 
        }


        [Test]
        public void WhenValidSubscriptionRequestWithPostEntityImages_ThenConvertToSubscriptionRequestModelRequestReturnsOkrequest()
        {
            var result = _subscriptionService.ConvertToSubscriptionRequestModel(_fakeD365PayloadDetailsWithPostEntityImages);

            Assert.IsInstanceOf<SubscriptionRequest>(result);
            Assert.AreEqual(_fakeSubscriptionRequest.SubscriptionId, result.SubscriptionId);
            Assert.AreEqual(_fakeSubscriptionRequest.NotificationType, result.NotificationType);
        }

        private D365Payload GetD365PayloadDetailsWithInputParameters()
        {
            var d365Payload = new D365Payload()
            {
                CorrelationId = "6ea03f10-2672-46fb-92a1-5200f6a4faaa",
                InputParameters = new InputParameter[] { new InputParameter {
                                    value = new InputParameterValue {
                                                Attributes = new D365Attribute[] {  new D365Attribute { key = "ukho_webhookurl", value = "https://abc.com" },
                                                                                    new D365Attribute { key = "ukho_externalnotificationid", value = "246d71e7-1475-ec11-8943-002248818222" } },
                                                FormattedValues = new FormattedValue[] {new FormattedValue { key = "ukho_subscriptiontype", value = "Data test" },
                                                                                        new FormattedValue { key = "statecode", value = "Active" } } } } },
                PostEntityImages = new EntityImage[] { },
                OperationCreatedOn = "/Date(1642149320000+0000)/"
            };

            return d365Payload;
        }

        private D365Payload GetD365PayloadDetailsWithPostEntityImages()
        {
            var d365Payload = new D365Payload()
            {
                CorrelationId = "6ea03f10-2672-46fb-92a1-5200f6a4faaa",
                InputParameters = new InputParameter[] { new InputParameter {
                                    value = new InputParameterValue {
                                        Attributes = new D365Attribute[] { new D365Attribute { key = "test", value = "test" }},
                                        FormattedValues =  new FormattedValue[]{}}} },
                PostEntityImages = new EntityImage[] { new EntityImage {
                                    key= "SubscriptionImage",
                                    value = new EntityImageValue {
                                        Attributes = new D365Attribute[] { new D365Attribute { key = "ukho_webhookurl", value = "https://abc.com" },
                                                                           new D365Attribute { key = "ukho_externalnotificationid", value = "246d71e7-1475-ec11-8943-002248818222" } },
                                        FormattedValues = new FormattedValue[] { new FormattedValue { key = "ukho_subscriptiontype", value = "Data test" },
                                                                                 new FormattedValue { key = "statecode", value = "Active" }}}}},
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
