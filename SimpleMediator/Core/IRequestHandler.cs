using System.Threading;
using System.Threading.Tasks;

namespace SimpleMediator.Core
{
    public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse> 
    {
        Task<TResponse> HandleAsync(TRequest request, IMediationContext mediationContext,
            CancellationToken cancellationToken);
    }
}