using FinanceStructures;
using SecurityStatisticsFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using mathematics;

namespace PortfolioStatsCreatorHelper
{
    public static class PortfolioStatsCreators
    {



        public static bool TryCreateHTMLPage(Portfolio funds, List<string> toExport, string filepath)
        {
            StreamWriter htmlWriter = new StreamWriter(filepath);
            CreateHTMLHeader(htmlWriter);

            htmlWriter.WriteLine(" ");
            htmlWriter.WriteLine("Funds Data");
            htmlWriter.WriteLine(" ");
            string header = "name".PadRight(25) + "Latest Value".PadRight(15) + "CAR 3 Months(%)".PadRight(15) + "CAR 6 Months(%)".PadRight(15) + "CAR 1 Year(%)".PadRight(15) + "CARTotal(%)".PadRight(15);
            htmlWriter.WriteLine(header);
            List<string> companies = funds.GetSecuritiesCompanyNames();
            foreach (string compName in companies)
            {
                htmlWriter.WriteLine("");
                var securities = funds.CompanySecurities(compName);
                foreach (var sec in securities)
                {
                    string line = sec.GetName().PadRight(25) + MathSupport.Trunc(SecurityStatistics.SecurityLatestValue(sec.GetName(), compName), 3).PadRight(15) + MathSupport.Trunc(100 * SecurityStatistics.SecurityIRRTime(sec.GetName(), compName, DateTime.Today.AddMonths(-3), DateTime.Today), 3).PadRight(15) + MathSupport.Trunc(100 * SecurityStatistics.SecurityIRRTime(sec.GetName(), compName, DateTime.Today.AddMonths(-6), DateTime.Today), 3).PadRight(15) + MathSupport.Trunc(100 * SecurityStatistics.SecurityIRRTime(sec.GetName(), compName, DateTime.Today.AddMonths(-12), DateTime.Today), 3).PadRight(15) + MathSupport.Trunc(100 * SecurityStatistics.SecurityIRR(sec.GetName(), compName), 3).PadRight(15);
                    htmlWriter.WriteLine(line);
                }

                string compLine = compName.PadRight(25) + MathSupport.Trunc(funds.CompanyValue(compName, DateTime.Today), 4).PadRight(15) + MathSupport.Trunc(funds.IRRCompany(compName,DateTime.Today.AddMonths(-3), DateTime.Today), 4).PadRight(15) + MathSupport.Trunc(funds.IRRCompany(compName, DateTime.Today.AddMonths(-6), DateTime.Today), 4).PadRight(15) + MathSupport.Trunc(funds.IRRCompany(compName, DateTime.Today.AddMonths(-12), DateTime.Today), 4).PadRight(15);
                htmlWriter.WriteLine(compLine);
                htmlWriter.WriteLine("");

            }

            htmlWriter.WriteLine("");

            string totalLine = "Funds Total".PadRight(25) + MathSupport.Trunc(funds.AllSecuritiesValue(DateTime.Today), 4).PadRight(15) + MathSupport.Trunc(funds.IRRPortfolio(DateTime.Today.AddMonths(-3), DateTime.Today), 4).PadRight(15);
            htmlWriter.WriteLine(totalLine) ;
            htmlWriter.WriteLine("");

            htmlWriter.WriteLine("Name".PadRight(25) + "Value".PadRight(15));
            List<string> BankCompanies = funds.GetBankAccountCompanyNames();
            foreach (string compName in BankCompanies)
            {
                htmlWriter.WriteLine("");
                var bankAccounts = funds.CompanyBankAccounts(compName);
                foreach (CashAccount acc in bankAccounts)
                {
                    string line = acc.GetName().PadRight(25) + MathSupport.Trunc(acc.LatestValue().Value, 3).PadRight(15);
                    htmlWriter.WriteLine(line);
                }
                string compLine = compName.PadRight(25) + MathSupport.Trunc(funds.BankAccountCompanyValue(compName, DateTime.Today), 4).PadRight(15);
                htmlWriter.WriteLine(compLine);
                htmlWriter.WriteLine("");
            }

            string totalAccountsLine = "BankAccounts Total".PadRight(15) + MathSupport.Trunc(funds.AllBankAccountsValue(DateTime.Today), 4).PadRight(15);
            htmlWriter.WriteLine(totalAccountsLine);
            htmlWriter.WriteLine("");

            CreateHTMLFooter(htmlWriter);
            htmlWriter.Close();
            return true;
        }

        private static void CreateHTMLHeader(StreamWriter writer)
        {
            writer.WriteLine("<!DOCTYPE html>");
            writer.WriteLine("<META HTTP-EQUIV=\"Content - Type\" CONTENT=\"text / html; charset = UTF - 8\">");
            writer.WriteLine("<html>");
            writer.WriteLine("<head>");
            writer.WriteLine($"<title> Statement for funds as of {DateTime.Today.ToShortDateString()}</title>");
            writer.WriteLine("</head>");
            writer.WriteLine("<body>");
            writer.WriteLine("<pre style='color:#1f1c1b;background-color:#ffffff;'><span class=\"inner - pre\" style=\"font - size: 13px\">");
            writer.WriteLine("==========================================================================================================");

            writer.WriteLine("<h1>Portfolio Statement</h1>");
            writer.WriteLine($"on the date {DateTime.Today.ToShortDateString()}.");
            writer.WriteLine($"Produced by Finance Portfolio Database v{GlobalHeldData.GlobalData.versionNumber}");
            writer.WriteLine("as written by Matthew Egginton");

            writer.WriteLine("==========================================================================================================");
        }

        private static void CreateHTMLFooter(StreamWriter writer)
        {
            writer.WriteLine("===========================================================================================================");
            writer.WriteLine("</span></pre>");
            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
        }
    }
}
