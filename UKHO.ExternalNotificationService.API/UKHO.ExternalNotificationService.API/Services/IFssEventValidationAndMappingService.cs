﻿using Azure.Messaging;
using FluentValidation.Results;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.EventModel;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Services
{
    public interface IFssEventValidationAndMappingService
    {
        Task<ValidationResult> ValidateFssEventData(FssEventData fssEventData);

        CloudEvent MapToCloudEvent(CloudEventCandidate<FssEventData> candidate);
    }
}
