using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.SubscriptionService.Configuration;

namespace UKHO.ExternalNotificationService.SubscriptionService.Helpers
{
    public class AuthTokenProvider : IAuthTokenProvider
    {
        public readonly IOptions<D365CallbackConfiguration> _D365CallbackConfiguration;
        private readonly ILogger<AuthTokenProvider> _logger;

        public AuthTokenProvider(IOptions<D365CallbackConfiguration> d365CallbackConfiguration, ILogger<AuthTokenProvider> logger)
        {
            _D365CallbackConfiguration = d365CallbackConfiguration;
            _logger = logger;
        }

        public async Task<string> GetADAccessToken(string CorrelationId)
        {
            _logger.LogInformation(EventIds.CreateSubscriptionRequestCompleted.ToEventId(),
               "Before ad authentication token generation and ClientId:{ClientId} and ClientSecret:{ClientSecret} and D365Uri:{D365Uri} and D365ApiUri:{D365ApiUri} and with _X-Correlation-ID:{CorrelationId}", _D365CallbackConfiguration.Value.ClientId, _D365CallbackConfiguration.Value.ClientSecret, _D365CallbackConfiguration.Value.D365Uri, _D365CallbackConfiguration.Value.D365ApiUri, CorrelationId);
            AuthenticationResult Token = await GenerateADAccessToken(_D365CallbackConfiguration.Value);
            _logger.LogInformation(EventIds.CreateSubscriptionRequestCompleted.ToEventId(),
                "After ad authentication generated  with _X-Correlation-ID:{CorrelationId}", CorrelationId);
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
