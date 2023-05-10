using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Common.Console;
using Common.Console.Commands;
using Common.Console.Options;

using Common.Structure.Extensions;
using Common.Structure.Reporting;

using FinancialStructures.Database;
using FinancialStructures.Database.Extensions;
using FinancialStructures.Database.Export.Statistics;
using Common.Structure.ReportWriting;

namespace FPDconsole
{
    internal sealed class StatisticsCommand : ICommand
    {
        readonly IFileSystem _fileSystem;
        readonly CommandOption<string> _filepathOption;
        readonly CommandOption<string> _outputPathOption;
        readonly CommandOption<DocumentType> _fileTypeOption;

        /// <inheritdoc/>
        public string Name => "stats";

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

        public StatisticsCommand(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            bool fileValidator(string filepath) => fileSystem.File.Exists(filepath);
            _filepathOption = new CommandOption<string>("filepath", "The path to the portfolio.", required: true, fileValidator);
            Options.Add(_filepathOption);
            _outputPathOption = new CommandOption<string>("outputPath", "Path for the statistics file.");
            Options.Add(_outputPathOption);
            _fileTypeOption = new CommandOption<DocumentType>("docType", "The type of the stats to output.", required: false);
            Options.Add(_fileTypeOption);
        }

        /// <inheritdoc/>
        public int Execute(IConsole console, string[] args = null)
        {
            return Execute(console, null, args);
        }

        /// <inheritdoc/>
        public int Execute(IConsole console, IReportLogger logger, string[] args = null)
        {
            IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
            portfolio.LoadPortfolio(_filepathOption.Value, _fileSystem, logger);
            logger.Log(ReportType.Information, "Loading", $"Successfully loaded portfolio from {_filepathOption.Value}");

            DocumentType docType = _fileTypeOption.Value;

            string filePath = _outputPathOption.Value ??
                _fileSystem.Path.Combine(portfolio.Directory(_fileSystem),
                $"{DateTime.Today.FileSuitableUKDateString()}{portfolio.DatabaseName(_fileSystem)}.{docType}");

            var settings = PortfolioStatisticsSettings.DefaultSettings();
            PortfolioStatistics stats = new PortfolioStatistics(portfolio, settings, _fileSystem);
            var exportSettings = PortfolioStatisticsExportSettings.DefaultSettings();
            stats.ExportToFile(_fileSystem, filePath, docType, exportSettings, logger);
            logger.Log(ReportType.Information, "StatisticsGeneration", $"Successfully generated statistics page {filePath}");

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
