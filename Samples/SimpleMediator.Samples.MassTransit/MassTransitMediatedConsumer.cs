using System.Threading.Tasks;
using MassTransit;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.MassTransit
{
    public class MassTransitMediatedConsumer<TMessage, TResponse> : IConsumer<TMessage> where TMessage : class, IMessage<TResponse> where TResponse : class
    {
        private readonly IMediator _mediator;

        public MassTransitMediatedConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var mediationContext = new MassTransitReceiveMediationContext<TMessage, TResponse>(context);
            var result = await _mediator.HandleAsync(context.Message, mediationContext);

            if (!mediationContext.IsHandled)
            {
                context.Respond(result);
            }
        }
    }
}