using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Common.Console;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FPDconsole
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            //builder.Configuration.AddCommandLine(args);
            builder.Services.AddConsoleContext(
                new List<Type>() { typeof(DownloadCommand), typeof(ImportCommand), typeof(StatisticsCommand) },
                args);
            IHost host = builder.Build();
            await host.RunAsync();
        }
    }
}