using System;
using System.IO.Abstractions;
using System.Net.Mail;
using System.Net.Mime;

using Effanville.Common.Structure.Extensions;

using Microsoft.Extensions.Logging;

namespace Effanville.FPD.Console.Utilities.Mail;

public sealed class MailSender : IMailSender
{
    private readonly ILogger _logger;

    public MailSender(ILogger<MailSender> logger) => _logger = logger;

    public void WriteEmail(
        IFileSystem fileSystem,
        SmtpInfo smtpInfo,
        MailInfo mailInfo)
    {
        try
        {
            if (!smtpInfo.Validate())
            {
                _logger.Error("Could not validate smtp info.");
                return;
            }

            SmtpClient client = new SmtpClient(smtpInfo.Host);
            MailMessage newMail = new MailMessage()
            {
                From = new MailAddress(mailInfo.Sender)
            };
            foreach (string toAddress in mailInfo.Recipients)
            {
                newMail.To.Add(toAddress);
            }

            newMail.Subject = mailInfo.Subject;
            newMail.IsBodyHtml = true;
            newMail.Body = mailInfo.Body;
            if (mailInfo.AttachmentFileNames != null)
            {
                foreach (string file in mailInfo.AttachmentFileNames)
                {
                    Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);

                    // Add time stamp information for the file.
                    ContentDisposition disposition = data.ContentDisposition;
                    if (disposition != null)
                    {
                        disposition.CreationDate = fileSystem.File.GetCreationTime(file);
                        disposition.ModificationDate = fileSystem.File.GetLastWriteTime(file);
                        disposition.ReadDate = fileSystem.File.GetLastAccessTime(file);
                    }

                    // Add the file attachment to this email message.
                    newMail.Attachments.Add(data);
                }
            }

            _logger.Info("Added all attachments.");

            client.EnableSsl = true;
            client.Port = smtpInfo.Port;
            client.Credentials = new System.Net.NetworkCredential(smtpInfo.AuthUser, smtpInfo.AuthPassword);
            client.Send(newMail);
            _logger.Info("Sent mail.");
        }
        catch (Exception ex)
        {
            _logger.Exception(ex);
        }
    }
}