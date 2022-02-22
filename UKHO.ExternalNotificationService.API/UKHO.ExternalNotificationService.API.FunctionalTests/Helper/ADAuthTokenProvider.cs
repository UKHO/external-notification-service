using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Helper
{
    public class ADAuthTokenProvider
    {
        private string ensAccessToken { get; set; }
        static readonly TestConfiguration s_testConnfig = new();
        public async Task<string> GetEnsAuthToken()
        {
            ensAccessToken = await GenerateEnsToken(s_testConnfig.ClientId, s_testConnfig.D365ClientId, s_testConnfig.D365Secret);
            return ensAccessToken;
        }

        private static async Task<string> GenerateEnsToken(string clientId, string d365ClientId, string d365Secret)
        {
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
             .Create(d365ClientId)
             .WithClientSecret(d365Secret)
            .WithAuthority($"{s_testConnfig.MicrosoftOnlineLoginUrl}{s_testConnfig.D365TenantId}/oauth2/token")
            .Build();

            AuthenticationResult result =await confidentialClientApplication.AcquireTokenForClient(
            new string[] { $"{clientId}/.default" }).ExecuteAsync();

            string accessToken = result.AccessToken;
            return accessToken;
        }

    }
}
