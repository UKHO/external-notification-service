using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public async Task CallbackToD365UsingDataverse(string externalEntityPath, object externalNotificationEntity, string d365CorrelationId, string correlationId)
        {
            string[] subscriptionId = externalEntityPath.Split("s(");
            string accessToken = await _authTokenProvider.GetADAccessToken();

            HttpResponseMessage httpResponse = await _callbackClient.GetCallbackD365Client(HttpMethod.Patch, externalEntityPath, accessToken, externalNotificationEntity, CancellationToken.None, correlationId);
            
                    if (httpResponse.StatusCode != System.Net.HttpStatusCode.NoContent)
                    {                        
                        _logger.LogError(EventIds.CallbackToD365UsingDataverseError.ToEventId(),
                    "Callback to D365 using Dataverse failed with statusCode:{StatusCode} and Requesturi:{RequestUri} and SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", httpResponse.StatusCode, httpResponse.RequestMessage, subscriptionId[1].Remove(subscriptionId[1].Length - 1), d365CorrelationId, correlationId);

                        //////throw new Exception(error); //should use HttpRequestException ??
                    }
                    _logger.LogInformation(EventIds.CallbackToD365UsingDataverseError.ToEventId(),
                    "Callback to D365 using Dataverse success with statusCode:{StatusCode} and Requesturi:{RequestUri} and SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", httpResponse.StatusCode, httpResponse.RequestMessage, subscriptionId[1].Remove(subscriptionId[1].Length - 1), d365CorrelationId, correlationId);
                }
            }
        }
