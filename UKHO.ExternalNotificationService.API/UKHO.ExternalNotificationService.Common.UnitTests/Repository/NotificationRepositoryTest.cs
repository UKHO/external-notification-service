using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Repository;

namespace UKHO.ExternalNotificationService.Common.UnitTests.Repository
{
    [TestFixture]
    public class NotificationRepositoryTest
    {
        private ICollection<NotificationType> _fakeNotificationType;
        private IOptions<EnsConfiguration> _fakeEnsConfiguration;
        private NotificationRepository _notificationRepository;

        [SetUp]
        public void Setup()
        {
            _fakeNotificationType = [new() { Name = "Data test", TopicName = "test" }];

            _fakeEnsConfiguration = A.Fake<IOptions<EnsConfiguration>>();
            _fakeEnsConfiguration.Value.NotificationTypes = _fakeNotificationType;

            _notificationRepository = new NotificationRepository(_fakeEnsConfiguration);
        }

        [Test]
        public void WhenPostValidNotificationTypes_ThenReturnsNotificationTypeInResponse()
        {
            NotificationType notificationType = _notificationRepository.GetAllNotificationTypes().FirstOrDefault(x => x.Name == "Data test");

            Assert.That(notificationType.Name, Is.EqualTo(_fakeNotificationType.FirstOrDefault(x => x.Name == "Data test").Name));
        }
    }
}
