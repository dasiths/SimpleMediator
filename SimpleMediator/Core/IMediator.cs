using System.Threading;
using System.Threading.Tasks;

namespace SimpleMediator.Core
{
    public interface IMediator
    {
        Task<TResponse> HandleAsync<TResponse>(IRequest<TResponse> request, IMediationContext mediationContext = default(IMediationContext),
            CancellationToken cancellationToken = default(CancellationToken));
    }
}