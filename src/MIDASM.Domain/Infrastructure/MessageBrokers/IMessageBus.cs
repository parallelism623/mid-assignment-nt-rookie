﻿
namespace MIDASM.Domain.Infrastructure.MessageBrokers;

public interface IMessageBus
{
    Task SendAsync<T>(T message, MetaData metaData = null, CancellationToken cancellationToken = default)
        where T : IMessageBusMessage;

    Task ReceiveAsync<TConsumer, T>(Func<T, MetaData, Task> action, CancellationToken cancellationToken = default)
        where T : IMessageBusMessage;

    Task ReceiveAsync<TConsumer, T>(CancellationToken cancellationToken = default)
        where T : IMessageBusMessage;

}

public interface IMessageBusMessage
{
}
public interface IMessageBusEvent : IMessageBusMessage
{
}

public interface IMessageBusCommand : IMessageBusMessage
{
}

