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
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services
{
    [TestFixture]
    public class ScsS100EventValidationAndMappingServiceTest
    {
        private IScsEventDataValidator _fakeScsEventDataValidator;
        private IOptions<ScsS100DataMappingConfiguration> _fakeScsS100DataMappingConfiguration;
        private ScsS100EventValidationAndMappingService _scsS100EventValidationAndMappingService;
        private ScsEventData _fakeScsEventData;
        private CustomCloudEvent _fakeCustomCloudEvent;
        private CloudEventCandidate<ScsEventData> _fakeCloudEventCandidate;

        [SetUp]
        public void Setup()
        {
            _fakeScsEventData = CustomCloudEventBase.GetScsEventData();
            _fakeCustomCloudEvent = CustomCloudEventBase.GetCustomCloudEvent();
            _fakeCustomCloudEvent.Data = _fakeScsEventData;
            _fakeCloudEventCandidate = CustomCloudEventBase.GetCloudEventCandidate<ScsEventData>(_fakeCustomCloudEvent);

            _fakeScsEventDataValidator = A.Fake<IScsEventDataValidator>();
            _fakeScsS100DataMappingConfiguration = A.Fake<IOptions<ScsS100DataMappingConfiguration>>();
            _fakeScsS100DataMappingConfiguration.Value.Source = "ScsS100-Test";

            _scsS100EventValidationAndMappingService = new ScsS100EventValidationAndMappingService(
                _fakeScsEventDataValidator,
                _fakeScsS100DataMappingConfiguration);
        }

        #region ValidateScsEventData
        [Test]
        public async Task WhenInvalidPayloadInRequest_ThenReceiveValidationErrors()
        {
            A.CallTo(() => _fakeScsEventDataValidator.Validate(A<ScsEventData>.Ignored))
                .Returns(new ValidationResult(new List<ValidationFailure>
                {
                    new ValidationFailure("ProductType", "ProductType cannot be blank or null.")
                }));

            ValidationResult result = await _scsS100EventValidationAndMappingService.ValidateScsEventData(new ScsEventData());

            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.Errors.Single().ErrorMessage, Is.EqualTo("ProductType cannot be blank or null."));
            });
        }

        [Test]
        public async Task WhenValidPayloadInRequest_ThenReceiveOkResponse()
        {
            A.CallTo(() => _fakeScsEventDataValidator.Validate(A<ScsEventData>.Ignored))
                .Returns(new ValidationResult(new List<ValidationFailure>()));

            ValidationResult result = await _scsS100EventValidationAndMappingService.ValidateScsEventData(_fakeScsEventData);

            Assert.That(result.IsValid);
        }
        #endregion

        #region ScsS100EventDataMapping
        [Test]
        public void WhenValidScsS100EventDataMappingRequest_ThenReturnCloudEvent()
        {
            CloudEvent result = _scsS100EventValidationAndMappingService.MapToCloudEvent(_fakeCloudEventCandidate);

            string data = Encoding.ASCII.GetString(result.Data);
            ScsEventData cloudEventData = JsonSerializer.Deserialize<ScsEventData>(data);

            Assert.Multiple(() =>
            {
                Assert.That(result.Type, Is.EqualTo(ScsS100DataMappingValueConstant.Type));
                Assert.That(result.Source, Is.EqualTo(_fakeScsS100DataMappingConfiguration.Value.Source));
                Assert.That(cloudEventData.ProductType, Is.EqualTo(_fakeScsEventData.ProductType));
            });
        }
        #endregion
    }
}   
