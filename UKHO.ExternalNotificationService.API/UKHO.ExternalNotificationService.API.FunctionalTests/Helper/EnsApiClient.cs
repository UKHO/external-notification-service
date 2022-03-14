using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.API.FunctionalTests.Model;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Helper
{
    public class EnsApiClient
    {
        static readonly HttpClient s_httpClient = new();
        private readonly string _apiHost;

        public EnsApiClient(string apiHost)
        {
            _apiHost = apiHost;
        }

        /// <summary>
        /// Post Subscription request
        /// </summary>
        /// <param name="d365Payload"></param>
        /// <param name="accessToken">Access Token, pass NULL to skip auth header</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostEnsApiSubscriptionAsync([FromBody] D365Payload d365Payload, string accessToken = null)
        {
            string uri = $"{_apiHost}/api/subscription";
            string payloadJson = JsonConvert.SerializeObject(d365Payload);
            HttpRequestMessage httpRequestMessage = GetHttpRequestMessage(accessToken, uri, payloadJson);

            return await s_httpClient.SendAsync(httpRequestMessage);
        }

        public async Task<HttpResponseMessage> OptionEnsApiSubscriptionAsync(string headerRequest = null, string headerRequestValue = null, string accessToken = null)
        {
            string uri = $"{_apiHost}/api/webhook";       
            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Options, uri);           
            if (headerRequest != null)
            {
                httpRequestMessage.Headers.Add(headerRequest, headerRequestValue);
            }
            if(accessToken!=null)
            {
                httpRequestMessage.SetBearerToken(accessToken);
            }
            return await s_httpClient.SendAsync(httpRequestMessage);
        }

        public async Task<HttpResponseMessage> PostEnsWebhookNewEventPublishedAsync([FromBody] JObject request, string accessToken = null)
        {
            string uri = $"{_apiHost}/api/webhook";
            string payloadJson = JsonConvert.SerializeObject(request);
            using HttpRequestMessage httpRequestMessage = GetHttpRequestMessage(accessToken, uri, payloadJson);

            return await s_httpClient.SendAsync(httpRequestMessage);
        }

        private static HttpRequestMessage GetHttpRequestMessage(string accessToken, string uri, string payloadJson)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(payloadJson, Encoding.UTF8, "application/json")
            };
            if (accessToken != null)
            {
                httpRequestMessage.SetBearerToken(accessToken);
            }

            return httpRequestMessage;
        }

        /// <summary>
        /// Get Ens Call back status
        /// </summary>
        /// <param name="d365ApiStubUrl"></param>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>

        public static async Task<HttpResponseMessage> GetEnsCallBackAsync(string stubBaseUri, string subscriptionId=null)
        {
            string uri =$"{stubBaseUri}/api/dynamics";
            if (subscriptionId!=null)
            {
                uri += $"?subscriptionid={subscriptionId}";
            }                     
            using var httpRequestMessage =new HttpRequestMessage(HttpMethod.Get, uri);
            return await s_httpClient.SendAsync(httpRequestMessage);
        }


        public static async Task<HttpResponseMessage> PostStubCommandToFailAsync(string stubBaseUri, string subscriptionId,int? statusCode)
        {
             string uri = $"{stubBaseUri}/api/dynamics/command-to-return-status/{subscriptionId}";

            if (statusCode!=null)
            {
                uri += $"/{statusCode}";
            }
                       
            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            return await s_httpClient.SendAsync(httpRequestMessage);
        }

       


    }
}
