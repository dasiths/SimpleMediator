using SimpleMediator.Core;

namespace SimpleMediator.Commands
{
    public interface ICommandHandler<in TCommand>: IRequestHandler<TCommand, Unit> where TCommand : IRequest<Unit>
    {
    }
}