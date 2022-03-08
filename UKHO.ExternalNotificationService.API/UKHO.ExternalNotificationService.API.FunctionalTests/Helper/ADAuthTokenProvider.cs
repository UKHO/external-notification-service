using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Helper
{
    public class ADAuthTokenProvider
    {
        private string EnsAccessToken { get; set; }
        static readonly TestConfiguration s_testConfig = new();
        public async Task<string> GetEnsAuthToken()
        {
            EnsAccessToken = await GenerateEnsToken(s_testConfig.ClientId, s_testConfig.D365ClientId, s_testConfig.D365Secret);
            return EnsAccessToken;
        }

        private static async Task<string> GenerateEnsToken(string clientId, string d365ClientId, string d365Secret)
        {
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
             .Create(d365ClientId)
             .WithClientSecret(d365Secret)
             .WithAuthority(new Uri(s_testConfig.MicrosoftOnlineLoginUrl + s_testConfig.D365TenantId))
             .Build();

            AuthenticationResult result = await confidentialClientApplication.AcquireTokenForClient(
            new string[] { $"{clientId}/.default" }).ExecuteAsync();

            string accessToken = result.AccessToken;
            return accessToken;
        }

    }
}
