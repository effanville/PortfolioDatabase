using FinancialStructures.FinanceStructures;
using SecurityStatisticsFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using GUIAccessorFunctions;
using FinancialStructures.DataStructures;
using BankAccountHelperFunctions;

namespace PortfolioStatsCreatorHelper
{
    public static class PortfolioStatsCreators
    {
        public static bool CreateHTMLPage(Portfolio funds, List<string> toExport, string filepath, bool DisplayValueFunds)
        {
            int maxStringLength = 25;
            StreamWriter htmlWriter = new StreamWriter(filepath);
            CreateHTMLHeader(htmlWriter);

            htmlWriter.WriteLine(" ");
            htmlWriter.WriteLine("Funds Data");
            htmlWriter.WriteLine(" ");

            var totals = DatabaseAccessor.GeneratePortfolioStatistics();
            var properties = totals.GetType().GetProperties();

            WriteHeader(htmlWriter, properties, maxStringLength);

            List<string> companies = funds.GetSecuritiesCompanyNames();
            foreach (string compName in companies)
            {
                
                var securities = DatabaseAccessor.GenerateCompanyFundsStatistics(compName);
                foreach (var sec in securities)
                {
                    if ((DisplayValueFunds && sec.LatestVal > 0) || !DisplayValueFunds)
                    {
                        string line = string.Empty;
                        foreach (var props in properties)
                        {
                            if (Double.TryParse(props.GetValue(sec).ToString(), out double result))
                            {
                                line += result.ToString().PadLeft(15);
                            }
                            else
                            {
                                line += props.GetValue(sec).ToString().PadRight(maxStringLength);
                            }
                        }

                        htmlWriter.WriteLine(line);
                    }
                }

                htmlWriter.WriteLine("");
            }

            if ((DisplayValueFunds && totals.LatestVal > 0) || !DisplayValueFunds)
            {
                htmlWriter.WriteLine("");
                string fundTotalLine = string.Empty;
                foreach (var props in properties)
                {
                    if (Double.TryParse(props.GetValue(totals).ToString(), out double result))
                    {
                        fundTotalLine += result.ToString().PadLeft(15);
                    }
                    else
                    {
                        fundTotalLine += props.GetValue(totals).ToString().PadRight(maxStringLength);
                    }
                }

                htmlWriter.WriteLine(fundTotalLine);
                htmlWriter.WriteLine("");
                htmlWriter.WriteLine("");
            }

            htmlWriter.WriteLine(" ");
            htmlWriter.WriteLine("Bank Accounts Data");
            htmlWriter.WriteLine(" ");

            DailyValuation_Named bankTotals = new DailyValuation_Named("Totals", string.Empty, DateTime.Today, BankAccountEditor.AllBankAccountValue(DateTime.Today));
            var bankProperties = bankTotals.GetType().GetProperties();

            WriteHeader(htmlWriter, bankProperties, maxStringLength);

            List<string> BankCompanies = funds.GetBankAccountCompanyNames();
            foreach (string compName in BankCompanies)
            {
                var bankAccounts = funds.GenerateBankAccountStatistics(compName);
                foreach (var acc in bankAccounts)
                {
                    if ((DisplayValueFunds && acc.Value > 0) || !DisplayValueFunds)
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
                                    line += prop.GetValue(acc).ToString().PadRight(maxStringLength);
                                }
                            }
                        }

                        htmlWriter.WriteLine(line);
                    }
                }

                htmlWriter.WriteLine("");
            }

            WriteSeparatorLine(htmlWriter);

            if ((DisplayValueFunds && bankTotals.Value > 0) || !DisplayValueFunds)
            {
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
                            totalAccountsLine += prop.GetValue(bankTotals).ToString().PadRight(maxStringLength);
                        }
                    }
                }

                htmlWriter.WriteLine(totalAccountsLine);
                htmlWriter.WriteLine("");
                WriteSeparatorLine(htmlWriter);
            }

            htmlWriter.WriteLine(" ");
            htmlWriter.WriteLine("Portfolio Totals");
            htmlWriter.WriteLine(" ");

            DailyValuation_Named portfolioTotals = new DailyValuation_Named("Totals", string.Empty, DateTime.Today, SecurityStatistics.TotalValue(DateTime.Today));
            var portfolioProperties = bankTotals.GetType().GetProperties();

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
                        totalLine += prop.GetValue(portfolioTotals).ToString().PadRight(maxStringLength);
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

        private static void WriteHeader(StreamWriter writer, System.Reflection.PropertyInfo[] info, int maxLength)
        {
            string header = string.Empty;

            foreach (var props in info)
            {
                if (props.PropertyType == typeof(string))
                {
                    header += props.Name.PadRight(maxLength);
                }

                if (props.PropertyType == typeof(double))
                {
                    header += props.Name.PadRight(15);
                }
            }

            writer.WriteLine(header);
            writer.WriteLine("");
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
