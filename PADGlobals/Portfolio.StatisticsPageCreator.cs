using BankAccountHelperFunctions;
using FinancialStructures.DataStructures;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using GUIAccessorFunctions;
using SecurityStatisticsFunctions;
using System;
using System.Collections.Generic;
using System.IO;

namespace PortfolioStatsCreatorHelper
{
    public static class StringExtensions
    {
        public static string WithMaxLength(this string value, int maxLength)
        {
            if (value == null)
            {
                return null;
            }

            return value.Substring(0, Math.Min(value.Length, maxLength));
        }
    }

    public static class PortfolioStatsCreators
    {
        public static void WriteSectorAnalysis(StreamWriter writer, Portfolio funds, System.Reflection.PropertyInfo[] info, bool DisplayValueFunds, int maxNameLength, int maxCompanyLength, int maxNumLength)
        {
            writer.WriteLine("<h2>Analysis By Sector</h2>");

            WriteHeader(writer, info, maxNameLength, maxCompanyLength, maxNumLength);

            List<string> sectors = funds.GetSecuritiesSectors();
            foreach (string sectorName in sectors)
            {
                List<SecurityStatsHolder> valuesToWrite = new List<SecurityStatsHolder>();
                valuesToWrite.Add(DatabaseAccessor.GenerateSectorFundsStatistics(sectorName));
                valuesToWrite.Add(DatabaseAccessor.GenerateBenchMarkStatistics(sectorName));
                int linesWritten = 0;
                foreach (var value in valuesToWrite)
                {
                    if ((DisplayValueFunds && value.LatestVal > 0) || !DisplayValueFunds)
                    {
                        string line = string.Empty;
                        foreach (var props in info)
                        {
                            if (Double.TryParse(props.GetValue(value).ToString(), out double result))
                            {
                                line += result.ToString().PadLeft(maxNumLength);
                            }
                            else
                            {
                                if (props.Name == "Name")
                                {
                                    line += props.GetValue(value).ToString().WithMaxLength(maxNameLength - 2).PadRight(maxNameLength);
                                }
                                else
                                {
                                    line += props.GetValue(value).ToString().WithMaxLength(maxCompanyLength - 2).PadRight(maxCompanyLength);
                                }
                            }
                        }
                        linesWritten++;
                        writer.WriteLine(line);
                    }
                }

                if (linesWritten > 0)
                {
                    writer.WriteLine("");
                }
            }
        }

