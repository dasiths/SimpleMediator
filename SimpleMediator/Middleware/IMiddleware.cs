using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Middleware
{
    public delegate Task<TResponse> HandleRequestDelegate<in TRequest, TResponse>(TRequest request);

    public interface IMiddleware<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> RunAsync(TRequest request, HandleRequestDelegate<TRequest, TResponse> next, IMediationContext mediationContext);
    }
}
