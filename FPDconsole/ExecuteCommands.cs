using FinancialStructures.Database;
using FinancialStructures.DataStructures;
using FinancialStructures.PortfolioAPI;
using StructureCommon.Reporting;
using FinancialStructures.StatisticStructures;
using FinancialStructures.StatsMakers;
using StructureCommon.Extensions;
using System;
using System.Collections.Generic;

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

            foreach (var token in tokens)
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
            foreach (var command in Enum.GetValues(typeof(CommandType)))
            {
                consoleWriter.Write(command.ToString());
            }
            consoleWriter.Write("");
            consoleWriter.Write("parameters - currently no parameters are implemented.");
            consoleWriter.Write("           - all parameters are ignored");
        }

        private void RunDownloadRoutine(Portfolio portfolio)
        {
            PortfolioDataUpdater.Downloader(portfolio, fReporter).Wait();
        }

        private void RunUpdateStatsRoutine(Portfolio portfolio)
        {
            var filePath = portfolio.Directory + "\\" + DateTime.Today.FileSuitableUKDateString() + portfolio.DatabaseName + ".html";
            UserOptions options = new UserOptions();
            var BankNames = new DayValue_Named();
            var props = BankNames.GetType().GetProperties();
            foreach (var name in props)
            {
                options.BankAccDataToExport.Add(name.Name);
            }

            var totals = new SecurityStatistics();
            var properties = totals.GetType().GetProperties();
            foreach (var name in properties)
            {
                options.SecurityDataToExport.Add(name.Name);
            }

            _ = PortfolioStatsCreators.CreateHTMLPageCustom(portfolio, filePath, options);
        }
    }
}
