using SimpleMediator.Core;

namespace SimpleMediator.Queries
{
    public interface IQuery<TResult>: IMessage<TResult>
    {
    }
}
