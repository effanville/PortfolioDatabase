using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Common.Console;
using Common.Console.Commands;
using Common.Console.Options;
using Common.Structure.Extensions;
using Common.Structure.Reporting;
using FinancialStructures.Database;
using FinancialStructures.Database.Download;
using FinancialStructures.Database.Extensions;
using FinancialStructures.Database.Export.Statistics;
using Common.Structure.ReportWriting;

namespace FPDconsole
{
    internal sealed class DownloadCommand : ICommand
    {
        private readonly IFileSystem fFileSystem;
        private readonly IReportLogger fLogger;
        private readonly CommandOption<string> fFilepathOption;
        private readonly CommandOption<bool> fUpdateStatsOption;

        public string Name => "download";

        /// <inheritdoc/>
        public IList<CommandOption> Options
        {
            get;
        } = new List<CommandOption>();

        /// <inheritdoc/>
        public IList<ICommand> SubCommands
        {
            get;
        } = new List<ICommand>();

        public DownloadCommand(IFileSystem fileSystem, IReportLogger logger)
        {
            fFileSystem = fileSystem;
            fLogger = logger;
            Func<string, bool> fileValidator = filepath => fileSystem.File.Exists(filepath);
            fFilepathOption = new CommandOption<string>("filepath", "The path to the portfolio.", required: true, fileValidator);
            Options.Add(fFilepathOption);
            fUpdateStatsOption = new CommandOption<bool>("updateStats", "Update stats for portfolio.");
            Options.Add(fUpdateStatsOption);
        }

        /// <inheritdoc/>
        public int Execute(IConsole console, string[] args = null)
        {
            IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
            portfolio.LoadPortfolio(fFilepathOption.Value, fFileSystem, fLogger);
            _ = fLogger.LogUseful(ReportType.Information, ReportLocation.Loading, $"Successfully loaded portfolio from {fFilepathOption.Value}");

            _ = PortfolioDataUpdater.Download(Account.All, portfolio, null, fLogger).ConfigureAwait(true);

            if (fUpdateStatsOption.Value)
            {
                string filePath = portfolio.Directory(fFileSystem) + "\\" + DateTime.Today.FileSuitableUKDateString() + portfolio.DatabaseName(fFileSystem) + ".html";
                var settings = PortfolioStatisticsSettings.DefaultSettings();
                PortfolioStatistics stats = new PortfolioStatistics(portfolio, settings, fFileSystem);
                var exportSettings = PortfolioStatisticsExportSettings.DefaultSettings();
                stats.ExportToFile(fFileSystem, filePath, DocumentType.Html, exportSettings, fLogger);
            }

            portfolio.SavePortfolio(fFilepathOption.Value, fFileSystem, fLogger);

            return 0;
        }

        /// <inheritdoc/>
        public bool Validate(IConsole console, string[] args)
        {
            return CommandExtensions.Validate(this, args, console);
        }

        /// <inheritdoc/>
        public void WriteHelp(IConsole console)
        {
            CommandExtensions.WriteHelp(this, console);
        }
    }
}
