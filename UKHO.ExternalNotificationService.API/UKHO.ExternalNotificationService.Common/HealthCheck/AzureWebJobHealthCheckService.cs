
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using UKHO.ExternalNotificationService.Common.Helpers;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public class AzureWebJobHealthCheckService : IAzureWebJobHealthCheckService
    {
        private readonly IWebJobAccessKeyProvider _webJobAccessKeyProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAzureWebJobHelper _azureWebJobHelper;

        public AzureWebJobHealthCheckService(IWebJobAccessKeyProvider webJobAccessKeyProvider,
                                       IWebHostEnvironment webHostEnvironment,
                                       IAzureWebJobHelper azureWebJobsHelper)
        {
            _webJobAccessKeyProvider = webJobAccessKeyProvider;
            _webHostEnvironment = webHostEnvironment;
            _azureWebJobHelper = azureWebJobsHelper;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                string webJobUri, userNameKey, passwordKey = string.Empty;

                userNameKey = $"ess-{_webHostEnvironment.EnvironmentName}-webapp-scm-username";
                passwordKey = $"ess-{_webHostEnvironment.EnvironmentName}-webapp-scm-password";
                webJobUri = $"https://ess-{_webHostEnvironment.EnvironmentName}-webapp.scm.azurewebsites.net/api/continuouswebjobs/SubscriptionServiceWebJob";
                string userPassword = _webJobAccessKeyProvider.GetWebJobsAccessKey(userNameKey) + ":" + _webJobAccessKeyProvider.GetWebJobsAccessKey(passwordKey);
                userPassword = Convert.ToBase64String(Encoding.Default.GetBytes(userPassword));

                WebJobDetails webJobDetails = new WebJobDetails
                {
                    UserPassword = userPassword,
                    WebJobUri = webJobUri
                };

                var webJobsHealth = _azureWebJobHelper.CheckWebJobsHealth(webJobDetails);
                await Task.WhenAll(webJobsHealth);

                return webJobsHealth.Result;
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Azure webjob is unhealthy", new Exception(ex.Message));
            }
        }
    }
}
