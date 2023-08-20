using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Common.Console;
using Common.Console.Commands;
using Common.Console.Options;
using Common.Structure.Reporting;

using FinancialStructures.Database;

namespace FPDconsole
{
    internal sealed class ImportCommand : ICommand
    {
        private readonly IFileSystem _fileSystem;
        private readonly CommandOption<string> _filepathOption;
        private readonly CommandOption<string> _otherDatabaseFilepath;

        public string Name => "import";

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

        public ImportCommand(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            bool fileValidator(string filepath) => fileSystem.File.Exists(filepath);
            _filepathOption = new CommandOption<string>("filepath", "The path to the portfolio.", required: true, fileValidator);
            Options.Add(_filepathOption);
            _otherDatabaseFilepath = new CommandOption<string>("importfilepath", "Filepath for that database to import from.", required: true, fileValidator);
            Options.Add(_otherDatabaseFilepath);
        }

        /// <inheritdoc/>
        public int Execute(IConsole console, string[] args = null) => Execute(console, null, args);

        /// <inheritdoc/>
        public int Execute(IConsole console, IReportLogger logger, string[] args = null)
        {
            IPortfolio portfolio = PortfolioFactory.CreateFromFile(_fileSystem, _filepathOption.Value, logger);
            logger.Log(ReportType.Information, $"{ReportLocation.Loading}", $"Successfully loaded portfolio from {_filepathOption.Value}");

            IPortfolio otherPortfolio = PortfolioFactory.CreateFromFile(_fileSystem, _otherDatabaseFilepath.Value, logger);
            logger.Log(ReportType.Information, $"{ReportLocation.Loading}", $"Successfully loaded portfolio from {_otherDatabaseFilepath.Value}");

            portfolio.ImportValuesFrom(otherPortfolio, logger);

            portfolio.SavePortfolio(_filepathOption.Value, _fileSystem, logger);

            return 0;
        }

        /// <inheritdoc/>
        public bool Validate(IConsole console, string[] args) => Validate(console, null, args);

        /// <inheritdoc/>
        public bool Validate(IConsole console, IReportLogger logger, string[] args) => CommandExtensions.Validate(this, args, console, logger);

        /// <inheritdoc/>
        public void WriteHelp(IConsole console) => CommandExtensions.WriteHelp(this, console);
    }
}
