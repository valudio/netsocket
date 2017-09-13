using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NetSocket.Sockets.Events;

namespace NetSocket.Sockets
{
    internal class SocketManager : IDisposable, ISocketManager
    {
        private readonly ILogger<SocketManager> _logger;
        private bool _isDisposed;

        public ConcurrentDictionary<Guid, IClient> Clients { get; private set; } = new ConcurrentDictionary<Guid, IClient>();

        public event SocketEventHandler OnInit;
        public event SocketEventHandler OnClose;
        public event SocketReceiveEventHandler OnMessage;
        public event SocketSentEventHandler OnSend;

        public SocketManager(ILogger<SocketManager> logger)
        {
            _logger = logger;
        }

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
            try
            {
                if (toClient?.WebSocket == null || toClient.WebSocket.State != WebSocketState.Open) return;
                await toClient.WebSocket.SendAsync(message);
                OnSend?.Invoke(this, new SocketSentEventArgs(toClient, fromClient, message));
            }
            catch (Exception e)
            {
                // most probable cause is a secondary thread disposing the client and the websocket before sending the message
                _logger.LogDebug($"Message to {toClient?.Id} from  {fromClient?.Id} has not been sent: {e}");
            }

        }

        private async Task ListeningLoopAsync(IClient client)
        {
            AddClient(client);
            OnInit?.Invoke(this, new SocketEventArgs(client));
            try
            {
                await ReceiveAsync(client);
            }
            catch (Exception e)
            {
                // connection unexpectedly closed
                // https://github.com/aspnet/WebSockets/issues/63
                _logger.LogDebug($"Client {client?.Id} with {client?.Ip} has been unexpectedly closed: {e}");
            }

            OnClose?.Invoke(this, new SocketEventArgs(client));
            RemoveClient(client);
        }

        private async Task ReceiveAsync(IClient client)
        {
            while (client.WebSocket.State == WebSocketState.Open)
            {
                string serializedMessage;
                WebSocketReceiveResult response;
                var buffer = new ArraySegment<byte>(new byte[1024 * 4]);

                using (var ms = new MemoryStream())
                {
                    do
                    {
                        response = await client.WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, response.Count);
                    }
                    while (!response.EndOfMessage);
             
                    ms.Position = 0;
                
                    using (var sr = new StreamReader(ms, Encoding.UTF8))
                    {
                        serializedMessage = await sr.ReadToEndAsync();
                    }
                }

                if (response.MessageType == WebSocketMessageType.Close)
                {
                    // https://github.com/aspnet/KestrelHttpServer/issues/989
                    await client.WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Close response received", CancellationToken.None);
                }
                else
                {
                    OnMessage?.Invoke(this, new SocketReceiveEventArgs(client, serializedMessage));
                }
            }
        }

        private void AddClient(IClient client)
        {
            if (Clients == null)
            {
                throw new Exception($"It seems that the SocketManager has been disposed: {_isDisposed}");
            }
            if (Clients.ContainsKey(client.Id))
            {
                RemoveClient(client);
            }
            if (!Clients.TryAdd(client.Id, client))
            {
                _logger.LogError("Client {Id} with {Ip} has been connected and added", client.Id, client.Ip);
            }
            _logger.LogDebug("Client {Id} with {Ip} has been connected and added", client.Id, client.Ip);
        }

        private void RemoveClient(IClient client)
        {
            var isRemoved = client == null || Clients.TryRemove(client.Id, out client);
            if (isRemoved) client?.Dispose();
            _logger.LogDebug("Client {Id} with {Ip} has been disconnected and removed", client?.Id, client?.Ip);
        }

        #region [IDisposable]


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || Clients == null || _isDisposed) return;
            Clients.Values.ToList().ForEach(x => x.Dispose());
            Clients.Clear();
            Clients = null;
            _isDisposed = true;
            _logger.LogDebug("SocketManager has been disposed");
        }

        ~SocketManager()
        {
            Dispose(false);
        }

        #endregion
    }
}
