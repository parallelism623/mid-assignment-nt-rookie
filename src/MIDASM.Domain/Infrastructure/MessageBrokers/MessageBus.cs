
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MIDASM.Domain.Infrastructure.MessageBrokers;

public class MessageBus(IServiceProvider serviceProvider) : IMessageBus
{

    private static List<Type> _consumers = new List<Type>();
    internal static void AddConsumers(Assembly assembly, IServiceCollection services)
    {
        var types = assembly.GetTypes()
            .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IMessageBusConsumer<,>)))
            .ToList();

        foreach (var type in types)
        {
            services.AddTransient(type);
        }

        _consumers.AddRange(types);
    }

    public Task SendAsync<T>(T message, MetaData metaData = default, CancellationToken cancellationToken = default) 
        where T : IMessageBusMessage
    {
        return serviceProvider.GetRequiredService<IMessageSender<T>>().SendAsync(message, metaData, cancellationToken);
    }

    public Task ReceiveAsync<TConsumer, T>(Func<T, MetaData, Task> action, CancellationToken cancellationToken = default) where T : IMessageBusMessage
    {
        return serviceProvider.GetRequiredService<IMessageReceiver<TConsumer, T>>()
            .ReceiveAsync(action, cancellationToken);
    }

    public Task ReceiveAsync<TConsumer, T>(CancellationToken cancellationToken = default) where T : IMessageBusMessage
    {
        return serviceProvider.GetRequiredService<IMessageReceiver<TConsumer, T>>()
            .ReceiveAsync(async (data, metaData) =>
            {
                using var scope = serviceProvider.CreateScope();
                foreach (Type handlerType in _consumers)
                {
                    bool canHandleEvent = handlerType.GetInterfaces()
                        .Any(x => x.IsGenericType
                                  && x.GetGenericTypeDefinition() == typeof(IMessageBusConsumer<,>)
                                  && x.GenericTypeArguments[0] == typeof(TConsumer) && x.GenericTypeArguments[1] == typeof(T));

                    if (canHandleEvent)
                    {
                        dynamic handler = scope.ServiceProvider.GetService(handlerType);
                        await handler.HandleAsync((dynamic)data, metaData, cancellationToken);
                    }
                }
            }, cancellationToken);
    }
}

public static class MessageBusExtensions
{
    public static IServiceCollection AddMessageBusConsumers(this IServiceCollection services, Assembly assembly)
    {
        MessageBus.AddConsumers(assembly, services);
        return services;
    }
}