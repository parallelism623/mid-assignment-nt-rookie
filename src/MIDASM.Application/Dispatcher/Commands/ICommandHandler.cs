
namespace MIDASM.Application.Dispatcher.Commands;

public interface ICommandHandler<TRequest> : IRequestHandler<TRequest>
where TRequest : IRequest
{
}
