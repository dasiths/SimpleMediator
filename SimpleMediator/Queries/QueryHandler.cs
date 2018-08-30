using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Queries
{
    public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse> where TQuery : IRequest<TResponse>
    {
        public async Task<TResponse> HandleAsync(TQuery request, IMediationContext mediationContext,
            CancellationToken cancellationToken)
        {
            return await HandleQueryAsync(request, mediationContext, cancellationToken);
        }

        protected abstract Task<TResponse> HandleQueryAsync(TQuery query, IMediationContext mediationContext,
            CancellationToken cancellationToken);
    }
}