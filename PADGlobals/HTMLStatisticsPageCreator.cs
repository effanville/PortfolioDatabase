using FinancialStructures.DataStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.Database;
using System;
using System.Collections.Generic;
using System.IO;
using FinancialStructures.FinanceStructures;

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

    public enum ExportType
    {
        HTML,
        CSV
    }

    public static class PortfolioStatsCreators
    {

        private static void WriteSpacing(StreamWriter writer, bool spacing)
        {
            if (spacing)
            {
                writer.WriteLine("");
            }
        }

        private static void WriteSectorAnalysis(StreamWriter writer, Portfolio portfolio, List<Sector> sectors, System.Reflection.PropertyInfo[] info, UserOptions options, int maxNameLength, int maxCompanyLength, int maxNumLength)
        {
            writer.WriteLine("<h2>Analysis By Sector</h2>");

            WriteHeader(writer, info, options.SecurityDataToExport, maxNameLength, maxCompanyLength, maxNumLength);

            List<string> sectorNames = portfolio.AllSecuritiesSectors();
            foreach (string sectorName in sectorNames)
            {
                List<SecurityStatsHolder> valuesToWrite = new List<SecurityStatsHolder>();
                valuesToWrite.Add(portfolio.GenerateSectorFundsStatistics(sectors, sectorName));
                valuesToWrite.Add(portfolio.GenerateBenchMarkStatistics(sectors, sectorName));
                int linesWritten = 0;
                foreach (var value in valuesToWrite)
                {
                    if ((options.DisplayValueFunds && value.LatestVal > 0) || !options.DisplayValueFunds)
                    {
                        string line = string.Empty;
                        foreach (var props in info)
                        {
                            if (options.SecurityDataToExport.Contains(props.Name))
                            {
                                if (double.TryParse(props.GetValue(value).ToString(), out double result))
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
                        }
                        linesWritten++;
                        writer.WriteLine(line);
                    }
                }

                if (linesWritten > 0)
                {
                    WriteSpacing(writer, options.Spacing);
                }
            }
        }

        public static bool CreateHTMLPageCustom(Portfolio portfolio, List<Sector> sectors, string filepath, UserOptions options)
        {
            return CreateHTMLPage(portfolio, sectors, filepath, options);
        }

        private static bool CreateHTMLPage(Portfolio portfolio, List<Sector> sectors, string filepath, UserOptions options)
        {
            int maxNameLength = Math.Min(25, portfolio.LongestName()+2);
            int maxCompanyLength = Math.Min(25, portfolio.LongestCompany() + 2);
            int maxNumLength = 10;
            int length = maxNameLength + maxCompanyLength + 8 * maxNumLength;
            StreamWriter htmlWriter = new StreamWriter(filepath);
            CreateHTMLHeader(htmlWriter, length);

            htmlWriter.WriteLine("<h2>Funds Data</h2>");
            
            var totals = portfolio.GeneratePortfolioStatistics();
            var properties = totals.GetType().GetProperties();

            WriteHeader(htmlWriter, properties,options.SecurityDataToExport, maxNameLength, maxCompanyLength, maxNumLength);

            List<string> companies = portfolio.GetSecuritiesCompanyNames();
            foreach (string compName in companies)
            {
                var securities = portfolio.GenerateCompanyFundsStatistics(compName);
                int linesWritten = 0;
                foreach (var sec in securities)
                {
                    if ((options.DisplayValueFunds && sec.LatestVal > 0) || !options.DisplayValueFunds)
                    {
                        string line = string.Empty;
                        foreach (var props in properties)
                        {
                            if (options.SecurityDataToExport.Contains(props.Name))
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
                        }

                        htmlWriter.WriteLine(line);
                        linesWritten += 1;
                    }
                }

                if (linesWritten > 0)
                {
                    WriteSpacing(htmlWriter, options.Spacing);
                }
            }

            if ((options.DisplayValueFunds && totals.LatestVal > 0) || !options.DisplayValueFunds)
            {
                htmlWriter.WriteLine("");
                string fundTotalLine = string.Empty;
                foreach (var props in properties)
                {
                    if (options.SecurityDataToExport.Contains(props.Name))
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
                }

                htmlWriter.WriteLine(fundTotalLine);
                WriteSpacing(htmlWriter, options.Spacing);
            }

            WriteSeparatorLine(htmlWriter, length);

            htmlWriter.WriteLine("<h2>Bank Accounts Data</h2>");

            DailyValuation_Named bankTotals = new DailyValuation_Named("Totals", string.Empty, DateTime.Today, portfolio.AllBankAccountsValue(DateTime.Today));
            var bankProperties = bankTotals.GetType().GetProperties();

            WriteHeader(htmlWriter, bankProperties, options.BankAccDataToExport, maxNameLength, maxCompanyLength, maxNumLength);

            List<string> BankCompanies = portfolio.GetBankAccountCompanyNames();
            foreach (string compName in BankCompanies)
            {
                var bankAccounts = portfolio.GenerateBankAccountStatistics(compName);
                int linesWritten = 0;
                foreach (var acc in bankAccounts)
                {
                    if ((options.DisplayValueFunds && acc.Value > 0) || !options.DisplayValueFunds)
                    {
                        string line = string.Empty;
                        foreach (var prop in bankProperties)
                        {
                            if (options.BankAccDataToExport.Contains(prop.Name))
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
                        }
                        linesWritten++;
                        htmlWriter.WriteLine(line);
                    }
                }

                if (linesWritten > 0)
                {
                    WriteSpacing(htmlWriter, options.Spacing);
                }
            }

            WriteSeparatorLine(htmlWriter, length);

            if ((options.DisplayValueFunds && bankTotals.Value > 0) || !options.DisplayValueFunds)
            {
                string totalAccountsLine = string.Empty;
                foreach (var prop in bankProperties)
                {
                    if (options.BankAccDataToExport.Contains(prop.Name))
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
                }

                htmlWriter.WriteLine(totalAccountsLine);
                WriteSpacing(htmlWriter, options.Spacing);
                WriteSeparatorLine(htmlWriter, length);
            }

            WriteSpacing(htmlWriter, options.Spacing);

            DailyValuation_Named portfolioTotals = new DailyValuation_Named("Total", "Portfolio", DateTime.Today, portfolio.Value(DateTime.Today));
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

            WriteSpacing(htmlWriter, options.Spacing);
            WriteSeparatorLine(htmlWriter, length);

            WriteSectorAnalysis(htmlWriter, portfolio, sectors, properties, options, maxNameLength, maxCompanyLength, maxNumLength);

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

        private static void WriteHeader(StreamWriter writer, System.Reflection.PropertyInfo[] info, List<string> names, int maxNameLength, int maxCompanyLength, int maxNumLength)
        {
            string header = string.Empty;

            foreach (var props in info)
            {
                if (names.Contains(props.Name))
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
                        header += props.Name.WithMaxLength(maxNumLength - 2).PadLeft(maxNumLength) + ",";
                    }
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
