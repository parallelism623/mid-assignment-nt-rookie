namespace MIDASS.Application.Services.BackgroundJobs.MailSenderBackgroundJob;

public interface IMailSenderBackgroundService
{
    public ValueTask QueueSendMailRequestAsync(Func<CancellationToken, ValueTask> workItem);
    public ValueTask<Func<CancellationToken, ValueTask>> DequeueSendMailRequestAsync();

}
