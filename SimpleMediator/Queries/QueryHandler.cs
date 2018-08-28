using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Queries
{
    public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse> where TQuery : IRequest<TResponse>
    {
        public async Task<TResponse> HandleAsync(TQuery request)
        {
            return await HandleQueryAsync(request);
        }

        protected abstract Task<TResponse> HandleQueryAsync(TQuery query);
    }
}