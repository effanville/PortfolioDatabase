using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.ReportingStructures;
using System;
using System.Threading.Tasks;

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

        public static Portfolio portfolio = new Portfolio();
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                
            }
            Console.WriteLine("Program Started");
            var reports = new ErrorReports();
            portfolio.LoadPortfolio("", reports);

            DoStuff(reports).Wait();
            Console.WriteLine("Program finished");
        }

        public static async Task DoStuff(ErrorReports reports)
        {
            await Download.DownloadSecurityLatest(new Security("hi", "low", "GBP", "https://markets.ft.com/data/funds/tearsheet/summary?s=gb00b2pljn71:gbx"), (reports) => WriteReports(reports), reports);
        }
    }
}
