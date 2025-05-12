
namespace MIDASM.Domain.Infrastructure.MessageBrokers;

public class MetaData
{
    public string MessageId { get; set; } = default;

    public string MessageVersion { get; set; } = default;

    public string ActivityId { get; set; } = default;

    public DateTimeOffset? CreationDateTime { get; set; }

    public DateTimeOffset? EnqueuedDateTime { get; set; }
}