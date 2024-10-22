using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.Structure.Reporting.LogAspect;
using Effanville.Common.Structure.ReportWriting;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Download;
using Effanville.FinancialStructures.Database.Export.Statistics;
using Effanville.FinancialStructures.Persistence;
using Effanville.FPD.Console.Utilities.Mail;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Effanville.FPD.Console
{
    internal sealed class DownloadCommand : ICommand, ILogInterceptable
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly IReportLogger _reportLogger;
        private readonly IMailSender _mailSender;
        private readonly CommandOption<string> _filepathOption;
        private readonly CommandOption<bool> _updateStatsOption;
        private readonly CommandOption<string> _mailRecipientOption;

        public string Name => "download";

        public ILogger Logger => _logger;
        /// <inheritdoc/>
        public IList<CommandOption> Options { get; } = new List<CommandOption>();

        /// <inheritdoc/>
        public IList<ICommand> SubCommands { get; } = new List<ICommand>();

        public DownloadCommand(IFileSystem fileSystem, ILogger<DownloadCommand> logger, IReportLogger reportLogger, IMailSender mailSender)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _reportLogger = reportLogger;
            _mailSender = mailSender;
            _filepathOption = new CommandOption<string>("filepath", "The path to the portfolio.", required: true, FileValidator);
            Options.Add(_filepathOption);
            _updateStatsOption = new CommandOption<bool>("updateStats", "Update stats for portfolio.");
            Options.Add(_updateStatsOption);
            _mailRecipientOption = new CommandOption<string>("mailTo", "The email address to mail the stats to.", required: false);
            Options.Add(_mailRecipientOption);
            return;

            bool FileValidator(string filepath) => fileSystem.File.Exists(filepath);
        }

        /// <inheritdoc/>
        [LogIntercept]
        public int Execute(IConfiguration config)
        {
            PortfolioPersistence portfolioPersistence = new PortfolioPersistence();
            PersistenceOptions persistenceOptions = PortfolioPersistence.CreateOptions(_filepathOption.Value, _fileSystem);
            IPortfolio portfolio = portfolioPersistence.Load(
                persistenceOptions,
                _reportLogger);
            _logger.Log(LogLevel.Information, $"Successfully loaded portfolio from {_filepathOption.Value}");

            PortfolioDataUpdater.Download(Account.All, portfolio, null, _reportLogger).Wait();

            if (_updateStatsOption.Value)
            {
                string directory = _fileSystem.Path.GetDirectoryName(_filepathOption.Value);
                string filePath = _fileSystem.Path.Combine(
                    directory,
                    $"{DateTime.Today:yyyy-MM-dd}-{portfolio.Name}.html");
                var settings = PortfolioStatisticsSettings.DefaultSettings();
                PortfolioStatistics stats = new PortfolioStatistics(portfolio, settings, _fileSystem);
                var exportSettings = PortfolioStatisticsExportSettings.DefaultSettings();
                stats.ExportToFile(_fileSystem, filePath, DocumentType.Html, exportSettings, _reportLogger);
                
                _logger.Log(LogLevel.Information, $"Attempting to mail to stored recipient '{_mailRecipientOption.Value}'");
                if (!string.IsNullOrWhiteSpace(_mailRecipientOption.Value))
                {
                    var exportString = stats.ExportString(true, DocumentType.Html, exportSettings);
                    string smtpAuthUser = config.GetValue<string>("SmtpAuthUser");
                    _logger.Log(LogLevel.Information, $"Attempting to mail with auth user of length {smtpAuthUser.Length}");
                    string smtpAuthPassword = config.GetValue<string>("SmtpAuthPassword");
                    _logger.Log(LogLevel.Information,$"Attempting to mail with auth pwd of length {smtpAuthPassword.Length}");
                    var smtpInfo = SmtpInfo.GmailHost();
                    smtpInfo.AuthUser = smtpAuthUser;
                    smtpInfo.AuthPassword = smtpAuthPassword;
                    var emailData = new MailInfo()
                    {
                        Sender = smtpAuthUser,
                        Subject = "[Update] Stats auto update",
                        Body = exportString.ToString(),
                        Recipients = new List<string>{_mailRecipientOption.Value}
                    };
                    _logger.Log(LogLevel.Information, $"Setup content for mailing.");
                    _mailSender.WriteEmail(_fileSystem, smtpInfo, emailData, _reportLogger);
                }
            }

            bool saved = portfolioPersistence.Save(portfolio, persistenceOptions, _reportLogger);
            return saved ? 0 : 1;
        }

        /// <inheritdoc/>
        [LogIntercept]
        public bool Validate( IConfiguration config) 
            => this.Validate(config, _logger);

        /// <inheritdoc/>
        [LogIntercept]
        public void WriteHelp() 
            => this.WriteHelp( _logger);
    }
}
