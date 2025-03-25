using System.Linq;
using FluentValidation.TestHelper;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.UnitTests.BaseClass;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Models.EventModel;

namespace UKHO.ExternalNotificationService.API.UnitTests.Validation
{
    [TestFixture]
    public class FssEventDataValidatorTest
    {
        private FssEventData _fakeFssEventData;
        private FssEventDataValidator _fssEventDataValidator;

        [SetUp]
        public void Setup()
        {
            _fakeFssEventData = CustomCloudEventBase.GetFssEventData();
            _fssEventDataValidator = new FssEventDataValidator();
        }

        #region BatchId
        [Test]
        public void WhenNullBatchIdInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.BatchId = null;

            var result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("BatchId");

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Any(x => x.ErrorMessage == "BatchId cannot be blank or null."));
                Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
            });
        }
        #endregion

        #region BusinessUnit
        [Test]
        public void WhenNullBusinessUnitInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.BusinessUnit = null;

            var result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("BusinessUnit");

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Any(x => x.ErrorMessage == "Business unit cannot be blank or null."));
                Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
            });
        }
        #endregion

        #region BatchPublishedDate
        [Test]
        public void WhenNullBatchPublishedDateInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.BatchPublishedDate = null;

            var result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("BatchPublishedDate");

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Any(x => x.ErrorMessage == "Batch published date cannot be blank or null."));
                Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
            });
        }
        #endregion

        #region Links
        [Test]
        public void WhenNullLinksValueInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Links = null;

            var result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("Links");

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Any(x => x.ErrorMessage == "Links detail cannot be blank or null."));
                Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
            });
        }
        #endregion

        #region BatchDetails
        [Test]
        public void WhenNullBatchDetailsInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Links.BatchDetails = null;

            var result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("BatchDetails");

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Any(x => x.ErrorMessage == "Links batch detail cannot be blank or null."));
                Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
            });
        }
        #endregion

        #region BatchStatus
        [Test]
        public void WhenNullBatchStatusInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Links.BatchStatus = null;

            var result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("BatchStatus");

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Any(x => x.ErrorMessage == "Links batch status cannot be blank or null."));
                Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
            });
        }
        #endregion

        #region BatchDetailsUri
        [Test]
        public void WhenNullBatchDetailsUriInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Links.BatchDetails.Href = null;

            var result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("BatchDetailsUri");

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Any(x => x.ErrorMessage == "Links batch detail uri cannot be blank or null."));
                Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
            });
        }
        #endregion

        #region BatchStatusUri
        [Test]
        public void WhenNullBatchStatusUriInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Links.BatchStatus.Href = null;

            var result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("BatchStatusUri");

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Any(x => x.ErrorMessage == "Links batch status uri cannot be blank or null."));
                Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
            });
        }
        #endregion

        #region Files
        [Test]
        public void WhenNullLinksInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Files.FirstOrDefault().Links = null;

            var result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("Links");

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Any(x => x.ErrorMessage == "File links cannot be blank or null."));
                Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
            });
        }
        #endregion

        #region MIMEType
        [Test]
        public void WhenNullMIMEInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Files.FirstOrDefault().MIMEType = null;

            var result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("MIMEType");

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Any(x => x.ErrorMessage == "File MIME type cannot be null."));
                Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
            });
        }
        #endregion

        #region Hash
        [Test]
        public void WhenNullHashInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Files.FirstOrDefault().Hash = null;

            var result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("Hash");

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Any(x => x.ErrorMessage == "File hash cannot be null."));
                Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
            });
        }
        #endregion

        #region FileName
        [Test]
        public void WhenNullFileNameInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Files.FirstOrDefault().FileName = null;

            var result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("FileName");

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Any(x => x.ErrorMessage == "File name cannot be null."));
                Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
            });
        }
        #endregion

        #region FileSize
        [Test]
        public void WhenNullFileSizeInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Files.FirstOrDefault().FileSize = 0;

            var result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("FileSize");

            Assert.Multiple(() =>
            {
                Assert.That(result.Errors.Any(x => x.ErrorMessage == "File size cannot be null."));
                Assert.That(result.Errors.Any(x => x.ErrorCode == "OK"));
            });
        }
        #endregion

        #region FssEventData
        [Test]
        public void WhenValidRequest_ThenReceiveSuccessfulResponse()
        {
            var result = _fssEventDataValidator.TestValidate(_fakeFssEventData);

            Assert.That(result.Errors, Is.Empty);
        }
        #endregion
    }
}
