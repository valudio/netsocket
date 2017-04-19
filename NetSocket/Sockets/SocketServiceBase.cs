using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetSocket.Sockets.Events;

namespace NetSocket.Sockets
{
    public abstract class SocketServiceBase : ISocketService, IDisposable
    {
        private bool _isStarted;
        private ISocketManager Manager { get; set; }
        public Guid Id { get; }
        protected SocketServiceBase()
        {
            Id = Guid.NewGuid();
        }

        public void Start(ISocketManager manager)
        {
            if (_isStarted) return;
            Manager = manager;
            Manager.OnInit += Manager_OnInit;
            Manager.OnMessage += Manager_OnMessage;
            Manager.OnSend += Manager_OnSend;
            Manager.OnClose += Manager_OnClose;
            _isStarted = true;
        }

        public async Task SendAllClientsAsync(string message, IClient fromClient = null)
        {
            await SendClientsAsync(Manager.Clients, message, fromClient);
        }

        public async Task SendAllClientsExceptAsync(IClient except, string message, IClient fromClient = null)
        {
            var clients = Manager.Clients.Where(c => c != except);
            await SendClientsAsync(clients, message, fromClient);
        }
        public async Task SendAllClientsExceptAsync(IEnumerable<IClient> except, string message, IClient fromClient = null)
        {
            var clients = Manager.Clients.Where(c => !Manager.Clients.Contains(c));
            await SendClientsAsync(clients, message, fromClient);
        }

        public async Task SendClientAsync(IClient toClient, string message, IClient fromClient = null)
        {
            await Manager.SendAsync(toClient, message, fromClient);
        }

        private async Task SendClientsAsync(IEnumerable<IClient> clients, string message, IClient fromClient)
        {
            // if we use parallel library we won't have async.
            // as this is only intended for small number of connections
            // we won't use it.
            var clientList = clients.ToList();
            foreach (var client in clientList)
            {
                await SendClientAsync(client, message, fromClient);
            }
        }

        #region [handlers]

        private void Manager_OnInit(object sender, SocketEventArgs e)
        {
            OnClientInitialized(e.FromClient);
        }

        private void Manager_OnClose(object sender, SocketEventArgs e)
        {
            OnClientClosed(e.FromClient);
        }

        private void Manager_OnMessage(object sender, SocketReceiveEventArgs e)
        {
            OnMessageReceived(e.FromClient, e.Message);
        }

        private void Manager_OnSend(object sender, SocketSentEventArgs e)
        {
            OnMessageSent(e.ToClient, e.Message, e.FromClient);
        }

        #endregion

        #region [virtual]

        public virtual void OnClientInitialized(IClient client) { }

        public virtual void OnMessageReceived(IClient fromClient, string message) { }

        public virtual void OnMessageSent(IClient toClient, string message, IClient fromClient) { }

        public virtual void OnClientClosed(IClient client) { }

        #endregion

        #region [IDisposable]

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || Manager == null) return;
            Manager.OnInit -= Manager_OnInit;
            Manager.OnMessage -= Manager_OnMessage;
            Manager.OnSend -= Manager_OnSend;
            Manager.OnClose -= Manager_OnClose;
            Manager = null;
        }

        ~SocketServiceBase()
        {
            Dispose(false);
        }

        #endregion
    }
}
