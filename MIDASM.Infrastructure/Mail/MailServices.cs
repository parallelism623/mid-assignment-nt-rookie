
using Microsoft.Extensions.Options;
using MIDASM.Application.Commons.Models;
using MIDASM.Application.Services.Mail;
using MIDASM.Infrastructure.Options;
using System.Net;
using System.Net.Mail;

namespace MIDASM.Infrastructure.Mail;

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

    public async Task SendMailWithAttachmentAsync(SendMailAttachmentData mailAttachmentData, CancellationToken cancellationToken = default)
    {
        byte[]? fileBytes = mailAttachmentData.FileBytes;
        string fileName = "due-date-book.xlsx";
        string mimeType = mailAttachmentData.MineType;

        var mailServer = _emailSettingsOptions.MailServer;
        var fromEmail = _emailSettingsOptions.FromEmail;
        var password = _emailSettingsOptions.Password;
        var senderName = _emailSettingsOptions.SenderName;
        int port = _emailSettingsOptions.MailPort;
        using var client = new SmtpClient(mailServer, port)
        {
            Credentials = new NetworkCredential(fromEmail, password),

            EnableSsl = true,
        };
        MailAddress fromAddress = new MailAddress(fromEmail, senderName);
        using MailMessage mailMessage = new MailMessage
        {
            From = fromAddress,
            Subject = mailAttachmentData.Subject,
            Body = mailAttachmentData.Body,
            IsBodyHtml = mailAttachmentData.IsBodyHtml
        };

        mailMessage.To.Add(mailAttachmentData.ToEmail);
        if(fileBytes != null)
        {
            var stream = new MemoryStream(fileBytes);

            var attachment = new Attachment(stream, fileName, mimeType);
            mailMessage.Attachments.Add(attachment);
            
        }
        await client.SendMailAsync(mailMessage);
    }
}
