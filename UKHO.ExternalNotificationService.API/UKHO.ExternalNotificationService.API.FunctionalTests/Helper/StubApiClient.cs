using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Helper
{
    public class StubApiClient
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _apiHost;

        public StubApiClient(string apiHost)
        {
            _apiHost = apiHost;
        }

        public async Task<HttpResponseMessage> PostStubApiCommandToReturnStatusAsync([FromBody] JsonObject request, string subject, HttpStatusCode? statusCode, string accessToken = null)
        {
            string uri = $"{_apiHost}/webhook/notification/command-to-return-status/{subject}";
            if (statusCode.HasValue)
            {
                uri += $"/{(int)statusCode.Value}";
            }
            string payloadJson = JsonSerializer.Serialize(request);
            HttpRequestMessage httpRequestMessage = GetHttpRequestMessage(HttpMethod.Post, accessToken, uri, payloadJson);

            return await _httpClient.SendAsync(httpRequestMessage);
        }

        public async Task<HttpResponseMessage> GetStubApiCacheReturnStatusAsync(string request, string accessToken = null)
        {
            string uri = $"{_apiHost}/webhook/notification?Subject={request}";
            string payloadJson = string.Empty;
            HttpRequestMessage httpRequestMessage = GetHttpRequestMessage(HttpMethod.Get, accessToken, uri, payloadJson);

            return await _httpClient.SendAsync(httpRequestMessage);
        }

        private static HttpRequestMessage GetHttpRequestMessage(HttpMethod type, string accessToken, string uri, string payloadJson)
        {
            HttpRequestMessage httpRequestMessage = new(type, uri)
            {
                Content = new StringContent(payloadJson, Encoding.UTF8, "application/json")
            };
            if (accessToken != null)
            {
                httpRequestMessage.SetBearerToken(accessToken);
            }
            return httpRequestMessage;
        }
    }
}
