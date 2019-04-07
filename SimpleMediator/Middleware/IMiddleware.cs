using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Middleware
{
    public delegate Task<TResponse> HandleMessageDelegate<in TMessage, TResponse>(TMessage message, IMediationContext mediationContext, CancellationToken cancellationToken);

    public interface IMiddleware<TMessage, TResponse> where TMessage : IMessage<TResponse>
    {
        Task<TResponse> RunAsync(TMessage message, IMediationContext mediationContext,
            CancellationToken cancellationToken, HandleMessageDelegate<TMessage, TResponse> next);
    }
}
