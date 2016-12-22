using System;
using System.Net;
using System.Net.WebSockets;

namespace NetSocket.Sockets
{
    public interface IClient : IDisposable
    {
        Guid Id { get; }
        IPAddress Ip { get; }
        WebSocket WebSocket { get; }
    }
}