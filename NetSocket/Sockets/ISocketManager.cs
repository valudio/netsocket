using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using NetSocket.Sockets.Events;

namespace NetSocket.Sockets
{
    public interface ISocketManager
    {
        event SocketEventHandler OnInit;
        event SocketEventHandler OnClose;
        event SocketReceiveEventHandler OnMessage;
        event SocketSentEventHandler OnSend;
        List<IClient> Clients { get;} 
        Task AddClientAsync(WebSocket ws, IPAddress ip, Dictionary<string, StringValues> additionalParameters);
        Task SendAsync(IClient toClient, string message, IClient fromClient = null);
    }
}