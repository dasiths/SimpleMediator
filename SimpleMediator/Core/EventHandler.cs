using System.Threading.Tasks;

namespace SimpleMediator.Core
{
    public abstract class EventHandler<TEvent> : IRequestHandler<TEvent, Unit> where TEvent : IRequest<Unit>
    {
        public async Task<Unit> HandleAsync(TEvent request)
        {
            await HandleEventAsync(request);
            return Unit.Result;
        }

        protected abstract Task HandleEventAsync(TEvent @event);
    }
}