using FinancialStructures.ReportingStructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FPDconsole
{
    class Program
    {
        static void WriteReports(ErrorReports reports)
        {
            foreach (var report in reports.GetReports())
            {
                Console.WriteLine(report.ToString());
            }
        }

        static void WriteReports(string message)
        {
            Console.WriteLine(message);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("FPDconsole.exe - version 0.1");
            if (args.Length == 0)
            {
                ExecuteCommands.DisplayHelp();
                return;
            }
            var reports = new ErrorReports();

            List<TextToken> values = ArgumentParser.Parse(args, reports);
            if (reports.GetReports(Location.Parsing).Any())
            {
                WriteReports(reports);
                ExecuteCommands.DisplayHelp();
                return;
            }

            ExecuteCommands.RunCommands(values, message => WriteReports(message), report => WriteReports(report), reports);

            Console.WriteLine("Program finished");
        }
    }
}
