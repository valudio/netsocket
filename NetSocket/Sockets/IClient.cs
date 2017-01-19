using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using Microsoft.Extensions.Primitives;

namespace NetSocket.Sockets
{
    public interface IClient : IDisposable
    {
        Guid Id { get; }
        IPAddress Ip { get; }
        WebSocket WebSocket { get; }
        Dictionary<string, StringValues> AdditionalParameters { get; }
    }
}