using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Commands
{
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : IMessage<Unit>
    {
        public async Task<Unit> HandleAsync(TCommand message, IMediationContext mediationContext,
            CancellationToken cancellationToken)
        {
            await HandleCommandAsync(message, mediationContext, cancellationToken);
            return Unit.Result;
        }

        protected abstract Task HandleCommandAsync(TCommand command, IMediationContext mediationContext,
            CancellationToken cancellationToken);
    }
}
