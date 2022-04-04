using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.AzureEventGridDomain;
using UKHO.ExternalNotificationService.SubscriptionService.D365Callback;

namespace UKHO.ExternalNotificationService.SubscriptionService.Helpers
{
    public static class CommonHelper
    {      
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger, int retryCount, double sleepDuration)
        {
            return Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.ServiceUnavailable)
                .OrResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(retryCount, (retryAttempt) =>
                {
                    return TimeSpan.FromSeconds(Math.Pow(sleepDuration, (retryAttempt - 1)));
                }, async (response, timespan, retryAttempt, _) =>
                {
                    KeyValuePair<string, IEnumerable<string>> retryAfterHeader = response.Result.Headers.FirstOrDefault(h => h.Key.ToLowerInvariant() == "retry-after");
                    KeyValuePair<string, IEnumerable<string>> correlationId = response.Result.RequestMessage.Headers.FirstOrDefault(h => h.Key.ToLowerInvariant() == "x-correlation-id");
                    int retryAfter = 0;
                    if (response.Result.StatusCode == HttpStatusCode.TooManyRequests && retryAfterHeader.Value != null && retryAfterHeader.Value.Any())
                    {
                        retryAfter = int.Parse(retryAfterHeader.Value.First());
                        await Task.Delay(TimeSpan.FromMilliseconds(retryAfter));
                    }
                    logger
                    .LogInformation(EventIds.RetryHttpClientD365CallbackRequest.ToEventId(), "Re-trying D365 Callback service request with uri {RequestUri} and delay {delay}ms and retry attempt {retry} with _X-Correlation-ID:{correlationId} as previous request was responded with {StatusCode}.",
                    response.Result.RequestMessage.RequestUri, timespan.Add(TimeSpan.FromMilliseconds(retryAfter)).TotalMilliseconds, retryAttempt, correlationId.Value, response.Result.StatusCode);
                });
        }
              
        public static ExternalNotificationEntity GetExternalNotificationEntity(SubscriptionRequestResult subscriptionRequestResult, bool isActive, int statusCode, int statecode)
        {
            ExternalNotificationEntity externalNotificationEntity = new();
            if (subscriptionRequestResult.ProvisioningState == "Succeeded")
            {
                externalNotificationEntity.ResponseStatusCode = statusCode;
                externalNotificationEntity.ResponseDetails = isActive ? $"Successfully added subscription @Time: { DateTime.UtcNow}" : $"Successfully removed subscription @Time: { DateTime.UtcNow}";
                externalNotificationEntity.statecode = statecode;
            }

            if (subscriptionRequestResult.ProvisioningState == "Failed")
            {
                externalNotificationEntity.ResponseStatusCode = statusCode;
                externalNotificationEntity.ResponseDetails = isActive ? $"Failed to add subscription @Time: {DateTime.UtcNow} with exception {subscriptionRequestResult.ErrorMessage}" : $"Failed to remove subscription @Time: {DateTime.UtcNow} with exception {subscriptionRequestResult.ErrorMessage}";
                externalNotificationEntity.statecode = statecode;
            }
            return externalNotificationEntity;
        }
    }
}
