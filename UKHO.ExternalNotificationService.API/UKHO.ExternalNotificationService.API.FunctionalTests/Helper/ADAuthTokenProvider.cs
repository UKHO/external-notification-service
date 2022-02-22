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
            ensAccessToken = await GenerateEnsToken(s_testConnfig.EnsClientId, s_testConnfig.EnsApimClientId, s_testConnfig.EnsClientSecret);
            return ensAccessToken;
        }

        private static async Task<string> GenerateEnsToken(string ensClientId, string ensApimClientId, string ensClientSecret)
        {
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
             .Create(ensApimClientId)
             .WithClientSecret(ensClientSecret)
            .WithAuthority($"{s_testConnfig.MicrosoftOnlineLoginUrl}{s_testConnfig.TenantId}/oauth2/token")
            .Build();

            AuthenticationResult result =await confidentialClientApplication.AcquireTokenForClient(
            new string[] { $"{ensClientId}/.default" }).ExecuteAsync();

            string accessToken = result.AccessToken;
            return accessToken;
        }

    }
}
