
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MIDASM.Domain.Infrastructure.MessageBrokers;

namespace MIDASM.Infrastructure.HostedServices.MessageBusConsumerBackgroundService;

public sealed class MessageBusConsumerBackgroundService<TConsumer, T>(
    IMessageBus messageBus,
    ILogger<MessageBusConsumerBackgroundService<TConsumer, T>> logger)
    : BackgroundService
    where T : IMessageBusMessage
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageBus.ReceiveAsync<TConsumer, T>(stoppingToken);
    }
}
