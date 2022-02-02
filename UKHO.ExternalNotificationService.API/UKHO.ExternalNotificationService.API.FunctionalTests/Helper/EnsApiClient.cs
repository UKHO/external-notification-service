using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UKHO.ExternalNotificationService.API.FunctionalTests.Model;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Helper
{
    public class EnsApiClient
    {
        static HttpClient httpClient = new HttpClient();
        private readonly string apiHost;

        public EnsApiClient(string apiHost)
        {
            this.apiHost = apiHost;
        }

        /// <summary>
        /// Post Subscription request
        /// </summary>
        /// <param name="d365Payload"></param>
        /// <param name="headerRequest">headerRequest, pass NULL to skip request header</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostEnsApiSubcriptionAsync([FromBody] D365Payload d365Payload, string headerRequest = null)
        {
            string uri = $"{apiHost}/api/subscription";
            string payloadJson = JsonConvert.SerializeObject(d365Payload);
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            { Content = new StringContent(payloadJson, Encoding.UTF8, "application/json") })
            {
                if (headerRequest != null)
                {
                    httpRequestMessage.Headers.Add(headerRequest, string.Empty);
                }

                return await httpClient.SendAsync(httpRequestMessage);
            }
        }
    }
}
