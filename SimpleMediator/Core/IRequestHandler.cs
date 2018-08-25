using System.Threading.Tasks;

namespace SimpleMediator.Core
{
    public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse> 
    {
        Task<TResponse> HandleAsync(TRequest request);
    }

    public interface IRequestHandler<in TRequest> where TRequest : IRequest
    {
        Task HandleAsync(TRequest request);
    }
}