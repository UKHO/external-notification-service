using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging;
using FakeItEasy;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using NUnit.Framework;
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

        [SetUp]
        public void Setup()
        {
            _fakeFssEventData = CustomCloudEventBase.GetFssEventData();
            _fakeFssEventDataValidator = A.Fake<IFssEventDataValidator>();
            _fakeFssDataMappingConfiguration = A.Fake<IOptions<FssDataMappingConfiguration>>();
            _fakeFssDataMappingConfiguration.Value.Sources =
                [
                    new() {BusinessUnit = "AVCSData", Source = "fss-AVCSData"},
                    new() {BusinessUnit = "MaritimeSafetyInformation", Source = "fss-MaritimeSafetyInformation"},
                ];
            _fakeFssDataMappingConfiguration.Value.EventHostName = "files.admiralty.co.uk";
            _fakeFssDataMappingConfiguration.Value.PublishHostName = "test/fss";

            _fssEventValidationAndMappingService = new FssEventValidationAndMappingService(_fakeFssEventDataValidator, _fakeFssDataMappingConfiguration);
        }

        [Test]
        public async Task WhenNullBatchIdInRequest_ThenReceiveSuccessfulResponse()
        {
            A.CallTo(() => _fakeFssEventDataValidator.Validate(A<FssEventData>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure> { new("BatchId", "BatchId cannot be blank or null.") }));

            ValidationResult result = await _fssEventValidationAndMappingService.ValidateFssEventData(new FssEventData());

            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.Errors.Single().ErrorMessage, Is.EqualTo("BatchId cannot be blank or null."));
            });
        }

        [Test]
        public async Task WhenValidPayloadInRequest_ThenReceiveOkResponse()
        {
            A.CallTo(() => _fakeFssEventDataValidator.Validate(A<FssEventData>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure>()));

            ValidationResult result = await _fssEventValidationAndMappingService.ValidateFssEventData(_fakeFssEventData);

            Assert.That(result.IsValid);
        }

        [TestCase("AVCSData")]
        [TestCase("MaritimeSafetyInformation")]
        public void WhenValidFssEventDataMappingRequest_ThenReturnCloudEvent(string businessUnit)
        {
            const string batchDetailsUri = "https://test/fss/batch/83d08093-7a67-4b3a-b431-92ba42feaea0";

            CustomCloudEvent customCloudEvent = CustomCloudEventBase.GetCustomCloudEvent(businessUnit);
            CloudEventCandidate<FssEventData> candidate = CustomCloudEventBase.GetCloudEventCandidate<FssEventData>(customCloudEvent);
            CloudEvent result = _fssEventValidationAndMappingService.MapToCloudEvent(candidate);

            string data = Encoding.ASCII.GetString(result.Data);
            FssEventData cloudEventData = JsonSerializer.Deserialize<FssEventData>(data);

            Assert.Multiple(() =>
            {
                Assert.That(result.Type, Is.EqualTo(FssDataMappingValueConstant.Type));
                Assert.That(result.Source, Is.EqualTo(_fakeFssDataMappingConfiguration.Value.Sources.Single(x => x.BusinessUnit == businessUnit).Source));
                Assert.That(cloudEventData.Links.BatchDetails.Href, Is.EqualTo(batchDetailsUri));
                Assert.That(cloudEventData.Links.BatchStatus.Href, Is.EqualTo(batchDetailsUri + "/status"));
                Assert.That(cloudEventData.Files.FirstOrDefault().Links.Get.Href, Is.EqualTo(batchDetailsUri + "/files/AVCS_S631-1_Update_Wk45_21_Only.zip"));
            });
        }

        [Test]
        public void WhenFssEventDataMappingRequestForUnconfiguredBusinessUnit_ThenThrowConfigurationMissingException()
        {
            const string businessUnit = "UnconfiguredBusinessUnit";
            CustomCloudEvent customCloudEvent = CustomCloudEventBase.GetCustomCloudEvent(businessUnit);
            CloudEventCandidate<FssEventData> candidate = CustomCloudEventBase.GetCloudEventCandidate<FssEventData>(customCloudEvent);

            Assert.Throws(Is.TypeOf<ConfigurationMissingException>().And.Message.EqualTo($"Missing FssDataMappingConfiguration configuration for {businessUnit} business unit"),
                delegate
                {
                    _fssEventValidationAndMappingService.MapToCloudEvent(candidate);
                });
        }
    }
}
