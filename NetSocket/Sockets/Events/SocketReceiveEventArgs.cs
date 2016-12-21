using PhoneNotifier.WS.Core.Sockets;

namespace NetSocket.Sockets.Events
{
    public class SocketReceiveEventArgs : SocketEventArgs
    {
        public string Message { get; private set; }

        public SocketReceiveEventArgs(IClient client, string message): base(client)
        {
            SocketDirection = SocketDirection.In;
            Message = message;
        }
    }
}
