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
            _logger.LogWarning(EventIds.ElasticsearchClientNotConfigured.ToEventId(), "ADDS Monitoring Process: ElasticsearchClient is not configured. _X-Correlation-ID:{correlationId}.", correlationId);
            return;
        }

        var document = new ElasticLogDocument
        {
            Timestamp = DateTime.UtcNow,
            ProductName = addsData.ProductName,
            EditionNumber = addsData.EditionNumber,
            UpdateNumber = addsData.UpdateNumber,
            StatusName = addsData.StatusName,
            StatusDate = null,
            EventType = addsData.Type,
            StartTimestamp = null,
            StopTimestamp = DateTime.UtcNow,
            Duration = null,
            IsComplete = false,
            Environment = _elasticApmConfiguration.Environment,
            IsAbnormal = true
        };

        _logger.LogInformation(EventIds.StopAddsElasticMonitoringProcessStart.ToEventId(), "ADDS Monitoring Process: Started. Document: {Document}. _X-Correlation-ID: {CorrelationId}.", JsonConvert.SerializeObject(document), correlationId);

        document.Id = GetHash(document);
        try
        {
            var startProcess = await _elasticSearchClient.GetAsync<ElasticLogDocument>
                (document.Id, idx => idx.Index(_elasticApmConfiguration.IndexName), cancellationToken);
            if (startProcess is { Source: not null })
            {
                var startEventDocument = startProcess.Source;
                _logger.LogInformation(EventIds.StopAddsElasticMonitoringProcessUpdatingStartDocument.ToEventId(), "ADDS Monitoring Process: Updating start document with stop metrics. Document: {Document}. _X-Correlation-ID: {CorrelationId}.", JsonConvert.SerializeObject(startEventDocument), correlationId);

                document.StartTimestamp = startEventDocument.StartTimestamp;
                document.StopTimestamp = DateTime.UtcNow;
                if (document.StartTimestamp != null)
                {
                    var duration = document.StopTimestamp.Value -
                                   document.StartTimestamp.Value;
                    document.Duration = (int?)duration.TotalSeconds;
                }
                document.Timestamp = startEventDocument.Timestamp;
                document.StatusDate = startEventDocument.StatusDate;
                document.IsComplete = true;
                document.IsAbnormal = false;
                document.ImmediateRelease = startEventDocument.ImmediateRelease;
            } else {
                _logger.LogWarning(EventIds.StopAddsElasticMonitoringProcessStartDocumentNotFound.ToEventId(), "ADDS Monitoring Process: Start document not found, logging abnormal stop document. _X-Correlation-ID: {CorrelationId}.", correlationId);
            }
            var response = await _elasticSearchClient.IndexAsync(document, idx =>
                idx.Index(_elasticApmConfiguration.IndexName), cancellationToken);

            if (!response.IsValidResponse)
            {
                _logger.LogWarning(EventIds.StopAddsElasticMonitoringProcessInvalidResponse.ToEventId(), "ADDS Monitoring Process: Document failed to index. Document: {Document}. Response Debug Information: {ResponseDebugInformation} _X-Correlation-ID: {CorrelationId}.", JsonConvert.SerializeObject(document), response.DebugInformation, correlationId);
            }

            _logger.LogInformation(EventIds.StopAddsElasticMonitoringProcessCompleted.ToEventId(), "ADDS Monitoring Process: Completed. Document: {Document}. _X-Correlation-ID: {CorrelationId}.", JsonConvert.SerializeObject(document), correlationId);
        }
        catch (Exception exception)
        {
            _logger.LogError(EventIds.StopAddsElasticMonitoringProcessError.ToEventId(), exception, "ADDS Monitoring Process: Failed. Document: {Document}. _X-Correlation-ID: {CorrelationId}. Error Message:{Message}. Exception: {Exception}", JsonConvert.SerializeObject(document), correlationId, exception.Message, exception);
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
