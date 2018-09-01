using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Queries;

namespace SimpleMediator.Samples.MassTransit
{
    public class MyRequestHandler : QueryHandler<SimpleMassTransitMessage, SimpleMassTransitResponse>
    {
        protected override async Task<SimpleMassTransitResponse> HandleQueryAsync(SimpleMassTransitMessage query,
            IMediationContext mediationContext, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Proccessed with mediation context of {mediationContext.GetType().Name}");

            return new SimpleMassTransitResponse()
            {
                Message = query.Message + " received."
            };
        }
    }
}