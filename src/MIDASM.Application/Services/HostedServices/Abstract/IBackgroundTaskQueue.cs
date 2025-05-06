using System.Threading;

namespace MIDASM.Application.Services.HostedServices.Abstract;

public interface IBackgroundTaskQueue<T>
{
    ValueTask QueueBackgroundWorkItemAsync(T workItem, CancellationToken cancellationToken = default);

    ValueTask<T> DequeueBackgroundWorkItemAsync(
        CancellationToken cancellationToken = default);
}
