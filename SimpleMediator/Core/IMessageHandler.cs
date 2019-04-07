using System.Threading;
using System.Threading.Tasks;

namespace SimpleMediator.Core
{
    public interface IMessageHandler<in TMessage, TResponse> where TMessage : IMessage<TResponse> 
    {
        Task<TResponse> HandleAsync(TMessage message, IMediationContext mediationContext,
            CancellationToken cancellationToken);
    }
}