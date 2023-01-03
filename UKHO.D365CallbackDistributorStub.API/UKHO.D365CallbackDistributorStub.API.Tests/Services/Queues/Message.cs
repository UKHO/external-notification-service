namespace UKHO.D365CallbackDistributorStub.API.Tests.Services.Queues
{

    public class Message
    {
        private readonly string _data;

        public Message(string data)
        {
            _data = data;
        }

        public string Data => _data;
    }
}
