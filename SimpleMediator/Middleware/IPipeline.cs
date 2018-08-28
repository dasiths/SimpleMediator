using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Middleware
{
    public interface IPipeline<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request, IServiceFactory serviceFactory);
    }
}
