using SimpleMediator.Core;

namespace SimpleMediator.Commands
{
    public interface ICommandHandler<in TCommand>: IMessageHandler<TCommand, Unit> where TCommand : IMessage<Unit>
    {
    }
}