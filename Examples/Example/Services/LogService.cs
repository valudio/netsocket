using NetSocket.Sockets;

namespace Example.Services
{
    [SocketService(true)]
    public class LogService : SocketServiceBase
    {
        public override async void OnClientInitialized(IClient client)
        {
            await SendClientAsync(client, $"You're now connected {client.Id}");
            await SendAllClientsExceptAsync(client, $"{client.Id} is connected :: {Id}");
        }

        public override async void OnClientClosed(IClient client)
        {   
            await SendAllClientsExceptAsync(client, $"{client.Id} has been disconnected");
        }
    }
}
