using System.IO.Abstractions;

using Effanville.Common.Structure.Reporting;

namespace Effanville.FPD.Console.Utilities.Mail;

public interface IMailSender
{
    void WriteEmail(
        IFileSystem fileSystem,
        SmtpInfo smtpInfo,
        MailInfo mailInfo,
        IReportLogger logger);
}