using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.SubscriptionService.Configuration;

namespace UKHO.ExternalNotificationService.SubscriptionService.Helpers
{
    [ExcludeFromCodeCoverage] ////Excluded from code coverage as it has ADD interaction
    public class AuthTokenProvider : IAuthTokenProvider
    {
        private readonly IOptions<D365CallbackConfiguration> _d365CallbackConfiguration;
        private readonly IOptions<AzureADConfiguration> _azureADConfiguration;
        private readonly ILogger<AuthTokenProvider> _logger;

        public AuthTokenProvider(IOptions<D365CallbackConfiguration> d365CallbackConfiguration, IOptions<AzureADConfiguration> azureADConfiguration, ILogger<AuthTokenProvider> logger)
        {
            _d365CallbackConfiguration = d365CallbackConfiguration;
            _azureADConfiguration = azureADConfiguration;
            _logger = logger;
        }

        public async Task<string> GetADAccessToken(SubscriptionRequestMessage subscriptionMessage)
        { 
            try
            {
                AuthenticationResult token = await GenerateADAccessToken(_d365CallbackConfiguration.Value.D365Uri, _azureADConfiguration.Value);
                return token.AccessToken;
            }
            catch (Exception ex)
            {
               _logger.LogError(EventIds.ADAuthenticationFailed.ToEventId(),
               "AD Authentication failed with message:{ex} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", ex.Message, subscriptionMessage.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
                return string.Empty;
            }             
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
