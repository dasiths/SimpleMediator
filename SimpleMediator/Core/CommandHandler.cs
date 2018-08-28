using System.Threading.Tasks;

namespace SimpleMediator.Core
{
    public abstract class CommandHandler<TCommand> : IRequestHandler<TCommand, Unit> where TCommand : IRequest<Unit>
    {
        public async Task<Unit> HandleAsync(TCommand request)
        {
            await HandleCommandAsync(request);
            return new Unit();
        }

        protected abstract Task HandleCommandAsync(TCommand command);
    }
}
