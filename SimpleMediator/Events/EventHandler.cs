using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Events
{
    public abstract class EventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IRequest<Unit>
    {
        public async Task<Unit> HandleAsync(TEvent request, IMediationContext mediationContext)
        {
            await HandleEventAsync(request, mediationContext);
            return Unit.Result;
        }

        protected abstract Task HandleEventAsync(TEvent @event, IMediationContext mediationContext);
    }
}