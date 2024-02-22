using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console;
using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Persistence;

namespace Effanville.FPD.Console
{
    internal sealed class ImportCommand : ICommand
    {
        private readonly IFileSystem _fileSystem;
        private readonly CommandOption<string> _filepathOption;
        private readonly CommandOption<string> _otherDatabaseFilepath;

        public string Name => "import";

        /// <inheritdoc/>
        public IList<CommandOption> Options { get; } = new List<CommandOption>();

        /// <inheritdoc/>
        public IList<ICommand> SubCommands { get; } = new List<ICommand>();

        public ImportCommand(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _filepathOption =
                new CommandOption<string>("filepath", "The path to the portfolio.", required: true, FileValidator);
            Options.Add(_filepathOption);
            _otherDatabaseFilepath = new CommandOption<string>("importfilepath",
                "Filepath for that database to import from.", required: true, FileValidator);
            Options.Add(_otherDatabaseFilepath);
            return;
            bool FileValidator(string filepath) => fileSystem.File.Exists(filepath);
        }

        /// <inheritdoc/>
        public int Execute(IConsole console, string[] args = null) => Execute(console, null, args);

        /// <inheritdoc/>
        public int Execute(IConsole console, IReportLogger logger, string[] args = null)
        {
            var portfolioPersistence = new PortfolioPersistence();
            var portfolioOptions = PortfolioPersistence.CreateOptions(_filepathOption.Value, _fileSystem);
            IPortfolio portfolio = portfolioPersistence.Load(portfolioOptions, logger);
            logger.Log(ReportType.Information, $"{ReportLocation.Loading}",
                $"Successfully loaded portfolio from {_filepathOption.Value}");

            var otherPortfolioOptions = PortfolioPersistence.CreateOptions(_otherDatabaseFilepath.Value, _fileSystem);
            IPortfolio otherPortfolio = portfolioPersistence.Load(otherPortfolioOptions, logger);
            logger.Log(ReportType.Information, $"{ReportLocation.Loading}",
                $"Successfully loaded portfolio from {_otherDatabaseFilepath.Value}");

            portfolio.ImportValuesFrom(otherPortfolio, logger);

            var xmlPersistence = new XmlPortfolioPersistence();
            xmlPersistence.Save(portfolio, new XmlFilePersistenceOptions(_filepathOption.Value, _fileSystem), logger);
            return 0;
        }

        /// <inheritdoc/>
        public bool Validate(IConsole console, string[] args) => Validate(console, null, args);

        /// <inheritdoc/>
        public bool Validate(IConsole console, IReportLogger logger, string[] args) =>
            this.Validate(args, console, logger);

        /// <inheritdoc/>
        public void WriteHelp(IConsole console) => CommandExtensions.WriteHelp(this, console);
    }
}