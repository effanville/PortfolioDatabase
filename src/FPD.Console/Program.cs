﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Effanville.Common.Console;
using Effanville.Common.Structure.Reporting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Effanville.FPD.Console
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            IHost host = builder.SetupConsole(
                    args,
                new List<Type>() { typeof(DownloadCommand), typeof(ImportCommand), typeof(StatisticsCommand) })
                .Build();
            await host.RunAsync();
        }
    }
}