        public static bool CreateHTMLPage(Portfolio funds, List<string> toExport, string filepath, bool DisplayValueFunds)
        {
            int maxNameLength = Math.Min(25, DatabaseAccessor.LongestName()+2);
            int maxCompanyLength = Math.Min(25, DatabaseAccessor.LongestCompany() + 2);
            int maxNumLength = 10;
            int length = maxNameLength + maxCompanyLength + 8 * maxNumLength;
            StreamWriter htmlWriter = new StreamWriter(filepath);
            CreateHTMLHeader(htmlWriter, length);

            htmlWriter.WriteLine("<h2>Funds Data</h2>");
            
            var totals = DatabaseAccessor.GeneratePortfolioStatistics();
            var properties = totals.GetType().GetProperties();

            WriteHeader(htmlWriter, properties, maxNameLength, maxCompanyLength, maxNumLength);

            List<string> companies = funds.GetSecuritiesCompanyNames();
            foreach (string compName in companies)
            {
                var securities = DatabaseAccessor.GenerateCompanyFundsStatistics(compName);
                int linesWritten = 0;
                foreach (var sec in securities)
                {
                    if ((DisplayValueFunds && sec.LatestVal > 0) || !DisplayValueFunds)
                    {
                        string line = string.Empty;
                        foreach (var props in properties)
                        {
                            if (Double.TryParse(props.GetValue(sec).ToString(), out double result))
                            {
                                line += result.ToString().PadLeft(maxNumLength);
                            }
                            else
                            {
                                if (props.Name == "Name")
                                {
                                    line += props.GetValue(sec).ToString().WithMaxLength(maxNameLength - 2).PadRight(maxNameLength);
                                }
                                else
                                {
                                    line += props.GetValue(sec).ToString().WithMaxLength(maxCompanyLength - 2).PadRight(maxCompanyLength);
                                }
                            }
                        }

                        htmlWriter.WriteLine(line);
                        linesWritten += 1;
                    }
                }

                if (linesWritten > 0)
                {
                    htmlWriter.WriteLine("");
                }
            }

            if ((DisplayValueFunds && totals.LatestVal > 0) || !DisplayValueFunds)
            {
                htmlWriter.WriteLine("");
                string fundTotalLine = string.Empty;
                foreach (var props in properties)
                {
                    if (Double.TryParse(props.GetValue(totals).ToString(), out double result))
                    {
                        fundTotalLine += result.ToString().PadLeft(maxNumLength);
                    }
                    else
                    {
                        if (props.Name == "Name")
                        {
                            fundTotalLine += props.GetValue(totals).ToString().WithMaxLength(maxNameLength - 2).PadRight(maxNameLength);
                        }
                        else
                        {
                            fundTotalLine += props.GetValue(totals).ToString().WithMaxLength(maxCompanyLength - 2).PadRight(maxCompanyLength);
                        }
                    }
                }

                htmlWriter.WriteLine(fundTotalLine);
                htmlWriter.WriteLine("");
            }

            WriteSeparatorLine(htmlWriter, length);

            htmlWriter.WriteLine("<h2>Bank Accounts Data</h2>");

            DailyValuation_Named bankTotals = new DailyValuation_Named("Totals", string.Empty, DateTime.Today, BankAccountEditor.AllBankAccountValue(DateTime.Today));
            var bankProperties = bankTotals.GetType().GetProperties();

            WriteHeader(htmlWriter, bankProperties, maxNameLength, maxCompanyLength, maxNumLength);

            List<string> BankCompanies = funds.GetBankAccountCompanyNames();
            foreach (string compName in BankCompanies)
            {
                var bankAccounts = funds.GenerateBankAccountStatistics(compName);
                int linesWritten = 0;
                foreach (var acc in bankAccounts)
                {
                    if ((DisplayValueFunds && acc.Value > 0) || !DisplayValueFunds)
                    {
                        string line = string.Empty;
                        foreach (var prop in bankProperties)
                        {
                            if (Double.TryParse(prop.GetValue(acc).ToString(), out double result))
                            {
                                line += result.ToString().PadLeft(maxNumLength);
                            }
                            else
                            {
                                if (prop.PropertyType == typeof(string))
                                {
                                    if (prop.Name == "Name")
                                    {
                                        line += prop.GetValue(acc).ToString().WithMaxLength(maxNameLength - 2).PadRight(maxNameLength);
                                    }
                                    else
                                    {
                                        line += prop.GetValue(acc).ToString().WithMaxLength(maxCompanyLength - 2).PadRight(maxCompanyLength);
                                    }
                                }
                            }
                        }
                        linesWritten++;
                        htmlWriter.WriteLine(line);
                    }
                }

                if (linesWritten > 0)
                {
                    htmlWriter.WriteLine("");
                }
            }

            WriteSeparatorLine(htmlWriter, length);

            if ((DisplayValueFunds && bankTotals.Value > 0) || !DisplayValueFunds)
            {
                string totalAccountsLine = string.Empty;
                foreach (var prop in bankProperties)
                {
                    if (Double.TryParse(prop.GetValue(bankTotals).ToString(), out double result))
                    {
                        totalAccountsLine += result.ToString().PadLeft(maxNumLength);
                    }
                    else
                    {
                        if (prop.PropertyType == typeof(string))
                        {
                            if (prop.Name == "Name")
                            {
                                totalAccountsLine += prop.GetValue(bankTotals).ToString().WithMaxLength(maxNameLength - 2).PadRight(maxNameLength);
                            }
                            else
                            {
                                totalAccountsLine += prop.GetValue(bankTotals).ToString().WithMaxLength(maxCompanyLength - 2).PadRight(maxCompanyLength);
                            }
                        }
                    }
                }

                htmlWriter.WriteLine(totalAccountsLine);
                htmlWriter.WriteLine("");
                WriteSeparatorLine(htmlWriter, length);
            }

            htmlWriter.WriteLine(" ");

            DailyValuation_Named portfolioTotals = new DailyValuation_Named("Total", "Portfolio", DateTime.Today, SecurityStatistics.TotalValue(DateTime.Today));
            var portfolioProperties = bankTotals.GetType().GetProperties();

            string totalLine = string.Empty;
            foreach (var prop in portfolioProperties)
            {
                if (Double.TryParse(prop.GetValue(portfolioTotals).ToString(), out double result))
                {
                    totalLine += result.ToString().PadLeft(maxNumLength);
                }
                else
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        if (prop.Name == "Name")
                        {
                            totalLine += prop.GetValue(portfolioTotals).ToString().WithMaxLength(maxNameLength - 2).PadRight(maxNameLength);
                        }
                        else
                        {
                            totalLine += prop.GetValue(portfolioTotals).ToString().WithMaxLength(maxCompanyLength - 2).PadRight(maxCompanyLength);
                        }
                    }
                }
            }

