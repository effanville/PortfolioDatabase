using FinancialStructures.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FPDconsole
{
    class Program
    {
        static Action<string, string, string> ReportLogger => (type, location, message) => WriteReport(type, location, message);
        static void WriteReport(string errorType, string location, string message)
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
