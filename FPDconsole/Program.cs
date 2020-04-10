using FinancialStructures.Reporting;
using System;
using System.Collections.Generic;

namespace FPDconsole
{
    class Program
    {
        static void Main(string[] args)
        {
            void WriteReport(ReportSeverity detailLevel, ReportType errorType, ReportLocation location, string message)
            {
                Console.WriteLine(errorType + " - " + location + " - " + message);
            }
            LogReporter ReportLogger = new LogReporter(WriteReport);
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
