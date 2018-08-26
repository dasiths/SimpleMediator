using System.Threading.Tasks;

namespace SimpleMediator.Core
{
    public abstract class CommandHandler<TCommand> : IRequestHandler<TCommand, Unit> where TCommand : IQuery<Unit>
    {
        public async Task<Unit> HandleAsync(TCommand request)
        {
            await ExecuteAsync(request);
            return new Unit();
        }

        protected abstract Task ExecuteAsync(TCommand command);
    }
}
