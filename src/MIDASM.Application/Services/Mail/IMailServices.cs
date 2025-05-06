using MailKit.Net.Smtp;
using MIDASM.Application.Commons.Models;

using MimeKit;
namespace MIDASM.Application.Services.Mail;

public interface IMailServices
{
    public Task SendMailAsync(string toEmail, string subject, string body, bool isBodyHtml = true, CancellationToken cancellationToken = default);
    public Task SendMailWithAttachmentAsync(SendMailAttachmentData mailAttachmentData,
        CancellationToken cancellationToken = default);

    Task<SmtpClient> GetSmtpClient();
    MimeMessage GetMimeMessage(SendMailAttachmentData mailAttachmentData);
}
