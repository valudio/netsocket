
namespace Example.Services
{
    [SocketService(true)]
    public class EchoService : SocketServiceBase
    {
        public EchoService(ISocketManager manager) : base(manager) { }

        public override async void OnMessageReceived(IClient fromClient, string message)
        {
            base.OnMessageReceived(fromClient, message);
            await SendAllClientsAsync($"{fromClient.Id} says: {message} <:> {Id}", fromClient);
        }
    }
}
