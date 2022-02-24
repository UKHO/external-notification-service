using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Logging;
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

        public async Task<HttpResponseMessage> CallbackToD365UsingDataverse(string externalEntityPath, object externalNotificationEntity, string d365CorrelationId, string correlationId)
        {
            string[] subscriptionId = externalEntityPath.Split("s(");
            string subId = subscriptionId[1].Remove(subscriptionId[1].Length - 1);
            string accessToken = await _authTokenProvider.GetADAccessToken();

            _logger.LogInformation(EventIds.BeforeCallbackToD365.ToEventId(),
            "AD Authentication token generated for SubscriptionId:{SubscriptionId} and before calling to D365 using dataverse api for _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subId, d365CorrelationId, correlationId);
            HttpResponseMessage httpResponse = await _callbackClient.GetCallbackD365Client(HttpMethod.Patch, externalEntityPath, accessToken, externalNotificationEntity, correlationId, CancellationToken.None);

            if (httpResponse.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                _logger.LogError(EventIds.ErrorInCallbackToD365HttpClient.ToEventId(),
            "Callback to D365 using Dataverse failed with statusCode:{StatusCode} and Requesturi:{RequestUri} and SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", httpResponse.StatusCode, httpResponse.RequestMessage.RequestUri, subId, d365CorrelationId, correlationId);
              return httpResponse;
            }
                _logger.LogInformation(EventIds.CallbackToD365Completed.ToEventId(),
            "Callback to D365 using Dataverse succeeded with statusCode:{StatusCode} and Requesturi:{RequestUri} and SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", httpResponse.StatusCode, httpResponse.RequestMessage.RequestUri, subId, d365CorrelationId, correlationId);
            return httpResponse;
        }
    }
}
