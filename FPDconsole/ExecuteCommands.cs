using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using System;
using System.Collections.Generic;

namespace FPDconsole
{
    internal static class ExecuteCommands
    {
        internal static void RunCommands(List<TextToken> tokens, Action<string, string, string> reportLogger)
        {
            // first we must load the portfolio to edit. Find the text token specifying where to load.
            TextToken filePath = tokens.Find(token => token.TokenType == TextTokenType.FilePath);
            Portfolio portfolio = new Portfolio();
            List<Sector> sectors = new List<Sector>();
            sectors = portfolio.LoadPortfolio(filePath.Value, reportLogger);
            reportLogger("Report", "Loading", $"Successfully loaded portfolio from {filePath.Value}");

            foreach (var token in tokens)
            {
                if (token.TokenType == TextTokenType.Help)
                {
                    DisplayHelp();
                }
                if (token.TokenType == TextTokenType.Download)
                {
                    RunDownloadRoutine(portfolio, sectors, reportLogger);
                }
            }

            portfolio.SavePortfolio(sectors, filePath.Value, reportLogger);
        }

        internal static void DisplayHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("Possible Commands:");
            foreach (var command in Enum.GetValues(typeof(CommandType)))
            {
                Console.WriteLine(command.ToString());
            }

            Console.WriteLine("");
            Console.WriteLine("Command Syntax:");
            Console.WriteLine("FPDconsole.exe <<filePath>> <<command>> <<parameters>>");
        }

        private static void RunDownloadRoutine(Portfolio portfolio, List<Sector> sectors, Action<string, string, string> reportLogger)
        {
            DataUpdater.Downloader(portfolio, sectors, reportLogger).Wait();
        }
    }
}
