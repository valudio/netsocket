using System;
using Microsoft.AspNetCore.Builder;
using NetSocket.Sockets;
using PhoneNotifier.WS.Core.Sockets;

namespace NetSocket.Middleware
{
    public static class RealTimeCommMiddleWareExtensions
    {
        public static IApplicationBuilder UseRealTimeComm(this IApplicationBuilder app, ISocketServiceLoader loader = null)
        {
            if (app == null)
                throw new ArgumentException("app");
            app.UseWebSockets();
            return app.UseMiddleware<RealTimeCommMiddleWare>(new SocketManager(), loader ?? new SocketServiceLoader());
        }
    }
}
