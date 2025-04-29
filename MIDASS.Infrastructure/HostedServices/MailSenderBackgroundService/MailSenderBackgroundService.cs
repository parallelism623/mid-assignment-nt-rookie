
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MIDASS.Application.Services.HostedServices.Abstract;

namespace MIDASS.Infrastructure.HostedServices.MailSenderBackgroundService;

public class MailSenderBackgroundService : BackgroundService
{
    private readonly IBackgroundTaskQueue<Func<IServiceProvider, CancellationToken, ValueTask>> _queue;
    private readonly ILogger<MailSenderBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    public MailSenderBackgroundService(ILogger<MailSenderBackgroundService> logger, 
        IServiceProvider serviceProvider, IBackgroundTaskQueue<Func<IServiceProvider, 
            CancellationToken, ValueTask>> queue)
    {   
        _logger = logger;
        _queue = queue;
        _serviceProvider = serviceProvider;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            var workItem = await _queue.DequeueBackgroundWorkItemAsync(stoppingToken);

            try
            {
                await workItem(_serviceProvider, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred executing {nameof(workItem)}.");
            }

        }
        _logger.LogInformation("Queued Processor Background Service is stopping.");
    }
}

