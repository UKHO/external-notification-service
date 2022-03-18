using FluentValidation.TestHelper;
using NUnit.Framework;
using System.Linq;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.UnitTests.BaseClass;

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

        #region ProductType
        [Test]
        public void WhenNullProductTypeInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.ProductType = null;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("ProductType");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "ProductType cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region productName
        [Test]
        public void WhenNullProductNameInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.ProductName = null;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("ProductName");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "ProductName cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region EditionNumber
        [Test]
        public void WhenEditionNumberLessThanZeroInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.EditionNumber = -1;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("EditionNumber");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "EditionNumber cannot be less than zero or blank."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region UpdateNumber
        [Test]
        public void WhenUpdateNumberLessThanZeroInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.UpdateNumber = -1;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("UpdateNumber");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "UpdateNumber cannot be less than zero or blank."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region BoundingBox
        [Test]
        public void WhenNullBoundingBoxInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.BoundingBox = null;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("BoundingBox");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "BoundingBox cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region NorthLimit
        [Test]
        public void WhenNorthLimitLessThanZeroInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.BoundingBox.NorthLimit = -1;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("NorthLimit");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "NorthLimit cannot be less than zero or blank."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region SouthLimit
        [Test]
        public void WhenSouthLimitLessThanZeroInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.BoundingBox.SouthLimit = -1;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("SouthLimit");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "SouthLimit cannot be less than zero or blank."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region EastLimit
        [Test]
        public void WhenEastLimitLessThanZeroInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.BoundingBox.EastLimit = -1;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("EastLimit");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "EastLimit cannot be less than zero or blank."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region WestLimit
        [Test]
        public void WhenWestLimitLessThanZeroInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.BoundingBox.WestLimit = -1;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("WestLimit");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "WestLimit cannot be less than zero or blank."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region Status
        [Test]
        public void WhenNullStatusInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.Status = null;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("Status");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Status cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region StatusDate
        [Test]
        public void WhenNullStatusDateInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.Status.StatusDate = null;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("StatusDate");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "StatusDate cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region StatusName
        [Test]
        public void WhenNullStatusNameInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeScsEventData.Status.StatusName = null;

            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);
            result.ShouldHaveValidationErrorFor("StatusName");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "StatusName cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region ScsEventData
        [Test]
        public void WhenValidRequest_ThenReceiveSuccessfulResponse()
        {
            TestValidationResult<ScsEventData> result = _scsEventDataValidator.TestValidate(_fakeScsEventData);

            Assert.AreEqual(0, result.Errors.Count);
        }
        #endregion
    }
}
