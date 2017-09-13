using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using NetSocket.Sockets;

namespace NetSocket.Middleware
{
    public class RealTimeCommMiddleWare
    {
        private readonly ISocketManager _socketManager;
        private readonly RequestDelegate _next;
        private readonly IEnumerable<int> _ports;

        public RealTimeCommMiddleWare(RequestDelegate next, ISocketManager socketManager, ISocketServiceLoader loader, IEnumerable<int> ports)
        {
            _socketManager = socketManager;
            _ports = ports;
            loader.LoadServices(_socketManager);
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

#pragma warning disable IDE1006 // Naming Styles
        public async Task Invoke(HttpContext context)
#pragma warning restore IDE1006 // Naming Styles
        {
            if (context.WebSockets.IsWebSocketRequest && CheckPorts(context))
            {
                var additionalParameters = new Dictionary<string, StringValues>();
                if (context.Request.QueryString.HasValue)
                {
                    var queryString = context.Request.QueryString.Value;
                    additionalParameters = QueryHelpers.ParseQuery(queryString);
                }
                var ws = await context.WebSockets.AcceptWebSocketAsync();
                await _socketManager.AddClientAsync(ws, context.Connection.RemoteIpAddress, additionalParameters);
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        private bool CheckPorts(HttpContext context)
        {
            if (!_ports.Any()) return true;
            return context.Request.Host.Port.HasValue && _ports.Contains(context.Request.Host.Port.Value);
        }
    }
}
