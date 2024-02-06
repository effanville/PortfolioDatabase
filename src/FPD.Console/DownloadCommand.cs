using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Common.Structure.Extensions;
using Common.Structure.Reporting;
using Common.Structure.ReportWriting;

using Effanville.Common.Console;
using Effanville.Common.Console.Commands;
using Effanville.Common.Console.Options;
using Effanville.FPD.Console.Utilities.Mail;

using FinancialStructures.Database;
using FinancialStructures.Database.Download;
using FinancialStructures.Database.Export.Statistics;
using FinancialStructures.Persistence;

using Microsoft.Extensions.Configuration;

namespace Effanville.FPD.Console
{
    internal sealed class DownloadCommand : ICommand
    {
        private readonly IFileSystem _fileSystem;
        private readonly CommandOption<string> _filepathOption;
        private readonly CommandOption<bool> _updateStatsOption;
        private readonly CommandOption<string> _mailRecipientOption;

        public string Name => "download";

        /// <inheritdoc/>
        public IList<CommandOption> Options { get; } = new List<CommandOption>();

        /// <inheritdoc/>
        public IList<ICommand> SubCommands { get; } = new List<ICommand>();

        public DownloadCommand(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
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
        public int Execute(IConsole console, string[] args = null) 
            => Execute(console, null, args);

        /// <inheritdoc/>
        public int Execute(IConsole console, IReportLogger logger, string[] args = null)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            PortfolioPersistence portfolioPersistence = new PortfolioPersistence();
            PersistenceOptions persistenceOptions = PortfolioPersistence.CreateOptions(_filepathOption.Value, _fileSystem);
            IPortfolio portfolio = portfolioPersistence.Load(
                persistenceOptions,
                logger);
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
                
                logger.Log(ReportType.Information, "Mailing", $"Attempting to mail to stored recipient '{_mailRecipientOption.Value}'");
                if (!string.IsNullOrWhiteSpace(_mailRecipientOption.Value))
                {
                    string smtpAuthUser = config.GetValue<string>("SmtpAuthUser");
                    logger.Log(ReportType.Information, "Mailing", $"Attempting to mail with auth user of length {smtpAuthUser.Length}");
                    string smtpAuthPassword = config.GetValue<string>("SmtpAuthPassword");
                    logger.Log(ReportType.Information, "Mailing", $"Attempting to mail with auth pwd of length {smtpAuthPassword.Length}");
                    var smtpInfo = SmtpInfo.GmailHost();
                    smtpInfo.AuthUser = smtpAuthUser;
                    smtpInfo.AuthPassword = smtpAuthPassword;
                    var emailData = new MailInfo()
                    {
                        Sender = smtpAuthUser,
                        Subject = "[Update] Stats auto update",
                        Body = $"<h2>Statistic page update</h2><p>Update for portfolio {portfolio.Name} on date {DateTime.Now:yyyy-MM-dd}</p><p>Auto generated at {DateTime.Now:yyyy-MM-ddTHH:mm:ss}</p>",
                        Recipients = new List<string>{_mailRecipientOption.Value},
                        AttachmentFileNames = new List<string> {filePath}
                    };
                    logger.Log(ReportType.Information, "Mailing", $"Setup content for mailing.");
                    MailSender.WriteEmail(_fileSystem, smtpInfo, emailData, logger);
                }
            }

            bool saved = portfolioPersistence.Save(portfolio, persistenceOptions, logger);
            return saved ? 0 : 1;
        }

        /// <inheritdoc/>
        public bool Validate(IConsole console, string[] args) 
            => Validate(console, null, args);

        /// <inheritdoc/>
        public bool Validate(IConsole console, IReportLogger logger, string[] args) 
            => this.Validate(args, console, logger);

        /// <inheritdoc/>
        public void WriteHelp(IConsole console) 
            => CommandExtensions.WriteHelp(this, console);
    }
}
