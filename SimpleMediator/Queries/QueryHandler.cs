using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Queries
{
    public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse> where TQuery : IRequest<TResponse>
    {
        public async Task<TResponse> HandleAsync(TQuery request, IMediationContext mediationContext)
        {
            return await HandleQueryAsync(request, mediationContext);
        }

        protected abstract Task<TResponse> HandleQueryAsync(TQuery query, IMediationContext mediationContext);
    }
}