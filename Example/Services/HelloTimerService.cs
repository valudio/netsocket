using NetSocket.Sockets;
using System;

namespace Example.Services
{
    [SocketService(false)]
    public class HelloTimerService : SocketServiceBase
    {
        public HelloTimerService(ISocketManager manager) : base(manager)
        {
            var timer = new System.Timers.Timer(60000);
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            timer.Start();
        }

        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            await SendAllClientsAsync($"Hello at {DateTime.Now.ToUniversalTime()}");
        }

    }
}
