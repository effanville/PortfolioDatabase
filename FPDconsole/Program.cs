using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Common.Console;
using Common.Console.Commands;
using Common.Structure.Extensions;
using Common.Structure.Reporting;

namespace FPDconsole
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            // Create the logger.
            void reportAction(ReportSeverity severity, ReportType reportType, string location, string text)
            {
                string message = $"[{DateTime.Now}]-({reportType}) - [{location}] - {text}";
                if (reportType == ReportType.Error)
                {
                    writeError(message);
                }
                else
                {
                    writeLine(message);
                }
            }
            IReportLogger logger = new LogReporter(reportAction, saveInternally: true);

            // Create the Console to write output.
            void writeLine(string text) => Console.WriteLine(text);
            void writeError(string text)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(text);
                Console.ForegroundColor = color;
            }
            IConsole console = new ConsoleInstance(writeError, writeLine);

            IFileSystem fileSystem = new FileSystem();

            // Define the acceptable commands for this program.
            var validCommands = new List<ICommand>()
            {
                new DownloadCommand(fileSystem),
                new StatisticsCommand(fileSystem),
                new ImportCommand(fileSystem),
            };

            logger.Log(ReportSeverity.Useful, ReportType.Information, $"FPDConsole", "FPDconsole.exe - version 1");

            // Generate the context, validate the arguments and execute.
            int returnValue = ConsoleContext.SetAndExecute(args, console, logger, validCommands);
            string logPath = fileSystem.Path.Combine(fileSystem.Directory.GetCurrentDirectory(), $"{DateTime.Now.FileSuitableDateTimeValue()}-consoleLog.log");
            logger.WriteReportsToFile(logPath);
            return returnValue;
        }
    }
}
