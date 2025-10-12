namespace LabServer.Server.Service;

using System.Net;
using System.Net.Mail;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly SmtpClient _client;
    private System.String _fromAddress;
    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
        var smtpConfig = _configuration.GetSection("SMTP");
        
        _fromAddress = smtpConfig.GetValue<System.String>("email", "not@found.com");
        _client = new SmtpClient(smtpConfig.GetValue<System.String>("domain", "not_found.com"))
        {
            Port = smtpConfig.GetValue<System.Int32>("port", 587),
            Credentials = new NetworkCredential(
                smtpConfig.GetValue<System.String>("username", "default"),
                smtpConfig.GetValue<System.String>("password", "default")
            ),
            EnableSsl = smtpConfig.GetValue<System.Boolean>("ssl"),
            DeliveryMethod = SmtpDeliveryMethod.Network
        };
    }

    public System.Boolean Send(System.String targetEmail, System.String subject, System.String message)
    {
        try
        {
            _client.Send(_fromAddress, targetEmail, subject, message);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[!] an error occured while sending email: {e}");
            return false;
        }
        return true;
    }
}