﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Example.NetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:5000", "http://*.5001")
                .UseStartup<Startup>()
                .Build();
    }
}
