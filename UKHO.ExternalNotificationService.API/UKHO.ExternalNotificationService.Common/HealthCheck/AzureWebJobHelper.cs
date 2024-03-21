
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    [ExcludeFromCodeCoverage]
    public class AzureWebJobHelper : IAzureWebJobHelper
    {
        static readonly HttpClient s_httpClient = new();
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AzureWebJobHelper(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<HealthCheckResult> CheckWebJobsHealth(WebJobDetails webJob)
        {
            try
            {

                using HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, webJob.WebJobUri);
                s_httpClient.DefaultRequestHeaders.Accept.Clear();
                s_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                s_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", webJob.UserPassword);

                HttpResponseMessage response = await s_httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //rhz
                    string data = await response.Content.ReadAsStringAsync();
                    data = data.ToLower();
                    //dynamic webJobDetails = JsonSerializer.Deserialize<dynamic>(await response.Content.ReadAsStringAsync());
                    JsonNode webJobDetails = JsonSerializer.Deserialize<JsonNode>(data);
                    string webJobStatus = webJobDetails["status"].GetValue<string>();
                    if (webJobStatus != "Running")
                    {
                        string webJobDetail = $"Webjob ens-{_webHostEnvironment.EnvironmentName} status is {webJobStatus}";
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
