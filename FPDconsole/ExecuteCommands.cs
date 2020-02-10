using FinancialStructures.ReportingStructures;
using FinancialStructures.Database;
using System.Collections.Generic;
using FinancialStructures.FinanceStructures;
using System;

namespace FPDconsole
{
    public static class ExecuteCommands
    {
        public static void RunCommands(List<TextToken> tokens, Action<string> reportCallback, Action<ErrorReports> displayReports, ErrorReports reports)
        {
            // first we must load the portfolio to edit. Find the text token specifying where to load.
            TextToken filePath = tokens.Find(token => token.TokenType == TextTokenType.FilePath);
            Portfolio portfolio = new Portfolio();
            List<Sector> sectors = new List<Sector>();
            sectors = portfolio.LoadPortfolio(filePath.Value, reports);
            reportCallback($"Successfully loaded portfolio from {filePath.Value}");

            foreach (var token in tokens)
            {
                if (token.TokenType == TextTokenType.Help)
                {
                    DisplayHelp();
                }
                if (token.TokenType == TextTokenType.Download)
                {
                    RunDownloadRoutine(portfolio, sectors, displayReports, reports);
                }
            }

            portfolio.SavePortfolio(sectors, filePath.Value, reports);
            displayReports(reports);
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("I am showing you the help docs.");
        }

        private async static void RunDownloadRoutine(Portfolio portfolio, List<Sector> sectors, Action<ErrorReports> displayReports, ErrorReports reports)
        {
            await DataUpdater.Downloader(portfolio, sectors, displayReports, reports).ConfigureAwait(false);
        }
    }
}
