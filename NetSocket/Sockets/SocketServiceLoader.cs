using System.Collections.Generic;

namespace NetSocket.Sockets
{
    public class SocketServiceLoader : ISocketServiceLoader
    {
        private readonly IEnumerable<ISocketService> _services;
        public SocketServiceLoader(IEnumerable<ISocketService> services)
        {
            _services = services;
        }

        public void LoadServices(ISocketManager socketManager)
        {
            foreach (var service in _services)
            {
                service.Start(socketManager);
            }
        }
    }
}
