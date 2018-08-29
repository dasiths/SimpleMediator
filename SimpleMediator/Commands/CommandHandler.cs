using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Commands
{
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : IRequest<Unit>
    {
        public async Task<Unit> HandleAsync(TCommand request, IMediationContext mediationContext)
        {
            await HandleCommandAsync(request, mediationContext);
            return Unit.Result;
        }

        protected abstract Task HandleCommandAsync(TCommand command, IMediationContext mediationContext);
    }
}
