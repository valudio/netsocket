using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using Microsoft.Extensions.Primitives;

namespace NetSocket.Sockets
{
    public class Client : IClient
    {
        public WebSocket WebSocket { get; private set; }
        public IPAddress Ip { get; }
        public Guid Id { get; private set; }
        public Dictionary<string, StringValues> AdditionalParameters { get; private set; }

        public Client(WebSocket webSocket, IPAddress ip, Dictionary<string, StringValues> additionalParameters)
        {
            WebSocket = webSocket;
            Ip = ip;
            Id = Guid.NewGuid();
            AdditionalParameters = additionalParameters;
        }

        #region [IDisposable]

        private void ReleaseUnmanagedResources()
        {
            WebSocket.Dispose();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Client()
        {
            ReleaseUnmanagedResources();
        }

        #endregion
    }
}
