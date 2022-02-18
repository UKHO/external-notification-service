using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        [SetUp]
        public void Setup()
        {
            TestConfig = new TestConfiguration();
            EnsApiClient = new EnsApiClient(TestConfig.EnsApiBaseUrl);

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), TestConfig.PayloadFolder, TestConfig.PayloadFileName);

            D365Payload = JsonConvert.DeserializeObject<D365Payload>(File.ReadAllText(filePath));

            _queue = new QueueClient(_testConfig.EnsStorageConnectionString, _testConfig.EnsStorageQueueName);
            
        }
        
        [Test]
        public async Task WhenICallTheEnsSubscriptionApiWithAValidD365Payload_ThenMessageAddedInQueue()
        {
            HttpResponseMessage apiResponse = await EnsApiClient.PostEnsApiSubscriptionAsync(D365Payload);
            Assert.AreEqual(202, (int)apiResponse.StatusCode, $"Incorrect status code {apiResponse.StatusCode}  is  returned, instead of the expected 202.");

            QueueMessage[] messageQueue = Queue.ReceiveMessages();

            DateTime startTime = DateTime.UtcNow;

            while (messageQueue.Length < 1 && DateTime.UtcNow - startTime < TimeSpan.FromSeconds(TestConfig.WaitingTimeForQueueInSeconds))
            {
                await Task.Delay(1000);
                messageQueue = Queue.ReceiveMessages();
            }

            byte[] data = Convert.FromBase64String(messageQueue[0].Body.ToString());
            string messageBody = Encoding.UTF8.GetString(data);

            QueueMessageModel queueMessageData = JsonConvert.DeserializeObject<QueueMessageModel>(messageBody);

            string notificationType= D365Payload.InputParameters[0].Value.FormattedValues[4].Value.ToString();

            //verify notification type in message body
            Assert.AreEqual(notificationType, queueMessageData.NotificationType);
            
            Assert.IsNotNull(messageQueue[0].MessageId);           
        }
    }
}
