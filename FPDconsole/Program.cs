using FinancialStructures.Database;
using FinancialStructures.ReportingStructures;
using System;
using System.Linq;
using System.Collections.Generic;

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
                
            }
            var reports = new ErrorReports();

            List<TextToken> values = ArgumentParser.Parse(args, reports);
            if (reports.GetReports(Location.Parsing).Any())
            {
                WriteReports(reports);
                Console.WriteLine("Errors in inputs. Try again");
                Console.WriteLine("A more helpful message will soon be written.");
                return;
            }

            ExecuteCommands.RunCommands(values, message => WriteReports(message), report => WriteReports(report), reports);

            Console.WriteLine("Program finished");
        }
    }
}
