﻿using FluentValidation.TestHelper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using Attribute = UKHO.ExternalNotificationService.Common.Models.EventModel.Attribute;
using File = UKHO.ExternalNotificationService.Common.Models.EventModel.File;

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
            _fakeFssEventData = GetFssEventData();
            _fssEventDataValidator = new FssEventDataValidator();
        }

        #region BatchId
        [Test]
        public void WhenNullBatchIdInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.BatchId = null;

            TestValidationResult<FssEventData> result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("BatchId");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "BatchId cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region BusinessUnit
        [Test]
        public void WhenNullBusinessUnitInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.BusinessUnit = null;

            TestValidationResult<FssEventData> result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("BusinessUnit");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Business unit cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region BatchPublishedDate
        [Test]
        public void WhenNullBatchPublishedDateInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.BatchPublishedDate = null;

            TestValidationResult<FssEventData> result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("BatchPublishedDate");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Batch published date cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region Links
        [Test]
        public void WhenNullLinksValueInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Links = null;

            TestValidationResult<FssEventData> result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("Links");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Links detail cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region BatchDetails
        [Test]
        public void WhenNullBatchDetailsInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Links.BatchDetails = null;

            TestValidationResult<FssEventData> result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("BatchDetails");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Links batch detail cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region BatchStatus
        [Test]
        public void WhenNullBatchStatusInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Links.BatchStatus = null;

            TestValidationResult<FssEventData> result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("BatchStatus");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Links batch status cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region BatchDetailsUri
        [Test]
        public void WhenNullBatchDetailsUriInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Links.BatchDetails.Href = null;

            TestValidationResult<FssEventData> result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("BatchDetailsUri");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Links batch detail uri cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region BatchStatusUri
        [Test]
        public void WhenNullBatchStatusUriInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Links.BatchStatus.Href = null;

            TestValidationResult<FssEventData> result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("BatchStatusUri");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Links batch status uri cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region Files
        [Test]
        public void WhenNullLinksInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Files.FirstOrDefault().Links = null;

            TestValidationResult<FssEventData> result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("Links");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "File links cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region MIMEType
        [Test]
        public void WhenNullMIMEInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Files.FirstOrDefault().MIMEType= null;

            TestValidationResult<FssEventData> result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("MIMEType");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "File MIME type cannot be null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region Hash
        [Test]
        public void WhenNullHashInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Files.FirstOrDefault().Hash = null;

            TestValidationResult<FssEventData> result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("Hash");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "File hash cannot be null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region FileName
        [Test]
        public void WhenNullFileNameInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Files.FirstOrDefault().FileName = null;

            TestValidationResult<FssEventData> result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("FileName");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "File name cannot be null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region FileSize
        [Test]
        public void WhenNullFileSizeInRequest_ThenReceiveSuccessfulResponse()
        {
            _fakeFssEventData.Files.FirstOrDefault().FileSize = 0;

            TestValidationResult<FssEventData> result = _fssEventDataValidator.TestValidate(_fakeFssEventData);
            result.ShouldHaveValidationErrorFor("FileSize");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "File size cannot be null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region FssEventData
        [Test]
        public void WhenValidRequest_ThenReceiveSuccessfulResponse()
        {
            TestValidationResult<FssEventData> result = _fssEventDataValidator.TestValidate(_fakeFssEventData);

            Assert.AreEqual(0, result.Errors.Count);
        }
        #endregion

        private static FssEventData GetFssEventData()
        {
            Link linkBatchDetails = new()
            {
                Href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0"
            };
            Link linkBatchStatus = new()
            {
                Href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/status"
            };

            FileLinks fileLinks = new()
            {
                Get = new Link() { Href = @"https://files.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/AVCS_S631-1_Update_Wk45_21_Only.zip" },
            };

            BatchLinks links = new()
            {
                BatchDetails = linkBatchDetails,
                BatchStatus = linkBatchStatus
            };

            return new FssEventData()
            {
                Links = links,
                BusinessUnit = "AVCSData",
                Attributes = new List<Attribute> { },
                BatchId = "83d08093-7a67-4b3a-b431-92ba42feaea0",
                BatchPublishedDate = DateTime.UtcNow,
                Files = new File[] {new() { MIMEType= "application/zip",
                                                                    FileName= "AVCS_S631-1_Update_Wk45_21_Only.zip",
                                                                    FileSize=99073923,
                                                                    Hash="yNpJTWFKhD3iasV8B/ePKw==",
                                                                    Attributes=new List<Attribute> {},
                                                                    Links = fileLinks   }}
            };
        }
    }
}
