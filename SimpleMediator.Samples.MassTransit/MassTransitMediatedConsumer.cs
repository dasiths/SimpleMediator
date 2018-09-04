using System.Threading.Tasks;
using MassTransit;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.MassTransit
{
    public class MassTransitMediatedConsumer<TRequest, TResponse> : IConsumer<TRequest> where TRequest : class, IRequest<TResponse> where TResponse : class
    {
        private readonly IMediator _mediator;

        public MassTransitMediatedConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<TRequest> context)
        {
            var mediationContext = new MassTransitReceiveMediationContext<TRequest, TResponse>(context);
            var result = await _mediator.HandleAsync(context.Message, mediationContext);

            if (!mediationContext.IsHandled)
            {
                context.Respond(result);
            }
        }
    }
}