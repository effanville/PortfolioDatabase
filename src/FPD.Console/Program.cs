using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Effanville.Common.Console;
using Effanville.Common.Structure.Reporting.LogAspect;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Persistence;
using Effanville.FPD.Console.Utilities.Mail;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Effanville.FPD.Console
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddScoped<LogInterceptor>()
                .AddScoped<IMailSender, MailSender>()
                .AddScoped<IPersistence<IPortfolio>, PortfolioPersistence>();
            IHost host = builder.SetupConsole(
                    args,
                    new List<Type>() { typeof(DownloadCommand), typeof(ImportCommand), typeof(StatisticsCommand) })
                .Build();
            await host.RunAsync();
        }
    }
}