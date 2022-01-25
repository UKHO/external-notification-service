using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using UKHO.ExternalNotificationService.API.Filter;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.HealthCheck;
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IEventHubLoggingHealthClient, EventHubLoggingHealthClient>();
            services.AddHealthChecks().AddCheck<EventHubLoggingHealthCheck>("EventHubLoggingHealthCheck");
            services.Configure<EventHubLoggingConfiguration>(_configuration.GetSection("EventHubLoggingConfiguration"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor, IOptions<EventHubLoggingConfiguration> eventHubLoggingConfiguration)
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

            var tempConfig = builder.Build();
            string kvServiceUri = tempConfig["KeyVaultSettings:ServiceUri"];

            if (!string.IsNullOrWhiteSpace(kvServiceUri))
            {
                builder.AddAzureKeyVault(new Uri(kvServiceUri), new DefaultAzureCredential());
            }
#if DEBUG
            builder.AddJsonFile("appsettings.local.overrides.json", true, true);
#endif
            return builder.Build();
        }

        [SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "httpContextAccessor is used in action delegate")]
        private void ConfigureLogging(IApplicationBuilder app, ILoggerFactory loggerFactory,
                                   IHttpContextAccessor httpContextAccessor, IOptions<EventHubLoggingConfiguration> eventHubLoggingConfiguration)
        {

            if (!string.IsNullOrWhiteSpace(eventHubLoggingConfiguration?.Value.ConnectionString))
            {
                void ConfigAdditionalValuesProvider(IDictionary<string, object> additionalValues)
                {
                    if (httpContextAccessor.HttpContext != null)
                    {
                        additionalValues["_RemoteIPAddress"] = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                        additionalValues["_User-Agent"] = httpContextAccessor.HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? string.Empty;
                        additionalValues["_AssemblyVersion"] = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyFileVersionAttribute>().Single().Version;
                        additionalValues["_X-Correlation-ID"] =
                            httpContextAccessor.HttpContext.Request.Headers?[CorrelationIdMiddleware.XCorrelationIdHeaderKey].FirstOrDefault() ?? string.Empty;

                        if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                            additionalValues["_UserId"] = httpContextAccessor.HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
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
        }
    }
}
