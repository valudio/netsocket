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
        public Guid Id { get; }
        public Dictionary<string, StringValues> AdditionalParameters { get; }

        public Client(WebSocket webSocket, IPAddress ip, Dictionary<string, StringValues> additionalParameters)
        {
            WebSocket = webSocket;
            Ip = ip;
            Id = Guid.NewGuid();
            AdditionalParameters = additionalParameters;
        }

        #region [IDisposable]


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || WebSocket == null) return;
            WebSocket.Dispose();
            WebSocket = null;
        }

        ~Client()
        {
            Dispose(false);
        }

        #endregion
    }
}
