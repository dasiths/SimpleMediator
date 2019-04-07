using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Middleware;

namespace SimpleMediator.Samples.MassTransit
{
    public class MassTransitMediationMiddleware<TMessage, TResponse> : IMiddleware<TMessage, TResponse> where TMessage : class, IMessage<TResponse> where TResponse : class
    {
        public async Task<TResponse> RunAsync(TMessage message, IMediationContext mediationContext,
            CancellationToken cancellationToken, HandleMessageDelegate<TMessage, TResponse> next)
        {
            if (mediationContext.GetType().IsAssignableFrom(typeof(MassTransitSendMediationContext<TMessage, TResponse>)))
            {
                var context = mediationContext as MassTransitSendMediationContext<TMessage, TResponse>;
                return await context.Client.Request(message, cancellationToken);
            }

            // Pass through
            return await next.Invoke(message, mediationContext, cancellationToken);
        }
    }
}