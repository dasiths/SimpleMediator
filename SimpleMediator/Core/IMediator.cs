using System.Threading;
using System.Threading.Tasks;

namespace SimpleMediator.Core
{
    public interface IMediator
    {
        Task<TResponse> HandleAsync<TResponse>(IRequest<TResponse> request, IMediationContext mediationContext = null,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}