namespace NetSocket.Sockets.Events
{
    public delegate void SocketReceiveEventHandler(object sender, SocketReceiveEventArgs e);
    public delegate void SocketSentEventHandler(object sender, SocketSentEventArgs e);
    public delegate void SocketEventHandler(object sender, SocketEventArgs e);
}
