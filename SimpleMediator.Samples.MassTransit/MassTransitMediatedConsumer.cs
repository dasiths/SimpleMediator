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
            context.Respond(await _mediator.HandleAsync(context.Message, MassTransitReceiveMediationContext<TRequest, TResponse>.Default()));
        }
    }
}