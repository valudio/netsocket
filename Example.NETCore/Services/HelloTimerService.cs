using NetSocket.Sockets;
using System;

namespace Example.NETCore.Services
{
    [SocketService(true)]
    public class HelloTimerService : SocketServiceBase
    {
        public HelloTimerService(ISocketManager manager) : base(manager)
        {
            var timer = new System.Threading.Timer(async (o) =>
            {
                await SendAllClientsAsync($"Hello at {DateTime.Now.ToUniversalTime()}");
            }, null, 0, 30000);
            
        }
    }
}
