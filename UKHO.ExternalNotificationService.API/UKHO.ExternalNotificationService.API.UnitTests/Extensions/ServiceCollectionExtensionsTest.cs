using System.Collections.Generic;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Extensions;
namespace UKHO.ExternalNotificationService.API.UnitTests.Extensions;

[TestFixture]
public class ServiceCollectionExtensionsTest
{
    private IConfiguration _configuration;
    private string cloudId = "name:bG9jYWxob3N0JGFiY2QkZWZnaA==";
    private Dictionary<string, string> _inMemorySettings; 

    [SetUp]
    public void Setup()
    {
        _inMemorySettings = new()
        {
            {"ElasticApm:CloudId", cloudId},
            {"ElasticApm:ADDSApiKey", "test-api-key"},
            {"ADDSMonitoringEnabled", "true"}
        };
    }

    private IConfiguration CreateConfiguration(Dictionary<string, string> inMemorySettings)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Test]
    public void AddElasticSearchClient_ShouldRegisterElasticsearchClient()
    {
        ServiceCollection services = new();
        
        _configuration = CreateConfiguration(_inMemorySettings);

        services.AddElasticSearchClient(_configuration);

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        ElasticsearchClient elasticsearchClient = serviceProvider.GetService<ElasticsearchClient>();
        
        Assert.That(elasticsearchClient != null, "elasticsearchClient should not be null");
    }

    [Test]
    public void AddElasticSearchClient_ShouldNotRegisterElasticsearchClient_WhenNotEnabled()
    {
        _inMemorySettings["ADDSMonitoringEnabled"] = "false";
        _configuration = CreateConfiguration(_inMemorySettings);

        ServiceCollection services = new();

        services.AddElasticSearchClient(_configuration);

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        ElasticsearchClient elasticsearchClient = serviceProvider.GetService<ElasticsearchClient>();

        Assert.That(elasticsearchClient == null, "elasticsearchClient should be null");
    }
}
