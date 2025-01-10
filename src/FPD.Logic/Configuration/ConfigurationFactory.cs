using System;
using System.Reflection;

using Effanville.Common.UI;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.FPD.Logic.Configuration;

public static class ConfigurationFactory
{
    public static IConfiguration LoadConfig(this IServiceProvider provider)
    {
        UiGlobals globals = provider.GetService<UiGlobals>();
        Assembly assembly = Assembly.GetExecutingAssembly();
        AssemblyName name = assembly.GetName();
        if (name.Name == null)
        {
            return null;
        }

        string configLocation = globals.CurrentFileSystem.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            name.Name,
            "user.config");
        return UserConfiguration.LoadFromUserConfigFile(
            configLocation, 
            globals.CurrentFileSystem, 
            globals.ReportLogger);
    }
}