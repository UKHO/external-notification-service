using System.Linq;
using FluentValidation.TestHelper;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.UnitTests.BaseClass;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Models.EventModel;

namespace UKHO.ExternalNotificationService.API.UnitTests.Validation
{
    [TestFixture]
    public class ScsEventDataValidatorTest
    {
        private ScsEventData _fakeScsEventData;
        private ScsEventDataValidator _scsEventDataValidator;

        [SetUp]
        public void Setup()
        {
            _fakeScsEventData = CustomCloudEventBase.GetScsEventData();

            _scsEventDataValidator = new ScsEventDataValidator();
        }

        [Test]
        public void WhenNullProductTypeInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.ProductType = null;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("ProductType");

            Assert.That(result.Errors.Any(x => x.ErrorMessage == "ProductType cannot be blank or null."));
            Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
        }

        [Test]
        public void WhenNullProductNameInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.ProductName = null;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("ProductName");

            Assert.That(result.Errors.Any(x => x.ErrorMessage == "ProductName cannot be blank or null."));
            Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
        }

        [Test]
        public void WhenEditionNumberLessThanZeroInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.EditionNumber = -1;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("EditionNumber");

            Assert.That(result.Errors.Any(x => x.ErrorMessage == "EditionNumber cannot be less than zero or blank."));
            Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
        }

        [Test]
        public void WhenUpdateNumberLessThanZeroInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.UpdateNumber = -1;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("UpdateNumber");

            Assert.That(result.Errors.Any(x => x.ErrorMessage == "UpdateNumber cannot be less than zero or blank."));
            Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
        }

        [Test]
        public void WhenFileSizeLessThanZeroInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.FileSize = -1;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("FileSize");

            Assert.That(result.Errors.Any(x => x.ErrorMessage == "FileSize cannot be less than zero or blank."));
            Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
        }

        [Test]
        public void WhenNullBoundingBoxInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.BoundingBox = null;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("BoundingBox");

            Assert.That(result.Errors.Any(x => x.ErrorMessage == "BoundingBox cannot be blank or null."));
            Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
        }

        [TestCase(-90.1, false)]
        [TestCase(-90.0, true)]
        [TestCase(0, true)]
        [TestCase(90, true)]
        [TestCase(90.1, false)]
        public void WhenNorthLimitHasValue_ReceiveCorrectResponse(double value, bool shouldBeValid)
        {
            _fakeScsEventData.BoundingBox.NorthLimit = value;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);

            CheckLatLongResult(nameof(ProductBoundingBox.NorthLimit), 90, result, shouldBeValid);
        }

        [TestCase(-90.1, false)]
        [TestCase(-90.0, true)]
        [TestCase(0, true)]
        [TestCase(90, true)]
        [TestCase(90.1, false)]
        public void WhenSouthLimitHasValue_ReceiveCorrectResponse(double value, bool shouldBeValid)
        {
            _fakeScsEventData.BoundingBox.SouthLimit = value;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);

            CheckLatLongResult(nameof(ProductBoundingBox.SouthLimit), 90, result, shouldBeValid);
        }

        [TestCase(-180.1, false)]
        [TestCase(-180.0, true)]
        [TestCase(0, true)]
        [TestCase(180, true)]
        [TestCase(180.1, false)]
        public void WhenEastLimitHasValue_ReceiveCorrectResponse(double value, bool shouldBeValid)
        {
            _fakeScsEventData.BoundingBox.EastLimit = value;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);

            CheckLatLongResult(nameof(ProductBoundingBox.EastLimit), 180, result, shouldBeValid);
        }

        [TestCase(-180.1, false)]
        [TestCase(-180.0, true)]
        [TestCase(0, true)]
        [TestCase(180, true)]
        [TestCase(180.1, false)]
        public void WhenWestLimitHasValue_ReceiveCorrectResponse(double value, bool shouldBeValid)
        {
            _fakeScsEventData.BoundingBox.WestLimit = value;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);

            CheckLatLongResult(nameof(ProductBoundingBox.WestLimit), 180, result, shouldBeValid);
        }

        [Test]
        public void WhenNullStatusInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.Status = null;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("Status");

            Assert.That(result.Errors.Any(x => x.ErrorMessage == "Status cannot be blank or null."));
            Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
        }

        [Test]
        public void WhenNullStatusDateInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.Status.StatusDate = null;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("StatusDate");

            Assert.That(result.Errors.Any(x => x.ErrorMessage == "StatusDate cannot be blank or null."));
            Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
        }

        [Test]
        public void WhenNullStatusNameInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.Status.StatusName = null;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("StatusName");

            Assert.That(result.Errors.Any(x => x.ErrorMessage == "StatusName cannot be blank or null."));
            Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
        }

        [Test]
        public void WhenValidRequest_ThenReceiveSuccessfulResponse()
        {
            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);

            Assert.That(0, Is.EqualTo(result.Errors.Count));
        }

        private static void CheckLatLongResult(string property, int limit, TestValidationResult<ScsEventData> result, bool shouldBeValid)
        {
            if (shouldBeValid)
            {
                Assert.That(result.IsValid);
                Assert.That(0, Is.EqualTo(result.Errors.Count));
            }
            else
            {
                Assert.That(1, Is.EqualTo(result.Errors.Count));
                result.ShouldHaveValidationErrorFor(property);
                Assert.That(result.Errors.Any(x => x.ErrorMessage == $"{property} should be in the range -{limit}.0 to +{limit}.0."));
                Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
            }
        }
    }
}
