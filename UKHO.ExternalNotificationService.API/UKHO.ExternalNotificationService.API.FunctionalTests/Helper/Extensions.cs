using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Helper
{
    public static class Extensions
    {

        /// <summary>
        /// Reads response body json as given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpResponseMessage"></param>
        /// <returns></returns>
        public static async Task<T> ReadAsTypeAsync<T>(this HttpResponseMessage httpResponseMessage)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string bodyJson = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(bodyJson,options);
        }

        public static void SetBearerToken(this HttpRequestMessage requestMessage, string accessToken)
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}
