using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using UKHO.ExternalNotificationService.API.Filters;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.API.Validation;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.HealthCheck;
using UKHO.ExternalNotificationService.Common.Helpers;
using UKHO.ExternalNotificationService.Common.Repository;
using UKHO.ExternalNotificationService.Common.Storage;
using UKHO.Logging.EventHubLogProvider;

namespace UKHO.ExternalNotificationService.API
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment env)
        {
            _configuration = BuildConfiguration(env);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            var ensAuthConfiguration = new AzureADConfiguration();
            _configuration.Bind("EnsAuthConfiguration", ensAuthConfiguration);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer("AzureAD", options =>
                    {
                        options.Audience = ensAuthConfiguration.ClientId;
                        options.Authority = $"{ensAuthConfiguration.MicrosoftOnlineLoginUrl}{ensAuthConfiguration.TenantId}";
                    });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("AzureAD")
                .Build();
            });

            services.Configure<EventHubLoggingConfiguration>(_configuration.GetSection("EventHubLoggingConfiguration"));
            services.Configure<EnsConfiguration>(_configuration.GetSection("EnsConfiguration"));           
            services.Configure<SubscriptionStorageConfiguration>(_configuration.GetSection("SubscriptionStorageConfiguration"));
            services.Configure<EventGridDomainConfiguration>(_configuration.GetSection("EventGridDomainConfiguration"));
            services.Configure<FssDataMappingConfiguration>(_configuration.GetSection("FssDataMappingConfiguration"));
            services.Configure<ScsDataMappingConfiguration>(_configuration.GetSection("ScsDataMappingConfiguration"));

            services.AddApplicationInsightsTelemetry();
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(_configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
                loggingBuilder.AddAzureWebAppDiagnostics();
            });
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddHeaderPropagation(options =>
            {
                options.Headers.Add(CorrelationIdMiddleware.XCorrelationIdHeaderKey);
            });
            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IEventHubLoggingHealthClient, EventHubLoggingHealthClient>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IAzureMessageQueueHelper, AzureMessageQueueHelper>();           
            services.AddScoped<ID365PayloadValidator, D365PayloadValidator>();
            services.AddSingleton<INotificationRepository, NotificationRepository>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IAzureBlobStorageHelper, AzureBlobStorageHelper>();
            services.AddScoped<IAzureWebJobHealthCheckService, AzureWebJobHealthCheckService>();
            services.AddScoped<IAzureWebJobHelper, AzureWebJobHelper>();
            services.AddSingleton<IWebJobAccessKeyProvider>(_ => new WebJobAccessKeyProvider(_configuration));
            services.AddSingleton<INotificationRepository, NotificationRepository>();
            services.AddScoped<IEventProcessorFactory, EventProcessorFactory>();
            services.AddScoped<IFssEventValidationAndMappingService, FssEventValidationAndMappingService>();
            services.AddScoped<IFssEventDataValidator, FssEventDataValidator>();
            services.AddScoped<IEventProcessor, FssEventProcessor>();
            services.AddScoped<IAzureEventGridDomainService, AzureEventGridDomainService>();
            services.AddScoped<IEventSubscriptionConfiguration, EventSubscriptionConfiguration>();
            services.AddScoped<IScsEventValidationAndMappingService, ScsEventValidationAndMappingService>();
            services.AddScoped<IScsEventDataValidator, ScsEventDataValidator>();
            services.AddScoped<IEventProcessor, ScsEventProcessor>();

            services.AddHealthChecks()
                .AddCheck<EventHubLoggingHealthCheck>("EventHubLoggingHealthCheck")
                .AddCheck<AzureBlobStorageHealthCheck>("AzureBlobStorageHealthCheck")
                .AddCheck<AzureMessageQueueHealthCheck>("AzureMessageQueueHealthCheck")
                .AddCheck<AzureWebJobHealthCheck>("AzureWebJobsHealthCheck");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
                            IHttpContextAccessor httpContextAccessor, IOptions<EventHubLoggingConfiguration> eventHubLoggingConfiguration)
        {
            ConfigureLogging(app, loggerFactory, httpContextAccessor, eventHubLoggingConfiguration);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }

        protected IConfigurationRoot BuildConfiguration(IWebHostEnvironment hostingEnvironment)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                 .SetBasePath(hostingEnvironment.ContentRootPath)
                 .AddJsonFile("appsettings.json", false, true);

            builder.AddEnvironmentVariables();
            IConfigurationRoot tempConfig = builder.Build();
            string kvServiceUri = tempConfig["KeyVaultSettings:ServiceUri"];

            if (!string.IsNullOrWhiteSpace(kvServiceUri))
            {
                builder.AddAzureKeyVault(new Uri(kvServiceUri), new DefaultAzureCredential());
            }

            builder.AddJsonFile("ConfigurationFiles/NotificationTypes.json", false, true);

