namespace FPD.Console.Utilities.Mail;

public sealed class SmtpInfo
{
    public string Host { get; set; }
    public string AuthUser { get; set; }
    public string AuthPassword { get; set; }
    public int Port { get; set; }

    public static SmtpInfo GmailHost()
        => new SmtpInfo() { Host = "smtp.gmail.com", Port = 587};

    public bool Validate()
    {
        if (string.IsNullOrEmpty(Host))
        {
            return false;
        }            
        if (string.IsNullOrEmpty(AuthUser))
        {
            return false;
        }            
        if (string.IsNullOrEmpty(AuthPassword))
        {
            return false;
        }

        return true;
    }
}