
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.SubscriptionService.Helpers;

namespace UKHO.ExternalNotificationService.SubscriptionService.Services
{
    public class CallbackService : ICallbackService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthTokenProvider _authTokenProvider;
        private readonly ILogger<CallbackService> _logger;        

        public Uri BaseAddress { get { return _httpClient.BaseAddress; } }

        public CallbackService(IHttpClientFactory httpClientFactory, IAuthTokenProvider authTokenProvider, ILogger<CallbackService> logger)
        {
            _authTokenProvider = authTokenProvider;
            _logger = logger;
                           
            _httpClient = httpClientFactory.CreateClient("D365DataverseApi");
            _httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            _httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task CallbackToD365UsingDataverse(string externalEntityPath, object externalNotificationEntity, string D365CorrelationId, string CorrelationId)
        {
            using (var message = new HttpRequestMessage(HttpMethod.Patch, externalEntityPath))
            {
                string[] subscriptionId = externalEntityPath.Split("s(");                
                //Get the access token that is required for authentication.
                var accessToken = await _authTokenProvider.GetADAccessToken();
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                message.Content = new StringContent(JObject.FromObject(externalNotificationEntity).ToString());
                message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                using (HttpResponseMessage response = await _httpClient.SendAsync(message))
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                    {                        
                        _logger.LogError(EventIds.CallbackToD365UsingDataverseError.ToEventId(),
                    "Callback to D365 using Dataverse failed with statusCode:{StatusCode} and Requesturi:{RequestUri} and SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", response.StatusCode, response.RequestMessage, subscriptionId[1].PadRight(1), D365CorrelationId, CorrelationId);

                        //////throw new Exception(error); //should use HttpRequestException ??
                    }
                }
            }


        }
    }
}
