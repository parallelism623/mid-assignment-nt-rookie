
namespace MIDASM.Application.Dispatcher.Commands;

public interface ICommand : IRequest
{
}

public interface ICommand<T> : IRequest<T>{}
