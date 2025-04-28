
using MIDASS.Application.Services.Mail;
using System.Net.Mail;
using System.Net;
using MIDASS.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Threading;

namespace MIDASS.Infrastructure.Mail;

public class MailServices : IMailServices
{
    private readonly EmailSettingsOptions _emailSettingsOptions;
    public MailServices(IOptions<EmailSettingsOptions> emailSettingsOptions)
    {
        _emailSettingsOptions = emailSettingsOptions.Value;
    }
    public Task SendMailAsync(string toEmail, string subject, string body, bool isBodyHtml = true, CancellationToken cancellationToken = default)
    {
        var mailServer = _emailSettingsOptions.MailServer;
        var fromEmail = _emailSettingsOptions.FromEmail;
        var password = _emailSettingsOptions.Password;
        var senderName = _emailSettingsOptions.SenderName;
        int port = _emailSettingsOptions.MailPort;
        var client = new SmtpClient(mailServer, port)
        {
            Credentials = new NetworkCredential(fromEmail, password),

            EnableSsl = true,
        };
        MailAddress fromAddress = new MailAddress(fromEmail, senderName);
        MailMessage mailMessage = new MailMessage
        {
            From = fromAddress, 
            Subject = subject, 
            Body = body, 
            IsBodyHtml = isBodyHtml 
        };

        mailMessage.To.Add(toEmail);

        return client.SendMailAsync(mailMessage, cancellationToken);
    }
}
