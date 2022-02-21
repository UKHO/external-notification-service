
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    [ExcludeFromCodeCoverage]
    public class AzureWebJobHelper : IAzureWebJobHelper
    {
        static HttpClient httpClient = new();
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AzureWebJobHelper(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<HealthCheckResult> CheckWebJobsHealth(WebJobDetails webJob)
        {
            try
            {
                string webJobDetail, webJobStatus = string.Empty;

                using HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, webJob.WebJobUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", webJob.UserPassword);

                HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    dynamic webJobDetails = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                    webJobStatus = webJobDetails["status"];
                    if (webJobStatus != "Running")
                    {
                        webJobDetail = $"Webjob ens-{_webHostEnvironment.EnvironmentName} status is {webJobStatus}";
                        return HealthCheckResult.Unhealthy("Azure webjob is unhealthy", new Exception(webJobDetail));
                    }
                }
                else
                {
                    return HealthCheckResult.Unhealthy("Azure webjob is unhealthy", new Exception($"Webjob ens-{_webHostEnvironment.EnvironmentName} status code is {response.StatusCode}"));
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
