﻿
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using UKHO.ExternalNotificationService.Common.HealthCheck;

namespace UKHO.ExternalNotificationService.Common.Helpers
{
    [ExcludeFromCodeCoverage]
    public class AzureWebJobsHelper : IAzureWebJobsHelper
    {
        static HttpClient httpClient = new HttpClient();
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AzureWebJobsHelper(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<HealthCheckResult> CheckAllWebJobsHealth(WebJobDetails webJob)
        {
            try
            {
                string webJobDetail, webJobStatus = string.Empty;

                using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, webJob.WebJobUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var webJobDetails = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                    webJobStatus = webJobDetails["status"];
                    if (webJobStatus != "Running")
                    {
                        webJobDetail = $"Webjob ess-{_webHostEnvironment.EnvironmentName} status is {webJobStatus}";
                        return HealthCheckResult.Unhealthy("Azure webjob is unhealthy", new Exception(webJobDetail));
                    }
                }
                else
                {
                    return HealthCheckResult.Unhealthy("Azure webjob is unhealthy", new Exception($"Webjob ess-{_webHostEnvironment.EnvironmentName} status code is {response.StatusCode}"));
                }
                return HealthCheckResult.Healthy("Azure webjob is healthy");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Azure webjob is unhealthy", new Exception(ex.Message));
            }
        }
    }
}
