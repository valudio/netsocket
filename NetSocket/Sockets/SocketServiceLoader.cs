using System;
using System.Reflection;

namespace NetSocket.Sockets
{
    public class SocketServiceLoader : ISocketServiceLoader
    {
        public  void LoadServices(ISocketManager socketManager)
        {
            var types = GetType().Assembly.GetTypes().Where(t => typeof(ISocketService).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
            foreach (var type in types)
            {
                var attr = type.GetCustomAttributes(typeof(SocketServiceAttribute), false).SingleOrDefault();
                if (attr != null && !((SocketServiceAttribute)attr).Enabled) continue;
                var srv = (ISocketService)Activator.CreateInstance(type, socketManager);
                srv.Start();
            }
        }
    }
}
