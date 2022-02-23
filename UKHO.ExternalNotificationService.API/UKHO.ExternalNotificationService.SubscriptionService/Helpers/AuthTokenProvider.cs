using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.SubscriptionService.Configuration;

namespace UKHO.ExternalNotificationService.SubscriptionService.Helpers
{
    [ExcludeFromCodeCoverage] ////Excluded from code coverage as it has ADD interaction
    public class AuthTokenProvider : IAuthTokenProvider
    {
        public readonly IOptions<D365CallbackConfiguration> _d365CallbackConfiguration;
        public readonly IOptions<AzureADConfiguration> _azureADConfiguration;

        public AuthTokenProvider(IOptions<D365CallbackConfiguration> d365CallbackConfiguration, IOptions<AzureADConfiguration> azureADConfiguration)
        {
            _d365CallbackConfiguration = d365CallbackConfiguration;
            _azureADConfiguration = azureADConfiguration;
        }

        public async Task<string> GetADAccessToken()
        {           
            AuthenticationResult Token = await GenerateADAccessToken(_d365CallbackConfiguration.Value.D365Uri, _azureADConfiguration.Value);            
            return Token.AccessToken;
        }

        private static async Task<AuthenticationResult> GenerateADAccessToken(string d365Uri, AzureADConfiguration ensAuthConfiguration)
        {
            string[] scopes = new string[] { d365Uri + "/.default" };

            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(ensAuthConfiguration.ClientId)
                                                .WithClientSecret(ensAuthConfiguration.ClientSecret)
                                                .WithAuthority(new Uri(ensAuthConfiguration.MicrosoftOnlineLoginUrl + ensAuthConfiguration.TenantId))
                                                .Build();

            AuthenticationResult tokenTask = await app.AcquireTokenForClient(scopes).ExecuteAsync();            
            return tokenTask;
        }
    }
}
