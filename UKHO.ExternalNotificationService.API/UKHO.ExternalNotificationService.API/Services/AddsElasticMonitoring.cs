using System.Security.Cryptography;
using System.Text;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Monitoring;

namespace UKHO.ExternalNotificationService.API.Services;

public class AddsElasticMonitoringService : IAddsMonitoringService
{
    private readonly ILogger<AddsElasticMonitoringService> _logger;
    private readonly ElasticApmConfiguration _elasticApmConfiguration;
    private readonly ElasticsearchClient? _elasticSearchClient;

    public AddsElasticMonitoringService(ILogger<AddsElasticMonitoringService> logger, IOptions<ElasticApmConfiguration> elasticApmConfiguration, ElasticsearchClient? elasticSearchClient = null)
    {
        _logger = logger;
        _elasticApmConfiguration = elasticApmConfiguration.Value;
        _elasticSearchClient = elasticSearchClient;
    }
    public async Task StopProcessAsync(AddsData addsData, string correlationId, CancellationToken cancellationToken = default)
    {
        if (_elasticSearchClient is null)
        {
            _logger.LogWarning(EventIds.ElasticsearchClientNotConfigured.ToEventId(), "ElasticsearchClient is not configured, _X-Correlation-ID:{correlationId}.",
                 correlationId);
            return;
        }

        var document = new ElasticLogDocument
        {
            Timestamp = DateTime.UtcNow,
            ProductName = addsData.ProductName,
            EditionNumber = addsData.EditionNumber,
            UpdateNumber = addsData.UpdateNumber,
            StatusName = addsData.StatusName,
            StatusDate = string.Empty,
            EventType = addsData.Type,
            StartTimestamp = null,
            StopTimestamp = DateTime.UtcNow,
            Duration = null,
            IsComplete = false,
            Environment = _elasticApmConfiguration.Environment,
            IsAbnormal = true
        };

        _logger.LogInformation(EventIds.StopAddsElasticMonitoringProcessStart.ToEventId(), "Process to stop ADDS monitoring started for document {document} and _X-Correlation-ID:{correlationId}.",
            JsonConvert.SerializeObject(document), correlationId);

        document.Id = GetHash(document);
        try
        {
            var startProcess = await _elasticSearchClient.GetAsync<ElasticLogDocument>
                (document.Id, idx => idx.Index(_elasticApmConfiguration.IndexName), cancellationToken);
            if (startProcess is { Source: not null })
            {
                document.StartTimestamp = startProcess.Source.StartTimestamp;
                document.StopTimestamp = DateTime.UtcNow;
                if (document.StartTimestamp != null)
                {
                    var duration = document.StopTimestamp.Value -
                                   document.StartTimestamp.Value;
                    document.Duration = (int?)duration.TotalSeconds;
                }
                document.Timestamp = startProcess.Source.Timestamp;
                document.StatusDate = startProcess.Source.StatusDate;
                document.IsComplete = true;
                document.IsAbnormal = false;
                document.ImmediateRelease = startProcess.Source.ImmediateRelease;
            }
            await _elasticSearchClient.IndexAsync(document, idx =>
                idx.Index(_elasticApmConfiguration.IndexName), cancellationToken);

            _logger.LogInformation(EventIds.StopAddsElasticMonitoringProcessCompleted.ToEventId(), "Process to stop ADDS monitoring completed for document {document} and _X-Correlation-ID:{correlationId}.",
                JsonConvert.SerializeObject(document), correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(EventIds.StopAddsElasticMonitoringProcessError.ToEventId(), "Process to stop ADDS monitoring failed for document {document} and _X-Correlation-ID:{correlationId} with error:{Message}",
                JsonConvert.SerializeObject(document), correlationId, ex.Message);
        }
    }

    private static string GetHash(ElasticLogDocument request)
    {
        byte[] data = Encoding.UTF8.GetBytes(
            $"{request.ProductName}{request.EditionNumber}{request.UpdateNumber}{ request.StatusName}");
        byte[] hashBytes = SHA256.HashData(data);

        return BitConverter.ToString(hashBytes).Replace("-", "");
    }
}
