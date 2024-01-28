using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

namespace Common.Console.Commands;

public static class ConsoleCommandRegistration
{
    public static IServiceCollection AddConsoleCommands(
        this IServiceCollection serviceCollection,
        IEnumerable<Type> consoleCommandTypes)
    {
        foreach (var commandType in consoleCommandTypes)
        {
            serviceCollection.AddScoped(typeof(ICommand), commandType);
        }

        return serviceCollection;
    }
}