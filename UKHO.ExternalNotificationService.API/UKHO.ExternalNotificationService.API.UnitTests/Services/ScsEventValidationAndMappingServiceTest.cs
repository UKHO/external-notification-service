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
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services
{
    [TestFixture]
    public class ScsEventValidationAndMappingServiceTest
    {
        private IScsEventDataValidator _fakeScsEventDataValidator;
        private IOptions<ScsDataMappingConfiguration> _fakeScsDataMappingConfiguration;
        private ScsEventValidationAndMappingService _scsEventValidationAndMappingService;
        private ScsEventData _fakeScsEventData;
        private CustomCloudEvent _fakeCustomCloudEvent;

        [SetUp]
        public void Setup()
        {
            _fakeScsEventData = CustomCloudEventBase.GetScsEventData();
            _fakeCustomCloudEvent = CustomCloudEventBase.GetCustomCloudEvent();
            _fakeCustomCloudEvent.Data = _fakeScsEventData;

            _fakeScsEventDataValidator = A.Fake<IScsEventDataValidator>();
            _fakeScsDataMappingConfiguration = A.Fake<IOptions<ScsDataMappingConfiguration>>();
            _fakeScsDataMappingConfiguration.Value.Source = "Scs-Test";

            _scsEventValidationAndMappingService = new ScsEventValidationAndMappingService(_fakeScsEventDataValidator, _fakeScsDataMappingConfiguration);
        }

        #region ValidateScsEventData
        [Test]
        public async Task WhenNullBatchIdInRequest_ThenReceiveSuccessfulResponse()
        {
            A.CallTo(() => _fakeScsEventDataValidator.Validate(A<ScsEventData>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure>
                    {new ValidationFailure("ProductType", "ProductType cannot be blank or null.")}));

            ValidationResult result = await _scsEventValidationAndMappingService.ValidateScsEventData(new ScsEventData());

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("ProductType cannot be blank or null.", result.Errors.Single().ErrorMessage);
        }

        [Test]
        public async Task WhenValidPayloadInRequest_ThenReceiveOkResponse()
        {
            A.CallTo(() => _fakeScsEventDataValidator.Validate(A<ScsEventData>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure>()));

            ValidationResult result = await _scsEventValidationAndMappingService.ValidateScsEventData(_fakeScsEventData);

            Assert.IsTrue(result.IsValid);
        }
        #endregion

        #region ScsEventDataMapping
        [Test]
        public void WhenValidScsEventDataMappingRequest_ThenReturnCloudEvent()
        {
            const string correlationId = "7b838400-7d73-4a64-982b-f426bddc1296";

            CloudEvent result = _scsEventValidationAndMappingService.ScsEventDataMapping(_fakeCustomCloudEvent, correlationId);

            string data = Encoding.ASCII.GetString(result.Data);
            ScsEventData cloudEventData = JsonConvert.DeserializeObject<ScsEventData>(data);

            Assert.AreEqual(ScsDataMappingValueConstant.Type, result.Type);
            Assert.AreEqual(_fakeScsDataMappingConfiguration.Value.Source, result.Source);
            Assert.AreEqual(_fakeScsEventData.ProductType, cloudEventData.ProductType);
        }
        #endregion
    }
}
