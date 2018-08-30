using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Middleware;

namespace SimpleMediator.Samples.MassTransit
{
    public class MassTransitRequestMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse> where TRequest : class, IRequest<TResponse> where TResponse : class
    {
        public async Task<TResponse> RunAsync(TRequest request, IMediationContext mediationContext,
            CancellationToken cancellationToken, HandleRequestDelegate<TRequest, TResponse> next)
        {
            if (mediationContext.GetType().IsAssignableFrom(typeof(MassTransitMediationContext<TRequest, TResponse>)))
            {
                var context = mediationContext as MassTransitMediationContext<TRequest, TResponse>;
                return await context.Client.Request(request, cancellationToken);
            }

            // Pass through
            return await next.Invoke(request, mediationContext, cancellationToken);
        }
    }
}