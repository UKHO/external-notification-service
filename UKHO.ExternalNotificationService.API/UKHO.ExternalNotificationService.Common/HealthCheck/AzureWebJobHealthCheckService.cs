﻿
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            string userNameKey = $"ens-{_webHostEnvironment.EnvironmentName}-webapp-scm-username";
            string passwordKey = $"ens-{_webHostEnvironment.EnvironmentName}-webapp-scm-password";
            string webJobUri = $"https://ens-{_webHostEnvironment.EnvironmentName}-webapp.scm.azurewebsites.net/api/continuouswebjobs/SubscriptionServiceWebJob";
            string userPassword = _webJobAccessKeyProvider.GetWebJobsAccessKey(userNameKey) + ":" + _webJobAccessKeyProvider.GetWebJobsAccessKey(passwordKey);
            userPassword = Convert.ToBase64String(Encoding.Default.GetBytes(userPassword));

            WebJobDetails webJobDetails = new()
            {
                UserPassword = userPassword,
                WebJobUri = webJobUri
            };

            Task<HealthCheckResult> webJobsHealth = _azureWebJobHelper.CheckWebJobsHealth(webJobDetails);
            await webJobsHealth;

            return webJobsHealth.Result;
        }
    }
}
