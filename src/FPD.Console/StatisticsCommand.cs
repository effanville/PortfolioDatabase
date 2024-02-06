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

using FinancialStructures.Database.Export.Statistics;
using FinancialStructures.Persistence;

using Microsoft.Extensions.Configuration;

namespace Effanville.FPD.Console
{
    internal sealed class StatisticsCommand : ICommand
    {
        readonly IFileSystem _fileSystem;
        readonly CommandOption<string> _filepathOption;
        readonly CommandOption<string> _outputPathOption;
        readonly CommandOption<DocumentType> _fileTypeOption;
        private readonly CommandOption<string> _mailRecipientOption;

        /// <inheritdoc/>
        public string Name => "stats";

        /// <inheritdoc/>
        public IList<CommandOption> Options { get; } = new List<CommandOption>();

        /// <inheritdoc/>
        public IList<ICommand> SubCommands { get; } = new List<ICommand>();

        public StatisticsCommand(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
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
        public int Execute(IConsole console, string[] args = null) 
            => Execute(console, null, args);

        /// <inheritdoc/>
        public int Execute(IConsole console, IReportLogger logger, string[] args = null)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            var portfolioPersistence = new PortfolioPersistence();
            var portfolio = portfolioPersistence.Load(
                PortfolioPersistence.CreateOptions(_filepathOption.Value, _fileSystem),
                logger);
            logger.Log(ReportType.Information, "Loading",
                $"Successfully loaded portfolio from {_filepathOption.Value}");

            DocumentType docType = _fileTypeOption.Value;

            string directory = _fileSystem.Path.GetDirectoryName(_outputPathOption.Value);
            string filePath = _fileSystem.Path.Combine(
                directory,
                $"{DateTime.Today.FileSuitableUKDateString()}{portfolio.Name}.{docType}");

            var settings = PortfolioStatisticsSettings.DefaultSettings();
            PortfolioStatistics stats = new PortfolioStatistics(portfolio, settings, _fileSystem);
            var exportSettings = PortfolioStatisticsExportSettings.DefaultSettings();
            stats.ExportToFile(_fileSystem, filePath, docType, exportSettings, logger);
            logger.Log(ReportType.Information, "StatisticsGeneration",
                $"Successfully generated statistics page {filePath}");

            if (!string.IsNullOrWhiteSpace(_mailRecipientOption.Value))
            {
                string smtpAuthUser = config.GetValue<string>("SmtpAuthUser");
                string smtpAuthPassword = config.GetValue<string>("SmtpAuthPassword");
                var smtpInfo = SmtpInfo.GmailHost();
                smtpInfo.AuthUser = smtpAuthUser;
                smtpInfo.AuthPassword = smtpAuthPassword;
                var emailData = new MailInfo()
                {
                    Sender = smtpAuthUser,
                    Subject = "[Update] Stats auto update",
                    Body =
                        $"<h2>Statistic page update</h2><p>Update for portfolio {portfolio.Name} on date {DateTime.Now:yyyy-MM-dd}</p><p>Auto generated at {DateTime.Now:yyyy-MM-ddTHH:mm:ss}</p>",
                    Recipients = new List<string> { _mailRecipientOption.Value },
                    AttachmentFileNames = new List<string> { filePath }
                };
                MailSender.WriteEmail(_fileSystem, smtpInfo, emailData, logger);
            }

            return 0;
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
