using System;
using System.Collections.Generic;
using System.Reflection;
using FinancialStructures.Database;
using FinancialStructures.Database.Download;
using FinancialStructures.DataStructures;
using FinancialStructures.FinanceInterfaces;
using FinancialStructures.Statistics;
using FinancialStructures.StatisticStructures;
using FinancialStructures.StatsMakers;
using StructureCommon.Extensions;
using StructureCommon.FileAccess;
using StructureCommon.Reporting;

namespace FPDconsole
{
    internal class ExecuteCommands
    {
        private readonly LogReporter fReporter;
        private readonly ConsoleStreamWriter consoleWriter;
        public ExecuteCommands(LogReporter reporter, ConsoleStreamWriter writer)
        {
            fReporter = reporter;
            consoleWriter = writer;
        }

        internal void RunCommands(List<TextToken> tokens)
        {
            // first we must load the portfolio to edit. Find the text token specifying where to load.
            TextToken filePath = tokens.Find(token => token.TokenType == TextTokenType.FilePath);
            Portfolio portfolio = new Portfolio();
            portfolio.LoadPortfolio(filePath.Value, fReporter);
            _ = fReporter.LogUsefulWithStrings("Report", "Loading", $"Successfully loaded portfolio from {filePath.Value}");

            foreach (TextToken token in tokens)
            {
                if (token.TokenType == TextTokenType.Help)
                {
                    DisplayHelp();
                }
                if (token.TokenType == TextTokenType.Download)
                {
                    RunDownloadRoutine(portfolio);
                }
                if (token.TokenType == TextTokenType.DownloadUpdateStats)
                {
                    RunDownloadRoutine(portfolio);
                    RunUpdateStatsRoutine(portfolio);
                }
            }

            portfolio.SavePortfolio(filePath.Value, fReporter);
        }

        internal void DisplayHelp()
        {
            consoleWriter.Write("");
            consoleWriter.Write("Syntax for query:");
            consoleWriter.Write("FPDconsole.exe <<filePath>> <<command>> <<parameters>>");
            consoleWriter.Write("");
            consoleWriter.Write("filepath   - Absolute path to the database xml file ");
            consoleWriter.Write("");
            consoleWriter.Write("command    - The specified instruction to carry out");
            consoleWriter.Write("");
            consoleWriter.Write("Possible Commands:");
            foreach (object command in Enum.GetValues(typeof(CommandType)))
            {
                consoleWriter.Write(command.ToString());
            }
            consoleWriter.Write("");
            consoleWriter.Write("parameters - currently no parameters are implemented.");
            consoleWriter.Write("           - all parameters are ignored");
        }

        private void RunDownloadRoutine(Portfolio portfolio)
        {
            PortfolioDataUpdater.Download(Account.All, portfolio, null, fReporter).Wait();
        }

        private void RunUpdateStatsRoutine(Portfolio portfolio)
        {
            string filePath = portfolio.Directory + "\\" + DateTime.Today.FileSuitableUKDateString() + portfolio.DatabaseName + ".html";
            UserOptions options = new UserOptions();
            DayValue_Named BankNames = new DayValue_Named();
            PropertyInfo[] props = BankNames.GetType().GetProperties();
            foreach (PropertyInfo name in props)
            {
                options.BankAccDataToExport.AddRange(AccountStatisticsHelpers.DefaultBankAccountStats());
            }


            foreach (var name in AccountStatisticsHelpers.AllStatistics())
            {
                options.SecurityDataToExport.Add(name);
            }
            PortfolioStatistics stats = new PortfolioStatistics(portfolio, options);
            stats.ExportToFile(filePath, ExportType.Html, options, fReporter);
        }
    }
}
