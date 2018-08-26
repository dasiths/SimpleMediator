using System.Threading.Tasks;

namespace SimpleMediator.Core
{
    public interface IRequestHandler<in TRequest, TResponse> where TRequest : IQuery<TResponse> 
    {
        Task<TResponse> HandleAsync(TRequest request);
    }
}