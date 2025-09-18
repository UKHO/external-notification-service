using System;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Models.Monitoring;

namespace UKHO.ExternalNotificationService.API.UnitTests.Services;

[TestFixture]
public class AddsElasticMonitoringTest
{
    private ILogger<AddsElasticMonitoringService> _fakeLogger;
    private IOptions<ElasticApmConfiguration> _elasticApmConfiguration;
    private ElasticsearchClient _fakeElasticSearchClient;
    private AddsElasticMonitoringService _addsElasticMonitoringService;
    private AddsData _addsData;
    private string _correlationId;

    [SetUp]
    public void SetUp()
    {
        _fakeLogger = A.Fake<ILogger<AddsElasticMonitoringService>>();
        _elasticApmConfiguration = Options.Create(new ElasticApmConfiguration
        {
            Environment = "TestEnvironment", IndexName = "TestIndex"
        });
        _fakeElasticSearchClient = A.Fake<ElasticsearchClient>();
        _addsElasticMonitoringService =
            new AddsElasticMonitoringService(_fakeLogger, _elasticApmConfiguration, _fakeElasticSearchClient);
        _addsData = new AddsData
        {
            ProductName = "TestProduct",
            EditionNumber = 1,
            UpdateNumber = 1,
            StatusName = "TestStatus",
            Type = "TestType"
        };
        _correlationId = "TestCorrelationId";
    }

    [Test]
    public async Task StopProcessAsync_ShouldSetElasticDocumentDurationAndIsCompleteTrue()
    {
        // Arrange
        ConfigureElasticClientGetAsyncWithValidResponse();

        // Act
        await _addsElasticMonitoringService.StopProcessAsync(_addsData, _correlationId);

        // Assert
        Func<ElasticLogDocument, bool> documentMatcher = d =>
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(d.IsComplete, Is.True, "Is Complete should be true");
                Assert.That(d.Duration.HasValue, Is.True, "Duration should not be null");
                Assert.That(d.Duration ?? 0, Is.GreaterThan(TimeSpan.Zero.Seconds), "Duration should be greater than zero");
            }
            return true;
        };

        A.CallTo(() => _fakeElasticSearchClient.IndexAsync(A<ElasticLogDocument>.That.Matches(d => documentMatcher(d)),
                A<Action<IndexRequestDescriptor<ElasticLogDocument>>>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(_fakeLogger).Where(call => call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened(3, Times.Exactly);

        return;

        void ConfigureElasticClientGetAsyncWithValidResponse()
        {
            ElasticLogDocument document = new()
            {
                StartTimestamp = DateTime.UtcNow.AddMinutes(-5),
                ImmediateRelease = true,
                Id = "TestDocumentId",
                IsComplete = false,
                Duration = null
            };

            GetResponse<ElasticLogDocument> startProcessResponse = new() { Source = document };

            A.CallTo(() => _fakeElasticSearchClient.GetAsync(A<Id>.Ignored,
                    A<Action<GetRequestDescriptor<ElasticLogDocument>>>.Ignored, A<CancellationToken>.Ignored))
                .Returns(startProcessResponse);
        }
    }

    [Test]
    public async Task StopProcessAsync_ShouldLogErrorWhenProcessIsFailed()
    {
        // Arrange
        A.CallTo(() => _fakeElasticSearchClient.GetAsync(A<Id>.Ignored,
                A<Action<GetRequestDescriptor<ElasticLogDocument>>>.Ignored, A<CancellationToken>.Ignored))
            .ThrowsAsync(new Exception("failed"));

        // Act
        await _addsElasticMonitoringService.StopProcessAsync(_addsData, _correlationId);

        // Assert
        A.CallTo(() => _fakeElasticSearchClient.IndexAsync(A<ElasticLogDocument>.Ignored,
                A<Action<IndexRequestDescriptor<ElasticLogDocument>>>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(_fakeLogger).Where(call => call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened(1, Times.Exactly);

        A.CallTo(_fakeLogger).Where(call => call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
    }

    [Test]
    public async Task StopProcessAsync_ShouldLogWarningWhenElasticsearchClientIsNull()
    {
        // Arrange
        _addsElasticMonitoringService = new AddsElasticMonitoringService(_fakeLogger, _elasticApmConfiguration);

        // Act
        await _addsElasticMonitoringService.StopProcessAsync(_addsData, _correlationId);

        // Assert
        A.CallTo(() => _fakeElasticSearchClient.IndexAsync(A<ElasticLogDocument>.Ignored,
                A<Action<IndexRequestDescriptor<ElasticLogDocument>>>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _fakeElasticSearchClient.GetAsync(A<Id>.Ignored,
                A<Action<GetRequestDescriptor<ElasticLogDocument>>>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(_fakeLogger).Where(call => call.GetArgument<LogLevel>(0) == LogLevel.Warning)
            .MustHaveHappened(1, Times.Exactly);
    }
}
