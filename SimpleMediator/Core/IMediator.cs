using System.Threading;
using System.Threading.Tasks;

namespace SimpleMediator.Core
{
    public interface IMediator
    {
        Task<TResponse> HandleAsync<TResponse>(IMessage<TResponse> message,
            IMediationContext mediationContext = default(IMediationContext),
            CancellationToken cancellationToken = default(CancellationToken));
        Task<TResponse> HandleAsync<TMessage, TResponse>(TMessage message,
            IMediationContext mediationContext = default(IMediationContext),
            CancellationToken cancellationToken = default(CancellationToken)) where TMessage : IMessage<TResponse>;
    }
}