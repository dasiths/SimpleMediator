using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Middleware
{
    public delegate Task<TResponse> HandleRequestDelegate<in TRequest, TResponse>(TRequest request, IMediationContext mediationContext, CancellationToken cancellationToken);

    public interface IMiddleware<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> RunAsync(TRequest request, IMediationContext mediationContext,
            CancellationToken cancellationToken, HandleRequestDelegate<TRequest, TResponse> next);
    }
}
