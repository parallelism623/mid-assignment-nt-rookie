using MIDASM.Application.Commons.Models;

namespace MIDASM.Application.Services.Mail;

public interface IMailServices
{
    public Task SendMailAsync(string toEmail, string subject, string body, bool isBodyHtml = true, CancellationToken cancellationToken = default);
    public Task SendMailWithAttachmentAsync(SendMailAttachmentData mailAttachmentData,
        CancellationToken cancellationToken = default);
}
