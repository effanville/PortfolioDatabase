using System.Collections.Generic;

namespace FPDconsole.Utilities.Mail;

public sealed class MailInfo
{
    public string Sender { get; set; }
    public List<string> Recipients { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<string> AttachmentFileNames { get; set; }
}