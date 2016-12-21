using System;
using System.Net;
using System.Net.WebSockets;

namespace PhoneNotifier.WS.Core.Sockets
{
    public interface IClient : IDisposable
    {
        Guid Id { get; }
        IPAddress Ip { get; }
        WebSocket WebSocket { get; }
    }
}