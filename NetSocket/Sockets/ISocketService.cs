using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetSocket.Sockets
{
    public interface ISocketService
    {
        Guid Id { get; }
        ConcurrentDictionary<Guid, IClient> Clients { get; }
        void Start(ISocketManager manager);
        Task SendAllClientsAsync(string message, IClient fromClient = null);
        Task SendAllClientsExceptAsync(IClient except, string message, IClient fromClient = null);
        Task SendAllClientsExceptAsync(IEnumerable<IClient> except, string message, IClient fromClient = null);
        Task SendClientAsync(IClient toClient, string message, IClient fromClient = null);
        void OnMessageReceived(IClient fromClient, string message);
        void OnMessageSent(IClient toClient, string message, IClient fromClient);
        void OnClientInitialized(IClient client);
        void OnClientClosed(IClient client);
    }
}
