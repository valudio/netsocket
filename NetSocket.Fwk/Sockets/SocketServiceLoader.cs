using System;
using System.Linq;
using System.Reflection;

namespace NetSocket.Sockets
{
    public class SocketServiceLoader : ISocketServiceLoader
    {
        public  void LoadServices(ISocketManager socketManager)
        {
            var asm = Assembly.GetEntryAssembly();
            var types = asm.GetTypes().Where(t => {
                var info = t.GetTypeInfo();
                return !info.IsAbstract && !info.IsInterface && typeof(ISocketService).IsAssignableFrom(t);
            }); 
            foreach (var type in types)
            {
                var attr = type.GetTypeInfo().GetCustomAttribute<SocketServiceAttribute>(false);
                if (attr != null && !attr.Enabled) continue;
                var srv = (ISocketService)Activator.CreateInstance(type, socketManager);
                srv.Start();
            }
        }
    }
}
