using FinancialStructures.ReportLogging;
using System;
using System.Collections.Generic;

namespace FPDconsole
{
    class Program
    {
        static LogReporter ReportLogger = new LogReporter(WriteReport);
        static void WriteReport(string detailLevel, string errorType, string location, string message)
        {
            Console.WriteLine(errorType + " - " + location + " - " + message);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("FPDconsole.exe - version 0.1");
            if (args.Length == 0)
            {
                ExecuteCommands.DisplayHelp();
                return;
            }

            List<TextToken> values = ArgumentParser.Parse(args, ReportLogger);

            ExecuteCommands.RunCommands(values, ReportLogger);

            Console.WriteLine("Program finished");
        }
    }
}
