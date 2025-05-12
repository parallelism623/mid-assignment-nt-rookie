
namespace MIDASM.Infrastructure.MessageBroker.RabbitMQ;

public class RabbitMqSenderOptions
{

    public string HostName { get; set; } = default;

    public string UserName { get; set; } = default;

    public string Password { get; set; } = default;

    public string ExchangeName { get; set; } = default;

    public string RoutingKey { get; set; } = default;
}
