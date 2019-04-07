using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using SimpleMediator.Core;
using SimpleMediator.Queries;

namespace SimpleMediator.Samples.MassTransit
{
    public class MyMessageHandler : QueryHandler<SimpleMassTransitMessage, SimpleMassTransitResponse>
    {
        protected override async Task<SimpleMassTransitResponse> HandleQueryAsync(SimpleMassTransitMessage query,
            IMediationContext mediationContext, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Proccessed with mediation context of {mediationContext.GetType().Name}");

            var response = new SimpleMassTransitResponse()
            {
                Message = query.Message + " received."
            };

            // Or you can respond using the context directly
            //if (mediationContext is MassTransitReceiveMediationContext<SimpleMassTransitMessage, SimpleMassTransitResponse> context)
            //{
            //    await context.ConsumeContext.RespondAsync(response);
            //    context.IsHandled = true;
            //}

            return response;
        }
    }
}