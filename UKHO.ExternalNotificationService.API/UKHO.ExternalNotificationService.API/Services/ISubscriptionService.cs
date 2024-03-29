﻿using FluentValidation.Results;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Services
{
    public interface ISubscriptionService
    {
        Task<ValidationResult> ValidateD365PayloadRequest(D365Payload d365Payload);
        SubscriptionRequest ConvertToSubscriptionRequestModel(D365Payload d365Payload);
        Task AddSubscriptionRequest(SubscriptionRequest subscriptionRequest, NotificationType notificationType, string correlationId);
    }
}
