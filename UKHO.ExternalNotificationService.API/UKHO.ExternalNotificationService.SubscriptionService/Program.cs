using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Azure.Identity;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Serilog;
using Serilog.Events;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Models.Response;
using UKHO.ExternalNotificationService.SubscriptionService.Configuration;
using UKHO.ExternalNotificationService.SubscriptionService.Helpers;
using UKHO.ExternalNotificationService.SubscriptionService.Services;
using UKHO.Logging.EventHubLogProvider;

namespace UKHO.ExternalNotificationService.SubscriptionService
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        private static IConfiguration s_configurationBuilder;
        private static readonly string s_assemblyVersion = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyFileVersionAttribute>().Single().Version;
        public static void Main(string[] args)
        {
            HostBuilder hostBuilder = BuildHostConfiguration();

            IHost host = hostBuilder.Build();

            using (host)
            {
                host.Run();
            }
        }

        private static HostBuilder BuildHostConfiguration()
        {
             

            HostBuilder hostBuilder = new();
            hostBuilder.ConfigureAppConfiguration((_, builder) =>
            {
                builder.AddJsonFile("appsettings.json");
                //Add environment specific configuration files.
                string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (!string.IsNullOrWhiteSpace(environmentName))
                {
                    builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
                }
#if DEBUG
                //Add development overrides configuration
                builder.AddJsonFile("appsettings.local.overrides.json", true, true);
#endif

                //Add environment variables
                builder.AddEnvironmentVariables();

                IConfigurationRoot tempConfig = builder.Build();
                string kvServiceUri = tempConfig["KeyVaultSettings:ServiceUri"];
                if (!string.IsNullOrWhiteSpace(kvServiceUri))
                {
                    builder.AddAzureKeyVault(new Uri(kvServiceUri), new DefaultAzureCredential());
                }

                Program.s_configurationBuilder = builder.Build();
            })
             .ConfigureLogging((_, builder) =>
             {
                 builder.AddConfiguration(s_configurationBuilder.GetSection("Logging"));

#if DEBUG
                 builder.AddSerilog(new LoggerConfiguration()
                                 .WriteTo.File("Logs/UKHO.ExternalNotificationService.SubscriptionService-.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}")
                                 .MinimumLevel.Information()
                                 .MinimumLevel.Override("UKHO", LogEventLevel.Debug)
                                 .CreateLogger(), dispose: true);
#endif

                 builder.AddConsole();

                 //Add Application Insights if needed (if key exists in settings)
                 string instrumentationKey = s_configurationBuilder["APPINSIGHTS_INSTRUMENTATIONKEY"];
                 if (!string.IsNullOrEmpty(instrumentationKey))
                 {
                     builder.AddApplicationInsightsWebJobs(o => o.InstrumentationKey = instrumentationKey);
                 }
                 EventHubLoggingConfiguration eventHubConfig = s_configurationBuilder.GetSection("EventHubLoggingConfiguration").Get<EventHubLoggingConfiguration>();

                 if (!string.IsNullOrWhiteSpace(eventHubConfig.ConnectionString))
                 {
                     builder.AddEventHub(config =>
                     {
                         config.Environment = eventHubConfig.Environment;
                         config.DefaultMinimumLogLevel =
                             (LogLevel)Enum.Parse(typeof(LogLevel), eventHubConfig.MinimumLoggingLevel, true);
                         config.MinimumLogLevels["UKHO"] =
                             (LogLevel)Enum.Parse(typeof(LogLevel), eventHubConfig.UkhoMinimumLoggingLevel, true);
                         config.EventHubConnectionString = eventHubConfig.ConnectionString;
                         config.EventHubEntityPath = eventHubConfig.EntityPath;
                         config.System = eventHubConfig.System;
                         config.Service = eventHubConfig.Service;
                         config.NodeName = eventHubConfig.NodeName;
                         config.AdditionalValuesProvider = additionalValues =>
                         {
                             additionalValues["_AssemblyVersion"] = s_assemblyVersion;
                         };
                     });
                 }

             })

              .ConfigureServices((_, services) =>
              {
                  int retryCount = Convert.ToInt32(s_configurationBuilder["D365CallbackConfiguration:RetryCount"]);
                  double sleepDuration = Convert.ToDouble(s_configurationBuilder["D365CallbackConfiguration:SleepDuration"]);

                  Polly.Retry.AsyncRetryPolicy<HttpResponseMessage> retryPolicy = Policy.Handle<HttpRequestException>
                      (response => response.StatusCode != HttpStatusCode.NoContent)
                  .OrResult<HttpResponseMessage>(response => response.StatusCode != (HttpStatusCode.InternalServerError))
                  ///// .OrResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.BadRequest)           
                  .WaitAndRetryAsync(retryCount, (retryAttempt) =>
                  {
                      return TimeSpan.FromSeconds(Math.Pow(sleepDuration, (retryAttempt - 1)));
                  });

                  services.Configure<SubscriptionStorageConfiguration>(s_configurationBuilder.GetSection("SubscriptionStorageConfiguration"));
                  services.Configure<EventGridDomainConfiguration>(s_configurationBuilder.GetSection("EventGridDomainConfiguration"));
                  services.Configure<QueuesOptions>(s_configurationBuilder.GetSection("QueuesOptions"));
                  services.Configure<D365CallbackConfiguration>(s_configurationBuilder.GetSection("D365CallbackConfiguration"));
                  services.Configure<AzureADConfiguration>(s_configurationBuilder.GetSection("EnsAuthConfiguration"));
                  services.AddScoped<ISubscriptionServiceData, SubscriptionServiceData>();
                  services.AddScoped<IAzureEventGridDomainService, AzureEventGridDomainService>();
                  services.AddScoped<IAuthTokenProvider, AuthTokenProvider>();
                  services.AddScoped<ICallbackService, CallbackService>();           
                 
                  services.AddHttpClient("D365DataverseApi").AddPolicyHandler(retryPolicy);
                  services.AddScoped<ICallbackClient, CallbackClient>();
              })
              .ConfigureWebJobs(b =>
              {
                  b.AddAzureStorageCoreServices();
                  b.AddAzureStorageQueues();
              });

            return hostBuilder;
        }
    }
}
