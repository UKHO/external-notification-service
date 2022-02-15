using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Management.EventGrid;
using Microsoft.Rest;
using System.Threading;
using System.Threading.Tasks;


namespace UKHO.ExternalNotificationService.API.FunctionalTests.Helper
{
    public static class AzureEventGridDomainHelper
    {
        public static async Task<EventGridManagementClient> GetEventGridClient(string SubscriptionId, CancellationToken cancellationToken)
        {
            DefaultAzureCredential azureCredential = new();
            TokenRequestContext tokenRequestContext = new(new string[] { "https://management.azure.com/.default" });

            AccessToken tokenResult = await azureCredential.GetTokenAsync(tokenRequestContext, cancellationToken);
            TokenCredentials credential = new(tokenResult.Token);

            EventGridManagementClient _egClient = new(credential)
            {
                SubscriptionId = SubscriptionId
            };

            return _egClient;
        }
    }
}
