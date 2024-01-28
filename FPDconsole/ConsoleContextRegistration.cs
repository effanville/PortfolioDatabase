using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Common.Console.Commands;
using Common.Console.Reporting;

using Microsoft.Extensions.DependencyInjection;

namespace Common.Console;

public static class ConsoleContextRegistration
{
    public static IServiceCollection AddConsoleContext(
        this IServiceCollection serviceCollection,
        IEnumerable<Type> consoleCommandTypes,
        string[] args)
    {
        serviceCollection.AddSingleton<IFileSystem, FileSystem>();
        serviceCollection.AddReporting();
        serviceCollection.AddConsoleCommands(consoleCommandTypes);
        serviceCollection.AddScoped<ConsoleCommandArgs>(_ => new ConsoleCommandArgs(args));
        serviceCollection.AddHostedService<ConsoleHost>();
        return serviceCollection;
    }
}