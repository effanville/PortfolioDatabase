using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.Common.ReportWriting.Documents;
using Effanville.Common.Structure.Extensions;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.Structure.Reporting.LogAspect;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Export.Statistics;
using Effanville.FinancialStructures.Persistence;
using Effanville.FPD.Console.Utilities.Mail;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Effanville.FPD.Console
{
    internal sealed class StatisticsCommand : ICommand, ILogInterceptable
    {
        readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly IReportLogger _reportLogger;
        private readonly IMailSender _mailSender;
        private readonly IPersistence<IPortfolio> _persistence;
        readonly CommandOption<string> _filepathOption;
        readonly CommandOption<string> _outputPathOption;
        readonly CommandOption<DocumentType> _fileTypeOption;
        private readonly CommandOption<string> _mailRecipientOption;

        /// <inheritdoc/>
        public string Name => "stats";

        public ILogger Logger => _logger;

        /// <inheritdoc/>
        public IList<CommandOption> Options { get; } = new List<CommandOption>();

        /// <inheritdoc/>
        public IList<ICommand> SubCommands { get; } = new List<ICommand>();

        public StatisticsCommand(
            IFileSystem fileSystem,
            ILogger<StatisticsCommand> logger,
            IReportLogger reportLogger,
            IMailSender mailSender,
            IPersistence<IPortfolio> persistence)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _reportLogger = reportLogger;
            _mailSender = mailSender;
            _persistence = persistence;
            _filepathOption = new CommandOption<string>("filepath", "The path to the portfolio.", required: true, FileValidator);
            Options.Add(_filepathOption);
            _outputPathOption = new CommandOption<string>("outputPath", "Path for the statistics file.");
            Options.Add(_outputPathOption);
            _fileTypeOption = new CommandOption<DocumentType>("docType", "The type of the stats to output.", required: false);
            Options.Add(_fileTypeOption);
            _mailRecipientOption = new CommandOption<string>("mailTo", "The email address to mail the stats to.", required: false);
            Options.Add(_mailRecipientOption);
            return;

            bool FileValidator(string filepath) => fileSystem.File.Exists(filepath);
        }

        /// <inheritdoc/>
        [LogIntercept]
        public int Execute(IConfiguration config)
        {
            var portfolio = _persistence.Load(PortfolioPersistence.CreateOptions(_filepathOption.Value, _fileSystem));
            _logger.Info($"Successfully loaded portfolio from {_filepathOption.Value}");

            DocumentType docType = _fileTypeOption.Value;

            string directory = _fileSystem.Path.GetDirectoryName(_outputPathOption.Value);
            string filePath = _fileSystem.Path.Combine(
                directory,
                $"{DateTime.Today.FileSuitableUKDateString()}{portfolio.Name}.{docType}");

            var settings = PortfolioStatisticsSettings.DefaultSettings();
            PortfolioStatistics stats = new PortfolioStatistics(portfolio, settings, _fileSystem);
            var exportSettings = PortfolioStatisticsExportSettings.DefaultSettings();
            stats.ExportToFile(_fileSystem, filePath, docType, exportSettings, _reportLogger);
            _logger.Info($"Successfully generated statistics page {filePath}");

            if (!string.IsNullOrWhiteSpace(_mailRecipientOption.Value))
            {
                var exportString = stats.ExportString(true, docType, exportSettings);
                string smtpAuthUser = config.GetValue<string>("SmtpAuthUser");
                string smtpAuthPassword = config.GetValue<string>("SmtpAuthPassword");
                var smtpInfo = SmtpInfo.GmailHost();
                smtpInfo.AuthUser = smtpAuthUser;
                smtpInfo.AuthPassword = smtpAuthPassword;
                var emailData = new MailInfo()
                {
                    Sender = smtpAuthUser,
                    Subject = "[Update] Stats auto update",
                    Body = exportString.ToString(),
                    Recipients = new List<string> { _mailRecipientOption.Value }
                };
                _mailSender.WriteEmail(_fileSystem, smtpInfo, emailData);
            }

            return 0;
        }

        /// <inheritdoc/>
        [LogIntercept]
        public bool Validate(IConfiguration config)
            => this.Validate(config, _logger);

        /// <inheritdoc/>
        [LogIntercept]
        public void WriteHelp()
            => this.WriteHelp(_logger);
    }
}
