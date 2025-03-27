using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.Common.ReportWriting.Documents;
using Effanville.Common.Structure.Extensions;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.Structure.Reporting.LogAspect;
using Effanville.Common.Structure.WebAccess;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Export.Statistics;
using Effanville.FinancialStructures.Download;
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
        private readonly IConfiguration _config;
        private readonly IMailSender _mailSender;
        private readonly IPersistence<IPortfolio> _persistence;
        private readonly CommandOption<string> _filepathOption;
        private readonly CommandOption<bool> _updateStatsOption;
        private readonly CommandOption<string> _mailRecipientOption;
        private string _smtpAuthUser;
        private string _smtpAuthPassword;
        private CommandOptions _commandOptions;

        public string Name => "download";

        public ILogger Logger => _logger;
        /// <inheritdoc/>
        public IList<CommandOption> Options { get; } = new List<CommandOption>();

        /// <inheritdoc/>
        public IList<ICommand> SubCommands { get; } = new List<ICommand>();

        public DownloadCommand(
            IFileSystem fileSystem,
            ILogger<DownloadCommand> logger,
            IReportLogger reportLogger,
            IConfiguration config,
            IMailSender mailSender,
            IPersistence<IPortfolio> persistence)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _reportLogger = reportLogger;
            _config = config;
            _mailSender = mailSender;
            _persistence = persistence;
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
        public int Execute()
        {
            PersistenceOptions persistenceOptions = PortfolioPersistence.CreateOptions(_filepathOption.Value, _fileSystem);
            IPortfolio portfolio = _persistence.Load(persistenceOptions);
            _logger.Info($"Successfully loaded portfolio from {_filepathOption.Value}");

            WebDownloader webDownloader = new WebDownloader(_reportLogger);
            PriceDownloaderFactory priceDownloaderFactory = new PriceDownloaderFactory(_reportLogger, webDownloader);
            new PortfolioDataDownloader(priceDownloaderFactory).Download(portfolio, _reportLogger).Wait();

            if (_updateStatsOption.Value)
            {
                string directory = _fileSystem.Path.GetDirectoryName(_filepathOption.Value);
                string filePath = _fileSystem.Path.Combine(
                    directory,
                    $"{DateTime.Today.FileSuitableUKDateString()}-{portfolio.Name}.html");
                var settings = PortfolioStatisticsSettings.DefaultSettings();
                PortfolioStatistics stats = new PortfolioStatistics(portfolio, settings, _fileSystem);
                PortfolioStatisticsExportSettings exportSettings = _commandOptions.StatsExport.Create();
                stats.ExportToFile(_fileSystem, filePath, DocumentType.Html, exportSettings, _reportLogger);

                _logger.Info($"Attempting to mail to stored recipient '{_mailRecipientOption.Value}'");
                if (!string.IsNullOrWhiteSpace(_mailRecipientOption.Value))
                {
                    Common.ReportWriting.ReportBuilder exportString = stats.ExportString(true, DocumentType.Html, exportSettings);

                    SmtpInfo smtpInfo = SmtpInfo.GmailHost();
                    smtpInfo.AuthUser = _smtpAuthUser;
                    smtpInfo.AuthPassword = _smtpAuthPassword;
                    MailInfo emailData = new MailInfo()
                    {
                        Sender = _smtpAuthUser,
                        Subject = "[Update] Stats auto update",
                        Body = exportString.ToString(),
                        Recipients = new List<string> { _mailRecipientOption.Value }
                    };
                    _logger.Info($"Setup content for mailing.");
                    _mailSender.WriteEmail(_fileSystem, smtpInfo, emailData);
                }
            }

            bool saved = _persistence.Save(portfolio, persistenceOptions);
            return saved ? 0 : 1;
        }

        /// <inheritdoc/>
        [LogIntercept]
        public bool Validate()
        {
            _smtpAuthUser = _config.GetValue<string>("SmtpAuthUser");
            _logger.Info($"Mail user has length {_smtpAuthUser?.Length}");

            _smtpAuthPassword = _config.GetValue<string>("SmtpAuthPassword");

            _logger.Info($"Mail auth pwd has length {_smtpAuthPassword?.Length}");
            _commandOptions = _config.GetSection(CommandOptions.Command).Get<CommandOptions>();
            _logger.Info($"Retrieved options");
            return this.Validate(_config, _logger);
        }

        /// <inheritdoc/>
        [LogIntercept]
        public void WriteHelp()
            => this.WriteHelp(_logger);
    }
}
