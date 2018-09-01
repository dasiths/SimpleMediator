using SimpleMediator.Core;

namespace SimpleMediator.Queries
{
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IRequest<TResponse>
    {
    }
}