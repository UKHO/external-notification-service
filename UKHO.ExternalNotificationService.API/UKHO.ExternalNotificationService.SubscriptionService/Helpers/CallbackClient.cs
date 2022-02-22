using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using UKHO.ExternalNotificationService.SubscriptionService.Configuration;

namespace UKHO.ExternalNotificationService.SubscriptionService.Helpers
{
    public class CallbackClient : ICallbackClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<D365CallbackConfiguration> _d365CallbackConfiguration;

        public CallbackClient(IHttpClientFactory httpClientFactory, IOptions<D365CallbackConfiguration> d365CallbackConfiguration)
        {
            _httpClientFactory = httpClientFactory;
            _d365CallbackConfiguration= d365CallbackConfiguration;
        }

         public async Task<HttpResponseMessage> GetCallbackD365Client(HttpMethod method, string externalEntityPath, string accessToken, object externalNotificationEntity, CancellationToken cancellationToken, string correlationId)
        {

            ////////var retryPolicy = HttpPolicyExtensions
            ////////     .HandleTransientHttpError() // HttpRequestException, 5XX and 408
            ////////     .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

            HttpClient httpClient = _httpClientFactory.CreateClient("D365DataverseApi");
            httpClient.BaseAddress = new Uri(_d365CallbackConfiguration.Value.D365ApiUri);
            httpClient.Timeout = TimeSpan.FromMinutes(Convert.ToDouble(_d365CallbackConfiguration.Value.TimeOutInMins));
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
           

            HttpContent content = new StringContent(JObject.FromObject(externalNotificationEntity).ToString());
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            using var httpRequestMessage = new HttpRequestMessage(method, externalEntityPath)
            { Content = content };        

            if (correlationId != "")
            {
                httpRequestMessage.Headers.Add("X-Correlation-ID", correlationId);
            }

            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
            return response;            
        }       
    }
}