#if DEBUG
            builder.AddJsonFile("appsettings.local.overrides.json", true, true);
#endif
            return builder.Build();
        }

        private void ConfigureLogging(IApplicationBuilder app, ILoggerFactory loggerFactory,
                                    IHttpContextAccessor httpContextAccessor, IOptions<EventHubLoggingConfiguration> eventHubLoggingConfiguration)
        {
            if (!string.IsNullOrWhiteSpace(eventHubLoggingConfiguration?.Value.ConnectionString))
            {
                void ConfigAdditionalValuesProvider(IDictionary<string, object> additionalValues)
                {
                    if (httpContextAccessor.HttpContext != null)
                    {
                        additionalValues["_Environment"] = eventHubLoggingConfiguration.Value.Environment;
                        additionalValues["_System"] = eventHubLoggingConfiguration.Value.System;
                        additionalValues["_Service"] = eventHubLoggingConfiguration.Value.Service;
                        additionalValues["_NodeName"] = eventHubLoggingConfiguration.Value.NodeName;
                        additionalValues["_RemoteIPAddress"] = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                        additionalValues["_User-Agent"] = httpContextAccessor.HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? string.Empty;
                        additionalValues["_AssemblyVersion"] = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyFileVersionAttribute>().Single().Version;
                        additionalValues["_X-Correlation-ID"] =
                            httpContextAccessor.HttpContext.Request.Headers?[CorrelationIdMiddleware.XCorrelationIdHeaderKey].FirstOrDefault() ?? string.Empty;

                        if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                        {
                            additionalValues["_UserId"] = httpContextAccessor.HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
                        }
                    }
                }

                loggerFactory.AddEventHub(
                                         config =>
                                         {
                                             config.Environment = eventHubLoggingConfiguration.Value.Environment;
                                             config.DefaultMinimumLogLevel =
                                                 (LogLevel)Enum.Parse(typeof(LogLevel), eventHubLoggingConfiguration.Value.MinimumLoggingLevel, true);
                                             config.MinimumLogLevels["UKHO"] =
                                                 (LogLevel)Enum.Parse(typeof(LogLevel), eventHubLoggingConfiguration.Value.UkhoMinimumLoggingLevel, true);
                                             config.EventHubConnectionString = eventHubLoggingConfiguration.Value.ConnectionString;
                                             config.EventHubEntityPath = eventHubLoggingConfiguration.Value.EntityPath;
                                             config.System = eventHubLoggingConfiguration.Value.System;
                                             config.Service = eventHubLoggingConfiguration.Value.Service;
                                             config.NodeName = eventHubLoggingConfiguration.Value.NodeName;
                                             config.AdditionalValuesProvider = ConfigAdditionalValuesProvider;
                                         });
            }
#if (DEBUG)
            //Add file based logger for development
            loggerFactory.AddFile(_configuration.GetSection("Logging"));
#endif
            app.UseLogAllRequestsAndResponses(loggerFactory);

            app.UseCorrelationIdMiddleware()
               .UseErrorLogging(loggerFactory);
        }
    }
}
