namespace NetSocket.Sockets.Events
{
    public class SocketReceiveEventArgs : SocketEventArgs
    {
        public string Message { get; }

        public SocketReceiveEventArgs(IClient client, string message): base(client)
        {
            SocketDirection = SocketDirection.In;
            Message = message;
        }
    }
}
