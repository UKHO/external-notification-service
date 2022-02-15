using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Management.EventGrid;
using Microsoft.Azure.Management.EventGrid.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using UKHO.ExternalNotificationService.API.FunctionalTests.Helper;
using UKHO.ExternalNotificationService.API.FunctionalTests.Model;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.FunctionalTests
{
    class EnsSubscriptionRequestInQueueTest
    {
        private EnsApiClient _ensApiClient { get; set; }
        private TestConfiguration _testConfig { get; set; }
        private D365Payload _d365Payload { get; set; }
        private QueueClient _queue { get; set; }
        private EventGridManagementClient _eventGridMgmtClient { get; set; }

        [SetUp]
        public async Task SetupAsync()
        {
            _testConfig = new TestConfiguration();
            _ensApiClient = new EnsApiClient(_testConfig.EnsApiBaseUrl);

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), _testConfig.PayloadFolder, _testConfig.PayloadFileName);

            _d365Payload = JsonConvert.DeserializeObject<D365Payload>(File.ReadAllText(filePath));

            _queue = new QueueClient(_testConfig.EnsStorageConnectionString, _testConfig.EnsStorageQueueName);
            _eventGridMgmtClient = await AzureEventGridDomainHelper.GetEventGridClient(_testConfig.EventGridDomainConfig.SubscriptionId, CancellationToken.None);
        }
        
        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidD365Payload_ThenMessageAddedInQueueAndDomainTopicCreated()
        {
            HttpResponseMessage apiResponse = await _ensApiClient.PostEnsApiSubscriptionAsync(_d365Payload);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            QueueMessage[] messageQueue = _queue.ReceiveMessages();

            DateTime startTime = DateTime.UtcNow;

            while (messageQueue.Length < 1 && DateTime.UtcNow - startTime < TimeSpan.FromSeconds(_testConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(1000);
                messageQueue = _queue.ReceiveMessages();
            }

            byte[] data = Convert.FromBase64String(messageQueue[0].Body.ToString());
            string messageBody = Encoding.UTF8.GetString(data);

            QueueMessageModel queueMessageData = JsonConvert.DeserializeObject<QueueMessageModel>(messageBody);

            string notificationType= _d365Payload.InputParameters[0].Value.FormattedValues[4].Value.ToString();

            //verify notification type in message body
            Assert.AreEqual(notificationType, queueMessageData.NotificationType);
            
            Assert.IsNotNull(messageQueue[0].MessageId);

            //Wait for Topic creation
            await Task.Delay(TimeSpan.FromSeconds(_testConfig.WaitingTimeForTopicCreationInSeconds));

            //Get the Topic details
            DomainTopic topic = await _eventGridMgmtClient.DomainTopics.GetAsync(_testConfig.EventGridDomainConfig.ResourceGroup, _testConfig.EventGridDomainConfig.EventGridDomainName, _testConfig.EventGridDomainConfig.NotificationTypeTopicName,CancellationToken.None);

            Assert.AreEqual(_testConfig.EventGridDomainConfig.NotificationTypeTopicName, topic.Name);
        }
    }
}