            htmlWriter.WriteLine(totalLine);

            htmlWriter.WriteLine("");
            WriteSeparatorLine(htmlWriter, length);

            WriteSectorAnalysis(htmlWriter, funds, properties, DisplayValueFunds, maxNameLength, maxCompanyLength, maxNumLength);

            CreateHTMLFooter(htmlWriter, length);
            htmlWriter.Close();
            return true;
        }

        private static void WriteSeparatorLine(StreamWriter writer, int length)
        {
            string toWrite= string.Empty;
            int i = 0;
            while(i < length)
            {
                toWrite += "=";
                i++;
            }
            writer.WriteLine(toWrite);
        }

        private static void WriteHeader(StreamWriter writer, System.Reflection.PropertyInfo[] info, int maxNameLength, int maxCompanyLength, int maxNumLength)
        {
            string header = string.Empty;

            foreach (var props in info)
            {
                if (props.PropertyType == typeof(string))
                {
                    if (props.Name == "Name")
                    {
                        header += "<b>" + props.Name.WithMaxLength(maxNameLength - 2).PadRight(maxNameLength) + "</b>";
                    }
                    else
                    {
                        header += "<b>" + props.Name.WithMaxLength(maxCompanyLength - 2).PadRight(maxCompanyLength) + "</b>";
                    }
                    
                }

                if (props.PropertyType == typeof(double))
                {
                    header += "<b>" + props.Name.WithMaxLength(maxNumLength - 2).PadLeft(maxNumLength) + "</b>";
                }
            }

            writer.WriteLine(header);
            writer.WriteLine("");
        }

        private static void CreateHTMLHeader(StreamWriter writer, int length)
        {
            writer.WriteLine("<!DOCTYPE html>");
            writer.WriteLine("<META HTTP-EQUIV=\"Content - Type\" CONTENT=\"text / html; charset = UTF - 8\">");
            writer.WriteLine("<html>");
            writer.WriteLine("<head>");
            writer.WriteLine($"<title> Statement for funds as of {DateTime.Today.ToShortDateString()}</title>");
            writer.WriteLine("</head>");
            writer.WriteLine("<body>");
            writer.WriteLine("<pre style='color:#1f1c1b;background-color:#ffffff;'><span class=\"inner - pre\" style=\"font - size: 13px\">");

            writer.WriteLine("<h1>Portfolio Statement</h1>");
            writer.WriteLine($"on the date {DateTime.Today.ToShortDateString()}.");
            writer.WriteLine($"Produced by Finance Portfolio Database v{GlobalHeldData.GlobalData.versionNumber}");
            writer.WriteLine("as written by Matthew Egginton");

            WriteSeparatorLine(writer, length);
        }

        private static void CreateHTMLFooter(StreamWriter writer, int length)
        {
            WriteSeparatorLine(writer, length);
            writer.WriteLine("</span></pre>");
            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
        }
    }
}
