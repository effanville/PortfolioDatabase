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

using FinancialStructures.Persistence;

namespace FPDconsole
{
    internal sealed class DownloadCommand : ICommand
    {
        private readonly IFileSystem _fileSystem;
        private readonly CommandOption<string> _filepathOption;
        private readonly CommandOption<bool> _updateStatsOption;

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

        public DownloadCommand(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            Func<string, bool> fileValidator = filepath => fileSystem.File.Exists(filepath);
            _filepathOption = new CommandOption<string>("filepath", "The path to the portfolio.", required: true, fileValidator);
            Options.Add(_filepathOption);
            _updateStatsOption = new CommandOption<bool>("updateStats", "Update stats for portfolio.");
            Options.Add(_updateStatsOption);
        }

        /// <inheritdoc/>
        public int Execute(IConsole console, string[] args = null)
        {
            return Execute(console, null, args);
        }

        /// <inheritdoc/>
        public int Execute(IConsole console, IReportLogger logger, string[] args = null)
        {
            IPortfolio portfolio = PortfolioFactory.CreateFromFile(_fileSystem, _filepathOption.Value, logger);
            logger.Log(ReportType.Information, $"{ReportLocation.Loading}", $"Successfully loaded portfolio from {_filepathOption.Value}");

            PortfolioDataUpdater.Download(Account.All, portfolio, null, logger).Wait();

            if (_updateStatsOption.Value)
            {
                string directory = _fileSystem.Path.GetDirectoryName(_filepathOption.Value);
                string filePath = _fileSystem.Path.Combine(
                    directory,
                    $"{DateTime.Today.FileSuitableUKDateString()}{portfolio.Name}.html");
                var settings = PortfolioStatisticsSettings.DefaultSettings();
                PortfolioStatistics stats = new PortfolioStatistics(portfolio, settings, _fileSystem);
                var exportSettings = PortfolioStatisticsExportSettings.DefaultSettings();
                stats.ExportToFile(_fileSystem, filePath, DocumentType.Html, exportSettings, logger);
            }

            var xmlPersistence = new XmlPortfolioPersistence();
            xmlPersistence.Save(portfolio, new XmlFilePersistenceOptions(_filepathOption.Value, _fileSystem), logger);
            return 0;
        }

        /// <inheritdoc/>
        public bool Validate(IConsole console, string[] args)
        {
            return Validate(console, null, args);
        }

        /// <inheritdoc/>
        public bool Validate(IConsole console, IReportLogger logger, string[] args)
        {
            return CommandExtensions.Validate(this, args, console, logger);
        }

        /// <inheritdoc/>
        public void WriteHelp(IConsole console)
        {
            CommandExtensions.WriteHelp(this, console);
        }
    }
}
