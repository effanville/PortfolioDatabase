using System;
using System.Reflection;

using Effanville.Common.UI;
using Effanville.FPD.Logic.Configuration;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.FPD.UI;

public static class ConfigurationFactory
{
    public static IConfiguration LoadConfig(IServiceProvider provider)
    {
        var globals = provider.GetService<UiGlobals>();
        Assembly assembly = Assembly.GetExecutingAssembly();
        AssemblyName name = assembly.GetName();
        string configLocation = globals.CurrentFileSystem.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), name.Name, "user.config");
        return UserConfiguration.LoadFromUserConfigFile(
            configLocation, 
            globals.CurrentFileSystem, 
            globals.ReportLogger);
    }
}