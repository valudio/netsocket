using System;

namespace NetSocket.Sockets.Events
{
    public class SocketEventArgs : EventArgs
    {
        public IClient FromClient { get; }
        public SocketDirection SocketDirection { get; protected set; }

        public SocketEventArgs(IClient client)
        {
            FromClient = client;
            SocketDirection = SocketDirection.NotDefined;
        }
    }

}
