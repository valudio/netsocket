using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PhoneNotifier.WS.Core.Sockets;

namespace NetSocket.Middleware
{
    public class RealTimeCommMiddleWare
    {
        private readonly ISocketManager _socketManager;
        private readonly RequestDelegate _next;

        public RealTimeCommMiddleWare(RequestDelegate next, ISocketManager socketManager, ISocketServiceLoader loader)
        {
            _socketManager = socketManager;
            loader.LoadServices(_socketManager);
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

#pragma warning disable IDE1006 // Naming Styles
        public async Task Invoke(HttpContext context)
#pragma warning restore IDE1006 // Naming Styles
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var ws = await context.WebSockets.AcceptWebSocketAsync();
                await _socketManager.AddClientAsync(ws, context.Connection.RemoteIpAddress);
            }
            else
            {
                await _next.Invoke(context);
            }
        }


    }
}
