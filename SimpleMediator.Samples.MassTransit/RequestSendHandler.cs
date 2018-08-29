using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Queries;

namespace SimpleMediator.Samples.MassTransit
{
    public class RequestSendHandler : QueryHandler<SimpleMassTransitMessage, SimpleMassTransitResponse>
    {
        protected override async Task<SimpleMassTransitResponse> HandleQueryAsync(SimpleMassTransitMessage query, IMediationContext mediationContext)
        {
            return new SimpleMassTransitResponse()
            {
                Message = query.Message + " received."
            };
        }
    }
}