using System.Threading.Tasks;
using MassTransit;

namespace SimpleMediator.Samples.MassTransit
{
    public class RequestConsumer : IConsumer<SimpleMassTransitMessage>
    {
        public async Task Consume(ConsumeContext<SimpleMassTransitMessage> context)
        {
            context.Respond(new SimpleMassTransitResponse()
            {
                Message = context.Message.Message + " received."
            });
        }
    }
}