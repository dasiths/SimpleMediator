using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Middleware
{

    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

    public interface IMiddleware<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next);
    }
}
