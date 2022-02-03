using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Models.Request;
using FluentValidation.TestHelper;
using System.Linq;

namespace UKHO.ExternalNotificationService.API.UnitTests.Validation
{
    public class D365PayloadValidatorTest
    {
        private D365PayloadValidator _d365PayloadValidator;

        [SetUp]
        public void Setup()
        {
            _d365PayloadValidator = new D365PayloadValidator();
        }
        [Test]
        public void WhenNullCorrelationIdRequest_ThenReturnBadRequest()
        {
            var model = new D365Payload
            {
                CorrelationId = null
            };

            var result = _d365PayloadValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(fb => fb.CorrelationId);

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "CorrelationId cannot be blank or null."));
        }

        [Test]
        public void WhenEmptyCorrelationIdInRequest_ThenReturnBadRequest()
        {
            var model = new D365Payload
            {
                CorrelationId = string.Empty
            };

            var result = _d365PayloadValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(fb => fb.CorrelationId);

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "CorrelationId cannot be blank or null."));
        }

        [Test]
        public void WhenNullInputParametersInRequest_ThenReturnBadRequest()
        {
            var model = new D365Payload
            {
                InputParameters= null
            };
            var result = _d365PayloadValidator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(fb => fb.InputParameters);
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "InputParameters cannot be null."));
        }

        [Test]
        public void WhenNullPostEntityImagesInRequest_ThenReturnBadRequest()
        {
            var model = new D365Payload
            {
                PostEntityImages =null
            };
            var result = _d365PayloadValidator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(fb => fb.PostEntityImages);
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "PostEntityImages cannot be null."));
        }

        [Test]
        public void WhenValidD365PayloadRequest_ThenReturnSuccess()
        {
            var model = new D365Payload
                {
                    CorrelationId = "6ea03f10-2672-46fb-92a1-5200f6a4faaa",
                    InputParameters = new InputParameter[] { },
                    PostEntityImages = new EntityImage[] { }
                };

            var result = _d365PayloadValidator.TestValidate(model);
            Assert.AreEqual(0, result.Errors.Count);
        }
    }
}
