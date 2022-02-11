﻿using System.Threading.Tasks;
using FluentValidation.Results;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Services
{
    public interface ISubscriptionService
    {
        Task<ValidationResult> ValidateD365PayloadRequest(D365Payload d365Payload);

        SubscriptionRequest ConvertToSubscriptionRequestModel(D365Payload d365Payload);
        SubscriptionRequestMessage GetSubscriptionRequestMessage(SubscriptionRequest subscriptionRequest);
    }
}
