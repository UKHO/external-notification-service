using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.SubscriptionService.Configuration;

namespace UKHO.ExternalNotificationService.SubscriptionService.Helpers
{
    public class AuthTokenProvider : IAuthTokenProvider
    {
        public readonly IOptions<D365CallbackConfiguration> _D365CallbackConfiguration;

        public AuthTokenProvider(IOptions<D365CallbackConfiguration> d365CallbackConfiguration)
        {
            _D365CallbackConfiguration = d365CallbackConfiguration;
        }

        public async Task<string> GetADAccessToken()
        {
            var Token = await GenerateADAccessToken(_D365CallbackConfiguration.Value);
            return Token.AccessToken;
        }

        private static async Task<AuthenticationResult> GenerateADAccessToken(D365CallbackConfiguration callbackConfiguration)
        {
            string[] scopes = new string[] { callbackConfiguration.D365Uri + "/.default" };

            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(callbackConfiguration.ClientId)
                                                .WithClientSecret(callbackConfiguration.ClientSecret)
                                                .WithAuthority(new Uri(callbackConfiguration.MicrosoftOnlineLoginUrl + callbackConfiguration.TenantId))
                                                .Build();

            AuthenticationResult tokenTask = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return tokenTask;

        }
    }
}
