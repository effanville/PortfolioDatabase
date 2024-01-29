using System;
using System.IO.Abstractions;
using System.Net.Mail;
using System.Net.Mime;

using Common.Structure.Reporting;

namespace FPD.Console.Utilities.Mail;

internal static class MailSender
{
    public static void WriteEmail(
        IFileSystem fileSystem,
        SmtpInfo smtpInfo,
        MailInfo mailInfo,
        IReportLogger logger)
    {
        try
        {
            if (!smtpInfo.Validate())
            {
                logger.Error("Emailing", "Could not validate smtp info.");
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
            logger.Log(ReportType.Information, "Emailing", "Added all attachments.");

            client.EnableSsl = true;
            client.Port = smtpInfo.Port;
            client.Credentials = new System.Net.NetworkCredential(smtpInfo.AuthUser, smtpInfo.AuthPassword);
            client.Send(newMail);
            logger.Log(ReportType.Information, "Emailing", "Sent mail.");
        }
        catch (Exception ex)
        {
            logger.Error("Emailing", ex.ToString());
        }
    }
}