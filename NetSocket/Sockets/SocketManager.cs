using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using NetSocket.Sockets.Events;

namespace NetSocket.Sockets
{
    internal class SocketManager : IDisposable, ISocketManager
    {
        // TODO take care of locks while accessing the client list
        private static readonly List<IClient> _clients = new List<IClient>();

        public List<IClient> Clients => _clients;

        public event SocketEventHandler OnInit;
        public event SocketEventHandler OnClose;
        public event SocketReceiveEventHandler OnMessage;
        public event SocketSentEventHandler OnSend;

        public async Task AddClientAsync(WebSocket ws, IPAddress ip, Dictionary<string, StringValues> additionalParameters)
        {
            var client = new Client(ws, ip, additionalParameters);
            await Task.Run(async () =>
            {
                await ListeningLoopAsync(client);
            });
        }

        public async Task SendAsync(IClient toClient, string message, IClient fromClient)
        {
            if (toClient.WebSocket.State != WebSocketState.Open) return;
            await toClient.WebSocket.SendAsync(message);
            OnSend?.Invoke(this, new SocketSentEventArgs(toClient, fromClient, message));
        }

        private async Task ListeningLoopAsync(IClient client)
        {
            AddClient(client);
            var ws = client.WebSocket;
            var buffer = new byte[1024 * 4];
            OnInit?.Invoke(this, new SocketEventArgs(client));
            try
            {

                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    OnMessage?.Invoke(this, new SocketReceiveEventArgs(client, buffer.GetResponse()));
                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
            catch (IOException)
            {
                var number = 2;
                // closed connection unexpectly
                // https://github.com/aspnet/WebSockets/issues/63
            }

            OnClose?.Invoke(this, new SocketEventArgs(client));
            RemoveClient(client);
        }

        private void AddClient(IClient client)
        {
            var sameIdClient = Clients.Find(c => c.Id == client.Id);
            if (sameIdClient != null)
            {
                RemoveClient(sameIdClient);
            }
            Clients.Add(client);
        }

        private void RemoveClient(IClient client)
        {
            var isRemoved = client == null || Clients.Remove(client);
            if (isRemoved) client?.Dispose();
        }

        #region [IDisposable]

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~SocketManager()
        {
            ReleaseUnmanagedResources();
        }

        #endregion
    }
}
