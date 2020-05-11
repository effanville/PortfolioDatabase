using FinancialStructures.Database;
using FinancialStructures.PortfolioAPI;
using FinancialStructures.Reporting;
using System;
using System.Collections.Generic;

namespace FPDconsole
{
    internal static class ExecuteCommands
    {
        internal static void RunCommands(List<TextToken> tokens,ConsoleStreamWriter writer, LogReporter reportLogger)
        {
            // first we must load the portfolio to edit. Find the text token specifying where to load.
            TextToken filePath = tokens.Find(token => token.TokenType == TextTokenType.FilePath);
            Portfolio portfolio = new Portfolio();
            portfolio.LoadPortfolio(filePath.Value, reportLogger);
            _ = reportLogger.LogUsefulWithStrings("Report", "Loading", $"Successfully loaded portfolio from {filePath.Value}");

            foreach (var token in tokens)
            {
                if (token.TokenType == TextTokenType.Help)
                {
                    DisplayHelp(writer);
                }
                if (token.TokenType == TextTokenType.Download)
                {
                    RunDownloadRoutine(portfolio, reportLogger);
                }
            }

            portfolio.SavePortfolio(filePath.Value, reportLogger);
        }

        internal static void DisplayHelp(ConsoleStreamWriter writer)
        {
            writer.Write("");
            writer.Write("Syntax for query:");
            writer.Write("FPDconsole.exe <<filePath>> <<command>> <<parameters>>");
            writer.Write("");
            writer.Write("filepath   - Absolute path to the database xml file ");
            writer.Write("");
            writer.Write("command    - The specified instruction to carry out");
            writer.Write("");
            writer.Write("Possible Commands:");
            foreach (var command in Enum.GetValues(typeof(CommandType)))
            {
                writer.Write(command.ToString());
            }
            writer.Write("");
            writer.Write("parameters - currently no parameters are implemented.");
            writer.Write("           - all parameters are ignored");
        }

        private static void RunDownloadRoutine(Portfolio portfolio, LogReporter reportLogger)
        {
            PortfolioDataUpdater.Downloader(portfolio, reportLogger).Wait();
        }
    }
}
