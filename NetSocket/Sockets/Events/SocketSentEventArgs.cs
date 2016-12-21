namespace NetSocket.Sockets.Events
{
    public class SocketSentEventArgs : SocketEventArgs
    {
        public IClient ToClient { get; }
        public string Message { get; }

        public SocketSentEventArgs(IClient toClient, IClient fromClient,  string message) : base(fromClient)
        {
            ToClient = toClient;
            Message = message;
            SocketDirection = SocketDirection.Out;
        }
    }

}
