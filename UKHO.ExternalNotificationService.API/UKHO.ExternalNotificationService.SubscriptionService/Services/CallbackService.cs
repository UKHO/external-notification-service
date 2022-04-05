using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.SubscriptionService.Helpers;

namespace UKHO.ExternalNotificationService.SubscriptionService.Services
{
    public class CallbackService : ICallbackService
    {
        private readonly IAuthTokenProvider _authTokenProvider;
        private readonly ILogger<CallbackService> _logger;
        private readonly ICallbackClient _callbackClient;

        public CallbackService(IAuthTokenProvider authTokenProvider, ILogger<CallbackService> logger, ICallbackClient callbackClient)
        {
            _authTokenProvider = authTokenProvider;
            _logger = logger;
            _callbackClient = callbackClient;
        }

        public async Task<HttpResponseMessage> CallbackToD365UsingDataverse(string externalEntityPath, object externalNotificationEntity, SubscriptionRequestMessage subscriptionMessage)
        {
            string accessToken = await _authTokenProvider.GetADAccessToken(subscriptionMessage);

            if (accessToken != string.Empty)
            {
                _logger.LogError(EventIds.ErrorInCallbackToD365HttpClient.ToEventId(),
              "Test externalNotificationEntity value externalNotificationEntity :{externalNotificationEntity} and correlationID :{CorrelationId} and accessToken:{accessToken}", JsonConvert.SerializeObject(externalNotificationEntity), subscriptionMessage.CorrelationId, accessToken);

                HttpResponseMessage httpResponse = await _callbackClient.GetCallbackD365Client(externalEntityPath, accessToken, externalNotificationEntity, subscriptionMessage.CorrelationId, CancellationToken.None);

                if (httpResponse.StatusCode != HttpStatusCode.NoContent)
                {
                    _logger.LogError(EventIds.ErrorInCallbackToD365HttpClient.ToEventId(),
                "Callback to D365 using Dataverse failed with Status:{StatusCode} and RequestUri:{RequestUri} and SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", httpResponse.StatusCode, httpResponse.RequestMessage.RequestUri, subscriptionMessage.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
                    return httpResponse;
                }
                _logger.LogInformation(EventIds.CallbackToD365Completed.ToEventId(),
                    "Callback to D365 using Dataverse succeeded with Status:{StatusCode} and RequestUri:{RequestUri} and SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", httpResponse.StatusCode, httpResponse.RequestMessage.RequestUri, subscriptionMessage.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
                return httpResponse;
            }
            _logger.LogError(EventIds.ErrorInCallbackToD365HttpClient.ToEventId(),
                "As Authorization to AD Token failed with Status:{StatusCode} and SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", HttpStatusCode.Unauthorized, subscriptionMessage.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
            return new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized };
        }
    }
}
