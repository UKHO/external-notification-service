using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.SubscriptionService.Configuration;

namespace UKHO.ExternalNotificationService.SubscriptionService.Helpers
{
    [ExcludeFromCodeCoverage] ////Excluded from code coverage as it has ADD interaction
    public class AuthTokenProvider : IAuthTokenProvider
    {
        private readonly IOptions<D365CallbackConfiguration> _d365CallbackConfiguration;        
        private readonly ILogger<AuthTokenProvider> _logger;

        public AuthTokenProvider(IOptions<D365CallbackConfiguration> d365CallbackConfiguration, ILogger<AuthTokenProvider> logger)
        {
            _d365CallbackConfiguration = d365CallbackConfiguration;
            _logger = logger;
        }

        public async Task<string> GetADAccessToken(SubscriptionRequestMessage subscriptionMessage)
        { 
            try
            {
                string[] scopes = new[] { _d365CallbackConfiguration.Value.D365Uri + "/.default" };
                var defaultCredential = new DefaultAzureCredential();
                AccessToken tokenResponse = await defaultCredential.GetTokenAsync(new TokenRequestContext(scopes));
                return tokenResponse.Token;
            }
            catch (Exception ex)
            {
               _logger.LogError(EventIds.ADAuthenticationFailed.ToEventId(),
               "AD Authentication failed with message:{ex} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", ex.Message, subscriptionMessage.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
                return string.Empty;
            }             
        }        
    }
}
