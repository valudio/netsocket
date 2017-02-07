using NetSocket.Sockets;

namespace Example.NETCore.Services
{
    [SocketService(true)]
    public class EchoService : SocketServiceBase
    {
        public EchoService() : base() { }

        public override async void OnMessageReceived(IClient fromClient, string message)
        {
            base.OnMessageReceived(fromClient, message);
            await SendAllClientsAsync($"{fromClient.Id} says: {message} <:> {Id}", fromClient);
        }
    }
}
