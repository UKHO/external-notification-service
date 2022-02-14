﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Azure.Identity;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.SubscriptionService.Services;
using UKHO.Logging.EventHubLogProvider;

namespace UKHO.ExternalNotificationService.SubscriptionService
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        private static IConfiguration ConfigurationBuilder;
        private static string AssemblyVersion = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyFileVersionAttribute>().Single().Version;
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
            hostBuilder.ConfigureAppConfiguration((hostContext, builder) =>
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

                Program.ConfigurationBuilder = builder.Build();
            })
             .ConfigureLogging((hostContext, builder) =>
             {
                 builder.AddConfiguration(ConfigurationBuilder.GetSection("Logging"));

#if DEBUG
                 builder.AddSerilog(new LoggerConfiguration()
                                 .WriteTo.File("Logs/UKHO.ExternalNotificationService.SubscriptionService-.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}")
                                 .MinimumLevel.Information()
                                 .MinimumLevel.Override("UKHO", LogEventLevel.Debug)
                                 .CreateLogger(), dispose: true);
#endif

                 builder.AddConsole();

                 //Add Application Insights if needed (if key exists in settings)
                 string instrumentationKey = ConfigurationBuilder["APPINSIGHTS_INSTRUMENTATIONKEY"];
                 if (!string.IsNullOrEmpty(instrumentationKey))
                 {
                     builder.AddApplicationInsightsWebJobs(o => o.InstrumentationKey = instrumentationKey);
                 }
                 EventHubLoggingConfiguration eventhubConfig = ConfigurationBuilder.GetSection("EventHubLoggingConfiguration").Get<EventHubLoggingConfiguration>();

                 if (!string.IsNullOrWhiteSpace(eventhubConfig.ConnectionString))
                 {
                     builder.AddEventHub(config =>
                     {
                         config.Environment = eventhubConfig.Environment;
                         config.DefaultMinimumLogLevel =
                             (LogLevel)Enum.Parse(typeof(LogLevel), eventhubConfig.MinimumLoggingLevel, true);
                         config.MinimumLogLevels["UKHO"] =
                             (LogLevel)Enum.Parse(typeof(LogLevel), eventhubConfig.UkhoMinimumLoggingLevel, true);
                         config.EventHubConnectionString = eventhubConfig.ConnectionString;
                         config.EventHubEntityPath = eventhubConfig.EntityPath;
                         config.System = eventhubConfig.System;
                         config.Service = eventhubConfig.Service;
                         config.NodeName = eventhubConfig.NodeName;
                         config.AdditionalValuesProvider = additionalValues =>
                         {
                             additionalValues["_AssemblyVersion"] = AssemblyVersion;
                         };
                     });
                 }

             })
              .ConfigureServices((hostContext, services) =>
              {
                  services.Configure<SubscriptionStorageConfiguration>(ConfigurationBuilder.GetSection("SubscriptionStorageConfiguration"));
                  services.Configure<EventGridDomainConfiguration>(ConfigurationBuilder.GetSection("EventGridDomainConfiguration"));
                  services.Configure<QueuesOptions>(ConfigurationBuilder.GetSection("QueuesOptions"));
                  services.AddScoped<ISubscriptionServiceData, SubscriptionServiceData>();
                  services.AddScoped<IAzureEventGridDomainService, AzureEventGridDomainService>();                  
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