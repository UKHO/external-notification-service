using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using UKHO.ExternalNotificationService.Common.Configuration;

namespace UKHO.ExternalNotificationService.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElasticSearchClient(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("ADDSMonitoringEnabled"))
        {
            ElasticApmConfiguration elasticApmConfiguration = configuration.GetSection("ElasticApm").Get<ElasticApmConfiguration>
                () ?? new ElasticApmConfiguration();

            ElasticsearchClientSettings settings = new(elasticApmConfiguration.CloudId,
                new ApiKey(elasticApmConfiguration.AddsApiKey));

            services.AddSingleton(new ElasticsearchClient(settings));
        }

        return services;
    }
}
