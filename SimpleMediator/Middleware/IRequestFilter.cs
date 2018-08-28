using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Middleware
{
    public delegate Task RequestFilterDelegate<in TRequest>(TRequest request);

    public interface IRequestFilter<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task FilterAsync(TRequest request, RequestFilterDelegate<TRequest> next);
    }
}
