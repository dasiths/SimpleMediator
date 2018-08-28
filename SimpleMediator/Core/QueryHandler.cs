using System.Threading.Tasks;

namespace SimpleMediator.Core
{
    public abstract class QueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse> where TQuery : IRequest<TResponse>
    {
        public async Task<TResponse> HandleAsync(TQuery request)
        {
            return await HandleQueryAsync(request);
        }

        protected abstract Task<TResponse> HandleQueryAsync(TQuery query);
    }
}