using System.IO.Abstractions;

namespace Effanville.FPD.Console.Utilities.Mail;

public interface IMailSender
{
    void WriteEmail(
        IFileSystem fileSystem,
        SmtpInfo smtpInfo,
        MailInfo mailInfo);
}