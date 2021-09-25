using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using FinancialStructures.Database;
using FinancialStructures.Database.Download;
using Common.Structure.Extensions;
using Common.Structure.FileAccess;
using Common.Structure.Reporting;
using FinancialStructures.DataExporters.Statistics;

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
            var fileSystem = new FileSystem();
            // first we must load the portfolio to edit. Find the text token specifying where to load.
            TextToken filePath = tokens.Find(token => token.TokenType == TextTokenType.FilePath);
            IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
            portfolio.LoadPortfolio(filePath.Value, fileSystem, fReporter);
            _ = fReporter.LogUseful(ReportType.Information, ReportLocation.Loading, $"Successfully loaded portfolio from {filePath.Value}");

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
                    RunUpdateStatsRoutine(portfolio, fileSystem);
                }
            }

            portfolio.SavePortfolio(filePath.Value, fileSystem, fReporter);
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

        private void RunDownloadRoutine(IPortfolio portfolio)
        {
            PortfolioDataUpdater.Download(Account.All, portfolio, null, fReporter).Wait();
        }

        private void RunUpdateStatsRoutine(IPortfolio portfolio, IFileSystem fileSystem)
        {
            string filePath = portfolio.Directory(fileSystem) + "\\" + DateTime.Today.FileSuitableUKDateString() + portfolio.DatabaseName(fileSystem) + ".html";
            PortfolioStatistics stats = new PortfolioStatistics(portfolio, PortfolioStatisticsSettings.DefaultSettings(), fileSystem);
            stats.ExportToFile(fileSystem, filePath, ExportType.Html, PortfolioStatisticsExportSettings.DefaultSettings(), fReporter);
        }
    }
}
