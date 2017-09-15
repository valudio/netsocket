using NetSocket.Sockets;

namespace Example.Services
{
    [SocketService(true)]
    public class EchoService : SocketServiceBase
    {
        public override async void OnMessageReceived(IClient fromClient, string message)
        {
            base.OnMessageReceived(fromClient, message);
            await SendAllClientsAsync($"{fromClient.Id} says: <b>{message}</b> ## service Id: {Id}", fromClient);
        }
    }
}
