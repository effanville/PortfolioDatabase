using System;

using Common.Structure.Reporting;

using Microsoft.Extensions.DependencyInjection;

namespace Common.Console.Reporting;

public static class ConsoleInstanceRegistration
{
    public static IServiceCollection AddReporting(
        this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IReportLogger, LogReporter>(_ =>
            new LogReporter(ReportAction, saveInternally: true));
        return serviceCollection.AddSingleton<IConsole, ConsoleInstance>(
            _ => new ConsoleInstance(WriteError, WriteLine));
    }
    
    private static void WriteError(string text)
    {
        var color = System.Console.ForegroundColor;
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine(text);
        System.Console.ForegroundColor = color;
    }

    // Create the Console to write output.
    private static void WriteLine(string text) => System.Console.WriteLine(text);

    // Create the logger.
    private static void ReportAction(ReportSeverity severity, ReportType reportType, string location, string text)
    {
        string message = $"[{DateTime.Now}]-({reportType}) - [{location}] - {text}";
        if (reportType == ReportType.Error)
        {
            WriteError(message);
        }
        else
        {
            WriteLine(message);
        }
    }
}