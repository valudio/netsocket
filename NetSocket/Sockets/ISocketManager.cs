using System.Net;
using System.Net.WebSockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetSocket.Sockets.Events;

namespace PhoneNotifier.WS.Core.Sockets
{
    public interface ISocketManager
    {
        event SocketEventHandler OnInit;
        event SocketEventHandler OnClose;
        event SocketReceiveEventHandler OnMessage;
        event SocketSentEventHandler OnSend;
        List<IClient> Clients { get;} 
        Task AddClientAsync(WebSocket ws, IPAddress ip);
        Task SendAsync(IClient toClient, string message, IClient fromClient = null);
    }
}