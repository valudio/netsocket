using System;

namespace NetSocket.Sockets
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SocketServiceAttribute : Attribute
    {
        public bool Enabled { get; }

        public SocketServiceAttribute(bool enabled)
        {
            Enabled = enabled;
        }
    }
}
