using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using NetSocket.Sockets.Events;

namespace NetSocket.Sockets
{
    internal class SocketManager : IDisposable, ISocketManager
    {
        private static List<IClient> _clients = new List<IClient>();

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
            OnInit?.Invoke(this, new SocketEventArgs(client));
            try
            {
                await ReceiveAsync(client);
            }
            catch (IOException)
            {
                // connection unexpectedly closed
                // https://github.com/aspnet/WebSockets/issues/63
            }

            OnClose?.Invoke(this, new SocketEventArgs(client));
            RemoveClient(client);
        }

        // https://github.com/Vannevelj/StackSockets/blob/master/StackSockets/Library/StackSocket.cs
        private async Task ReceiveAsync(IClient client)
        {
            const int bufferSize = 1024;
            const int bufferAmplifier = 4;
            var temporaryBuffer = new byte[bufferSize];
            var buffer = new byte[bufferSize * bufferAmplifier];
            var offset = 0;

            while (client.WebSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult response;

                do
                {
                    response = await client.WebSocket.ReceiveAsync(new ArraySegment<byte>(temporaryBuffer), CancellationToken.None);
                    temporaryBuffer.CopyTo(buffer, offset);
                    offset += response.Count;
                    temporaryBuffer = new byte[bufferSize];
                } while (!response.EndOfMessage);

                if (response.MessageType == WebSocketMessageType.Close)
                {
                    // https://github.com/aspnet/KestrelHttpServer/issues/989
                    await client.WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Close response received", CancellationToken.None);
                }
                else
                {
                    var result = Encoding.UTF8.GetString(buffer);
                    OnMessage?.Invoke(this, new SocketReceiveEventArgs(client, result));

                    buffer = new byte[bufferSize * bufferAmplifier];
                    offset = 0;
                }
            }
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


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _clients == null) return;
            _clients.ForEach(x => x.Dispose());
            _clients = null;
        }

        ~SocketManager()
        {
            Dispose(false);
        }

        #endregion
    }
}
