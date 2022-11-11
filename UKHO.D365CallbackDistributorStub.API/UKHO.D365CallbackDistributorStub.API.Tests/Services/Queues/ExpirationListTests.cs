
using NUnit.Framework;
using UKHO.D365CallbackDistributorStub.API.Services.Data;

namespace UKHO.D365CallbackDistributorStub.API.Tests.Services.Queues
{

    public class ExpirationListTests
    {
        [Test]
        public void CannotHaveNegativeOrZeroExpiry()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var expirationList = new ExpirationList<Message>(TimeSpan.Zero);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                var expirationList = new ExpirationList<Message>(TimeSpan.FromMinutes(-1));
            });
        }

        [Test]
        public void CanAddAndRetrieveItemBeforeExpiry()
        {
            var expirationList = new ExpirationList<Message>(TimeSpan.FromHours(1));

            var message0 = new Message("message 0");
            var message1 = new Message("message 1");
            var message2 = new Message("message 2");

            expirationList.Add(message0);
            expirationList.Add(message1);
            expirationList.Add(message2);

            CollectionAssert.Contains(expirationList, message0);
            CollectionAssert.Contains(expirationList, message1);
            CollectionAssert.Contains(expirationList, message2);
        }

        [Test]
        public void CannotAddAndRetrieveItemBeforeExpiry()
        {
            var expirationList = new ExpirationList<Message>(TimeSpan.FromSeconds(1));

            var message0 = new Message("message 0");
            var message1 = new Message("message 1");
            var message2 = new Message("message 2");

            expirationList.Add(message0);
            expirationList.Add(message1);
            expirationList.Add(message2);

            Thread.Sleep(TimeSpan.FromSeconds(3));

            CollectionAssert.IsEmpty(expirationList);
        }

        [Test]
        public void CanRemoveItemFromList()
        {
            var expirationList = new ExpirationList<Message>(TimeSpan.FromHours(1));

            var message0 = new Message("message 0");
            var message1 = new Message("message 1");
            var message2 = new Message("message 2");

            expirationList.Add(message0);
            expirationList.Add(message1);
            expirationList.Add(message2);

            expirationList.Remove(message1);

            CollectionAssert.Contains(expirationList, message0);
            CollectionAssert.DoesNotContain(expirationList, message1);
            CollectionAssert.Contains(expirationList, message2);
        }
    }
}
