using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.SubscriptionService.Configuration;

namespace UKHO.ExternalNotificationService.SubscriptionService.Helpers
{
    [ExcludeFromCodeCoverage] ////Excluded from code coverage as it has actual http calls 
    public class CallbackClient : ICallbackClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<D365CallbackConfiguration> _d365CallbackConfiguration;

        public CallbackClient(IHttpClientFactory httpClientFactory, IOptions<D365CallbackConfiguration> d365CallbackConfiguration)
        {
            _httpClientFactory = httpClientFactory;
            _d365CallbackConfiguration = d365CallbackConfiguration;
        }

        public async Task<HttpResponseMessage> GetCallbackD365Client(string externalEntityPath, string accessToken, object externalNotificationEntity, string correlationId, CancellationToken cancellationToken)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("D365DataverseApi");
            httpClient.BaseAddress = new Uri(_d365CallbackConfiguration.Value.D365ApiUri);
            httpClient.Timeout = TimeSpan.FromMinutes(Convert.ToDouble(_d365CallbackConfiguration.Value.TimeOutInMins));
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpContent content = new StringContent(JObject.FromObject(externalNotificationEntity).ToString());
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, externalEntityPath)
            { Content = content };

            if (correlationId != string.Empty)
            {
                httpRequestMessage.Headers.Add("X-Correlation-ID", correlationId);
            }

            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
            return response;
        }
    }
}
