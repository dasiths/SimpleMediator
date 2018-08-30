using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.MassTransit
{
    public class MassTransitConnectedConsumer<TRequest, TResponse> : IConsumer<TRequest> where TRequest : class, IRequest<TResponse> where TResponse : class
    {
        private readonly IMediator _mediator;

        public MassTransitConnectedConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<TRequest> context)
        {
            context.Respond(await _mediator.HandleAsync(context.Message, MediationContext.Default, CancellationToken.None));
        }
    }
}