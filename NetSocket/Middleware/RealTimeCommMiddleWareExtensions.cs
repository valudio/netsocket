using System;
using Microsoft.AspNetCore.Builder;
using NetSocket.Sockets;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq;

namespace NetSocket.Middleware
{
    public static class RealTimeCommMiddleWareExtensions
    {
        public static IApplicationBuilder UseRealTimeComm(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentException("app");
            app.UseWebSockets();
            return app.UseMiddleware<RealTimeCommMiddleWare>();
        }

        public static IServiceCollection AddRealTimeComm(this IServiceCollection services)
        {
            services.AddSingleton<ISocketServiceLoader, SocketServiceLoader>();
            services.AddSingleton<ISocketManager, SocketManager>();

            Assembly
                .GetEntryAssembly()
                .GetTypes()
                .Where(t =>
                {
                    var info = t.GetTypeInfo();
                    var attr = info.GetCustomAttribute<SocketServiceAttribute>(false);
                    return !info.IsAbstract && !info.IsInterface && typeof(ISocketService).IsAssignableFrom(t) && (attr == null || attr.Enabled);
                })
                .ToList()
                .ForEach(x => services.AddScoped(typeof(ISocketService), x));

            return services;
        }
    }
}
