using SimpleMediator.Core;

namespace SimpleMediator.Queries
{
    public interface IQueryHandler<in TQuery, TResponse> : IMessageHandler<TQuery, TResponse>
        where TQuery : IMessage<TResponse>
    {
    }
}