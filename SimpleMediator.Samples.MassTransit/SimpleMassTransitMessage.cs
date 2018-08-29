using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MassTransit;

namespace SimpleMediator.Samples.MassTransit
{
    public class SimpleMassTransitMessage
    {
        public string Message { get; set; }
    }

    public class SimpleMassTransitResponse
    {
        public string Message { get; set; }
    }

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
