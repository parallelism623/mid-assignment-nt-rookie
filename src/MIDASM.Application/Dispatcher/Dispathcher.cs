namespace MIDASM.Application.Dispatcher;

public sealed class Dispatcher(IServiceProvider serviceProvider)
{
    public async Task<T> Dispatch<T>(IRequest<T> request, CancellationToken cancellationToken = default)
    {
        Type type = typeof(IRequestHandler<,>);
        Type[] typeArgs = { request.GetType(), typeof(T) };
        Type handlerType = type.MakeGenericType(typeArgs);

        dynamic handler = serviceProvider!.GetService(handlerType)
                ?? throw new ArgumentException($"Cannot resolve handler for {nameof(IRequest<T>)}");
        Task<T> result = handler.HandleAsync((dynamic)request, cancellationToken);

        return await result;
    }

    public async Task Dispatch(IRequest request, CancellationToken cancellationToken = default)
    {
        Type type = typeof(IRequestHandler<,>);
        Type[] typeArgs = { request.GetType()};
        Type handlerType = type.MakeGenericType(typeArgs);

        dynamic handler = serviceProvider!.GetService(handlerType)
                          ?? throw new ArgumentException($"Cannot resolve handler for {nameof(IRequest)}");
        Task result = handler.HandleAsync((dynamic)request, cancellationToken);

        await result;
    }
}
