
namespace MIDASM.Application.Dispatcher.Queries;

public interface IQueryHandler<TRequest, TResponse> 
    : IRequestHandler<TRequest, TResponse>
where TRequest : IRequest<TResponse>
{
}
