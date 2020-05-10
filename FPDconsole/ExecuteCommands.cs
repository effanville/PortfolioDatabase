using FinancialStructures.Database;
using FinancialStructures.PortfolioAPI;
using FinancialStructures.Reporting;
using System;
using System.Collections.Generic;

namespace FPDconsole
{
    internal static class ExecuteCommands
    {
        internal static void RunCommands(List<TextToken> tokens, LogReporter reportLogger)
        {
            // first we must load the portfolio to edit. Find the text token specifying where to load.
            TextToken filePath = tokens.Find(token => token.TokenType == TextTokenType.FilePath);
            Portfolio portfolio = new Portfolio();
            portfolio.LoadPortfolio(filePath.Value, reportLogger);
            reportLogger.LogUsefulWithStrings("Report", "Loading", $"Successfully loaded portfolio from {filePath.Value}");

            foreach (var token in tokens)
            {
                if (token.TokenType == TextTokenType.Help)
                {
                    DisplayHelp();
                }
                if (token.TokenType == TextTokenType.Download)
                {
                    RunDownloadRoutine(portfolio, reportLogger);
                }
            }

            portfolio.SavePortfolio(filePath.Value, reportLogger);
        }

        internal static void DisplayHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("Syntax for query:");
            Console.WriteLine("FPDconsole.exe <<filePath>> <<command>> <<parameters>>");
            Console.WriteLine("");
            Console.WriteLine("filepath   - Absolute path to the database xml file ");
            Console.WriteLine("");
            Console.WriteLine("command    - The specified instruction to carry out");
            Console.WriteLine("");
            Console.WriteLine("Possible Commands:");
            foreach (var command in Enum.GetValues(typeof(CommandType)))
            {
                Console.WriteLine(command.ToString());
            }
            Console.WriteLine("");
            Console.WriteLine("parameters - currently no parameters are implemented.");
            Console.WriteLine("           - all parameters are ignored");
        }

        private static void RunDownloadRoutine(Portfolio portfolio, LogReporter reportLogger)
        {
            PortfolioDataUpdater.Downloader(portfolio, reportLogger).Wait();
        }
    }
}
