
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MIDASS.Application.Services.BackgroundJobs.MailSenderBackgroundJob;
using MIDASS.Application.Services.HostedServices.Abstract;

namespace MIDASS.Infrastructure.HostedServices.MailSenderBackgroundService;

public class MailSenderBackgroundService : BackgroundService, IMailSenderBackgroundService
{
    private readonly ILogger<MailSenderBackgroundService> _logger;
    private readonly IBackgroundTaskQueue<Func<CancellationToken, ValueTask>> _queue;
    private const int Batch_Send = 10;
    public MailSenderBackgroundService(IBackgroundTaskQueue<Func<CancellationToken, ValueTask>> queue, 
         ILogger<MailSenderBackgroundService> logger)
    {
        _logger = logger;   
        _queue = queue;
    }

    public ValueTask<Func<CancellationToken, ValueTask>> DequeueSendMailRequestAsync()
    {
        return _queue.DequeueBackgroundWorkItemAsync();
    }

    public async ValueTask QueueSendMailRequestAsync(Func<CancellationToken, ValueTask> workItem)
    {
        await _queue.QueueBackgroundWorkItemAsync(workItem);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            var workItems = new List<Func<CancellationToken, ValueTask>>();
            for(int i = 0; i < Batch_Send; i++)
            {
                try
                {
                    using var cts = new CancellationTokenSource();
                    cts.CancelAfter(TimeSpan.FromSeconds(1));
                    var workItem = await _queue.DequeueBackgroundWorkItemAsync(cts.Token);
                    workItems.Add(workItem);
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Mail sender queue exception: {ex.Message}");
                }
            }
            if (workItems.Count > 0)
            {
                using var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(10));

                await Task.WhenAll(workItems.Select(wi => wi(cts.Token).AsTask()));
            }
        }
    }
}
