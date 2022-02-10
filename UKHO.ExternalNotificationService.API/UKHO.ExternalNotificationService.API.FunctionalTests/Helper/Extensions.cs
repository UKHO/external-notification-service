using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            string bodyJson = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(bodyJson);
        }
    }
}
