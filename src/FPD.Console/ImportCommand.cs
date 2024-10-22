using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.Structure.Reporting.LogAspect;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Persistence;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Effanville.FPD.Console
{
    internal sealed class ImportCommand : ICommand, ILogInterceptable
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<ImportCommand> _logger;
        private readonly IReportLogger _reportLogger;
        private readonly CommandOption<string> _filepathOption;
        private readonly CommandOption<string> _otherDatabaseFilepath;

        public string Name => "import";

        public ILogger Logger => _logger;

        /// <inheritdoc/>
        public IList<CommandOption> Options { get; } = new List<CommandOption>();

        /// <inheritdoc/>
        public IList<ICommand> SubCommands { get; } = new List<ICommand>();

        public ImportCommand(IFileSystem fileSystem, ILogger<ImportCommand> logger, IReportLogger reportLogger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _reportLogger = reportLogger;
            _filepathOption =
                new CommandOption<string>(
                    "filepath", 
                    "The path to the portfolio.",
                    required: true,
                    FileValidator);
            Options.Add(_filepathOption);
            _otherDatabaseFilepath = new CommandOption<string>(
                "importfilepath",
                "Filepath for that database to import from.", 
                required: true, 
                FileValidator);
            Options.Add(_otherDatabaseFilepath);
            return;
            bool FileValidator(string filepath) => fileSystem.File.Exists(filepath);
        }

        /// <inheritdoc/>
        [LogIntercept]
        public int Execute(IConfiguration config)
        {
            var portfolioPersistence = new PortfolioPersistence();
            var portfolioOptions = PortfolioPersistence.CreateOptions(_filepathOption.Value, _fileSystem);
            IPortfolio portfolio = portfolioPersistence.Load(portfolioOptions, _reportLogger);
            _logger.Log(LogLevel.Information, $"Successfully loaded portfolio from {_filepathOption.Value}");

            var otherPortfolioOptions = PortfolioPersistence.CreateOptions(_otherDatabaseFilepath.Value, _fileSystem);
            IPortfolio otherPortfolio = portfolioPersistence.Load(otherPortfolioOptions, _reportLogger);
            _logger.Log(LogLevel.Information, $"Successfully loaded portfolio from {_otherDatabaseFilepath.Value}");

            portfolio.ImportValuesFrom(otherPortfolio, _reportLogger);

            var xmlPersistence = new XmlPortfolioPersistence();
            xmlPersistence.Save(portfolio, new XmlFilePersistenceOptions(_filepathOption.Value, _fileSystem), _reportLogger);
            return 0;
        }

        /// <inheritdoc/>
        [LogIntercept]
        public bool Validate(IConfiguration config) => this.Validate(config, _logger);

        /// <inheritdoc/>
        [LogIntercept]
        public void WriteHelp() => this.WriteHelp(_logger);
    }
}