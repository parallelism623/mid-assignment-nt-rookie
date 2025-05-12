
using MediatR;

namespace MIDASM.Application.Dispatcher;

public interface IRequestHandler<TRequest>
    where TRequest : IRequest
{
    Task HandlerAsync(TRequest request, CancellationToken cancellationToken = default);
}

public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandlerAsync(TRequest request, CancellationToken cancellationToken = default);
} 