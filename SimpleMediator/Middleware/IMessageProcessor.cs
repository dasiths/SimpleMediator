using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Middleware
{
    public interface IMessageProcessor<in TMessage, TResponse>
        where TMessage : IMessage<TResponse>
    {
        Task<TResponse> HandleAsync(TMessage message, IMediationContext mediationContext,
            CancellationToken cancellationToken);
    }
}
