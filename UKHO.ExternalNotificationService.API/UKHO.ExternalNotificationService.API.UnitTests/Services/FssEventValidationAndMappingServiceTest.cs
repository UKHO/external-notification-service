using Azure.Messaging;
using FakeItEasy;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.API.UnitTests.BaseClass;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Exceptions;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services
{
    [TestFixture]
    public class FssEventValidationAndMappingServiceTest
    {
        private IFssEventDataValidator _fakeFssEventDataValidator;
        private IOptions<FssDataMappingConfiguration> _fakeFssDataMappingConfiguration;
        private FssEventValidationAndMappingService _fssEventValidationAndMappingService;
        private FssEventData _fakeFssEventData;
        private CustomCloudEvent _fakeCustomCloudEvent;

        [SetUp]
        public void Setup()
        {
            _fakeFssEventData = CustomCloudEventBase.GetFssEventData();
            _fakeCustomCloudEvent = CustomCloudEventBase.GetCustomCloudEvent();
            _fakeFssEventDataValidator = A.Fake<IFssEventDataValidator>();
            _fakeFssDataMappingConfiguration = A.Fake<IOptions<FssDataMappingConfiguration>>();
            _fakeFssDataMappingConfiguration.Value.Sources =
                new List<FssDataMappingConfiguration.SourceConfiguration>
                {
                    new() {BusinessUnit = "AVCSData", Source = "fss-AVCSData"},
                    new() {BusinessUnit = "MaritimeSafetyInformation", Source = "fss-MaritimeSafetyInformation"},
                };
            _fakeFssDataMappingConfiguration.Value.EventHostName = "files.admiralty.co.uk";
            _fakeFssDataMappingConfiguration.Value.PublishHostName = "test/fss";

            _fssEventValidationAndMappingService = new FssEventValidationAndMappingService(_fakeFssEventDataValidator, _fakeFssDataMappingConfiguration);
        }

        [Test]
        public async Task WhenNullBatchIdInRequest_ThenReceiveSuccessfulResponse()
        {
            A.CallTo(() => _fakeFssEventDataValidator.Validate(A<FssEventData>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure>
                    {new ValidationFailure("BatchId", "BatchId cannot be blank or null.")}));

            ValidationResult result = await _fssEventValidationAndMappingService.ValidateFssEventData(new FssEventData());

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("BatchId cannot be blank or null.", result.Errors.Single().ErrorMessage);
        }

        [Test]
        public async Task WhenValidPayloadInRequest_ThenReceiveOkResponse()
        {
            A.CallTo(() => _fakeFssEventDataValidator.Validate(A<FssEventData>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure>()));

            ValidationResult result = await _fssEventValidationAndMappingService.ValidateFssEventData(_fakeFssEventData);

            Assert.IsTrue(result.IsValid);
        }

        [TestCase("AVCSData")]
        [TestCase("MaritimeSafetyInformation")]
        public void WhenValidFssEventDataMappingRequest_ThenReturnCloudEvent(string businessUnit)
        {
            const string correlationId = "7b838400-7d73-4a64-982b-f426bddc1296";
            const string batchDetailsUri = "https://test/fss/batch/83d08093-7a67-4b3a-b431-92ba42feaea0";

            CustomCloudEvent customCloudEvent = CustomCloudEventBase.GetCustomCloudEvent(businessUnit);
            CloudEvent result = _fssEventValidationAndMappingService.FssEventDataMapping(customCloudEvent, correlationId);

            string data = Encoding.ASCII.GetString(result.Data);
            FssEventData cloudEventData = JsonConvert.DeserializeObject<FssEventData>(data);

            Assert.AreEqual(FssDataMappingValueConstant.Type, result.Type);
            Assert.AreEqual(_fakeFssDataMappingConfiguration.Value.Sources.Single(x => x.BusinessUnit == businessUnit).Source, result.Source);
            Assert.AreEqual(batchDetailsUri, cloudEventData.Links.BatchDetails.Href);
            Assert.AreEqual(batchDetailsUri + "/status", cloudEventData.Links.BatchStatus.Href);
            Assert.AreEqual(batchDetailsUri + "/files/AVCS_S631-1_Update_Wk45_21_Only.zip", cloudEventData.Files.FirstOrDefault().Links.Get.Href);
        }

        [Test]
        public void WhenFssEventDataMappingRequestForUnconfiguredBusinessUnit_ThenThrowConfigurationMissingException()
        {
            const string businessUnit = "UnconfiguredBusinessUnit";
            const string correlationId = "7b838400-7d73-4a64-982b-f426bddc1296";
            CustomCloudEvent customCloudEvent = CustomCloudEventBase.GetCustomCloudEvent(businessUnit);

            Assert.Throws(Is.TypeOf<ConfigurationMissingException>().And.Message.EqualTo($"Missing FssDataMappingConfiguration configuration for {businessUnit} business unit"),
                delegate
                {
                    _fssEventValidationAndMappingService.FssEventDataMapping(customCloudEvent, correlationId);
                });
        }
    }
}
