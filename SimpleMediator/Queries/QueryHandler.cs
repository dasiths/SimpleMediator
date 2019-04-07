using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Queries
{
    public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse> where TQuery : IMessage<TResponse>
    {
        public Task<TResponse> HandleAsync(TQuery message, IMediationContext mediationContext,
            CancellationToken cancellationToken)
        {
            return HandleQueryAsync(message, mediationContext, cancellationToken);
        }

        protected abstract Task<TResponse> HandleQueryAsync(TQuery query, IMediationContext mediationContext,
            CancellationToken cancellationToken);
    }
}