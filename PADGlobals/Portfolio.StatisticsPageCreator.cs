using FinancialStructures.FinanceStructures;
using SecurityStatisticsFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using FinancialStructures.Mathematics;
using GUIAccessorFunctions;
using FinancialStructures.DataStructures;
using BankAccountHelperFunctions;

namespace PortfolioStatsCreatorHelper
{
    public static class PortfolioStatsCreators
    {
        public static bool CreateHTMLPage(Portfolio funds, List<string> toExport, string filepath)
        {
            StreamWriter htmlWriter = new StreamWriter(filepath);
            CreateHTMLHeader(htmlWriter);

            htmlWriter.WriteLine(" ");
            htmlWriter.WriteLine("Funds Data");
            htmlWriter.WriteLine(" ");

            var totals = DatabaseAccessor.GeneratePortfolioStatistics();
            var properties = totals.GetType().GetProperties();
            string header = string.Empty;
            foreach (var props in properties)
            {
                if (props.PropertyType == typeof(string))
                { 
                    header += props.Name.PadRight(25);
                }

                if (props.PropertyType == typeof(double))
                { 
                    header += props.Name.PadRight(15);
                }
            }

            htmlWriter.WriteLine(header);
            htmlWriter.WriteLine("");
            List<string> companies = funds.GetSecuritiesCompanyNames();
            foreach (string compName in companies)
            {
                
                var securities = DatabaseAccessor.GenerateCompanyFundsStatistics(compName);
                foreach (var sec in securities)
                {
                    string line = string.Empty;
                    foreach (var props in properties)
                    {
                        if (Double.TryParse(props.GetValue(sec).ToString(), out double result))
                        {
                            line += result.ToString().PadRight(15);
                        }
                        else
                        {
                            line += props.GetValue(sec).ToString().PadRight(25);
                        }
                    }
                    htmlWriter.WriteLine(line);
                }

                htmlWriter.WriteLine("");
            }

            htmlWriter.WriteLine("");
            string fundTotalLine = string.Empty;
            foreach (var props in properties)
            {
                if (Double.TryParse(props.GetValue(totals).ToString(), out double result))
                {
                    fundTotalLine += result.ToString().PadRight(15);
                }
                else 
                {
                    fundTotalLine += props.GetValue(totals).ToString().PadRight(25);
                }
            }

            htmlWriter.WriteLine(fundTotalLine);
            htmlWriter.WriteLine("");
            DailyValuation_Named bankTotals = new DailyValuation_Named("Totals", string.Empty, DateTime.Today, BankAccountEditor.AllBankAccountValue(DateTime.Today));
            var bankProperties = bankTotals.GetType().GetProperties();
            string bankheader = string.Empty;

            foreach (var props in properties)
            {
                if (props.PropertyType == typeof(string))
                {
                    bankheader += props.Name.PadRight(25);
                }

                if (props.PropertyType == typeof(double))
                {
                    bankheader += props.Name.PadRight(15);
                }
            }

            htmlWriter.WriteLine(bankheader);
            htmlWriter.WriteLine("");

            List<string> BankCompanies = funds.GetBankAccountCompanyNames();
            foreach (string compName in BankCompanies)
            {
                var bankAccounts = funds.GenerateBankAccountStatistics(compName);
                foreach (var acc in bankAccounts)
                {
                    string line = string.Empty;
                    foreach (var prop in bankProperties)
                    {
                        if (Double.TryParse(prop.GetValue(acc).ToString(), out double result))
                        {
                            line += result.ToString().PadRight(15);
                        }
                        else
                        {
                            if (prop.PropertyType == typeof(string))
                            {
                                line += prop.GetValue(acc).ToString().PadRight(25);
                            }
                        }
                    }

                    htmlWriter.WriteLine(line);
                }

                htmlWriter.WriteLine("");
            }

            WriteSeparatorLine(htmlWriter);

            string totalAccountsLine = string.Empty;
            foreach (var prop in bankProperties)
            {
                if (Double.TryParse(prop.GetValue(bankTotals).ToString(), out double result))
                {
                    totalAccountsLine += result.ToString().PadRight(15);
                }
                else
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        totalAccountsLine += prop.GetValue(bankTotals).ToString().PadRight(25);
                    }
                }
            }

            htmlWriter.WriteLine(totalAccountsLine);
            htmlWriter.WriteLine("");
            WriteSeparatorLine(htmlWriter);

            DailyValuation_Named portfolioTotals = new DailyValuation_Named("Totals", string.Empty, DateTime.Today, SecurityStatistics.TotalValue(DateTime.Today));
            var portfolioProperties = bankTotals.GetType().GetProperties();
            string portfolioHeader = string.Empty;
            htmlWriter.WriteLine("Name".PadRight(25) + "Value".PadRight(15));
            foreach (var props in portfolioProperties)
            {
                if (props.PropertyType == typeof(string))
                {
                    portfolioHeader += props.Name.PadRight(25);
                }

                if (props.PropertyType == typeof(double))
                {
                    portfolioHeader += props.Name.PadRight(15);
                }
            }

            htmlWriter.WriteLine(portfolioHeader);

            string totalLine = string.Empty;
            foreach (var prop in portfolioProperties)
            {
                if (Double.TryParse(prop.GetValue(portfolioTotals).ToString(), out double result))
                {
                    totalLine += result.ToString().PadRight(15);
                }
                else
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        totalLine += prop.GetValue(portfolioTotals).ToString().PadRight(25);
                    }
                }
            }


            htmlWriter.WriteLine(totalLine);

            CreateHTMLFooter(htmlWriter);
            htmlWriter.Close();
            return true;
        }

        private static void WriteSeparatorLine(StreamWriter writer)
        {
            writer.WriteLine("=========================================================================================================="); 
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
            WriteSeparatorLine(writer);

            writer.WriteLine("<h1>Portfolio Statement</h1>");
            writer.WriteLine($"on the date {DateTime.Today.ToShortDateString()}.");
            writer.WriteLine($"Produced by Finance Portfolio Database v{GlobalHeldData.GlobalData.versionNumber}");
            writer.WriteLine("as written by Matthew Egginton");

            WriteSeparatorLine(writer);
        }

        private static void CreateHTMLFooter(StreamWriter writer)
        {
            WriteSeparatorLine(writer);
            writer.WriteLine("</span></pre>");
            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
        }
    }
}
