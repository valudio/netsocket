using System;
using System.Net;
using System.Net.WebSockets;

namespace PhoneNotifier.WS.Core.Sockets
{
    public class Client : IClient
    {
        public WebSocket WebSocket { get; private set; }
        public IPAddress Ip { get; }
        public Guid Id { get; private set; }

        public Client(WebSocket webSocket, IPAddress ip)
        {
            WebSocket = webSocket;
            Ip = ip;
            Id = Guid.NewGuid();
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
