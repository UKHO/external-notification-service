using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Models.Request;
using FluentValidation.TestHelper;
using System.Linq;

namespace UKHO.ExternalNotificationService.API.UnitTests.Validation
{
    public class SubscriptionRequestValidatorTest
    {
        private SubscriptionRequestValidator _subscriptionRequestValidator;

        [SetUp]
        public void Setup()
        {
            _subscriptionRequestValidator = new SubscriptionRequestValidator();
        }

        [Test]
        public void WhenNullD365CorrelationIdRequest_ThenReturnBadRequest()
        {
            var model = new SubscriptionRequest
            {
                D365CorrelationId = null
            };

            var result = _subscriptionRequestValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(fb => fb.D365CorrelationId);

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "D365CorrelationId cannot be blank or null."));
        }

        [Test]
        public void WhenEmptyD365CorrelationIdRequest_ThenReturnBadRequest()
        {
            var model = new SubscriptionRequest
            {
                D365CorrelationId = string.Empty
            };

            var result = _subscriptionRequestValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(fb => fb.D365CorrelationId);

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "D365CorrelationId cannot be blank or null."));
        }

        [Test]
        public void WhenNullSubscriptionIdRequest_ThenReturnBadRequest()
        {
            var model = new SubscriptionRequest
            {
                SubscriptionId = null
            };

            var result = _subscriptionRequestValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(fb => fb.SubscriptionId);

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "SubscriptionId cannot be blank or null."));
        }

        [Test]
        public void WhenEmptySubscriptionIdRequest_ThenReturnBadRequest()
        {
            var model = new SubscriptionRequest
            {
                SubscriptionId = string.Empty
            };

            var result = _subscriptionRequestValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(fb => fb.SubscriptionId);

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "SubscriptionId cannot be blank or null."));
        }

        [Test]
        public void WhenNullNotificationTypeRequest_ThenReturnBadRequest()
        {
            var model = new SubscriptionRequest
            {
                NotificationType = null
            };

            var result = _subscriptionRequestValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(fb => fb.NotificationType);

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "NotificationType cannot be blank or null."));
        }

        [Test]
        public void WhenEmptyNotificationTypeRequest_ThenReturnBadRequest()
        {
            var model = new SubscriptionRequest
            {
                NotificationType = string.Empty
            };

            var result = _subscriptionRequestValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(fb => fb.NotificationType);

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "NotificationType cannot be blank or null."));
        }

        [Test]
        public void WhenNullWebhookUrlRequest_ThenReturnBadRequest()
        {
            var model = new SubscriptionRequest
            {
                WebhookUrl = null
            };

            var result = _subscriptionRequestValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(fb => fb.WebhookUrl);

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "WebhookUrl cannot be blank or null."));
        }

        [Test]
        public void WhenEmptyWebhookUrlRequest_ThenReturnBadRequest()
        {
            var model = new SubscriptionRequest
            {
                WebhookUrl = string.Empty
            };

            var result = _subscriptionRequestValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(fb => fb.SubscriptionId);

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "WebhookUrl cannot be blank or null."));
        }

        [Test]
        public void WhenValidSubscriptionRequest_ThenReturnSuccess()
        {
            var model = new SubscriptionRequest
            {
                D365CorrelationId = "6ea03f10-2672-46fb-92a1-5200f6a4fabc",
                IsActive = true,
                NotificationType = "Data test",
                SubscriptionId = "246d71e7-1475-ec11-8943-002248818222",
                WebhookUrl = "https://abc.com"
            };

            var result = _subscriptionRequestValidator.TestValidate(model);
            Assert.AreEqual(0, result.Errors.Count);
        }
    }
}
