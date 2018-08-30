using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Events
{
    public abstract class EventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IRequest<Unit>
    {
        public async Task<Unit> HandleAsync(TEvent request, IMediationContext mediationContext,
            CancellationToken cancellationToken)
        {
            await HandleEventAsync(request, mediationContext, cancellationToken);
            return Unit.Result;
        }

        protected abstract Task HandleEventAsync(TEvent @event, IMediationContext mediationContext,
            CancellationToken cancellationToken);
    }
}