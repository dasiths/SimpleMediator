using System;
using Autofac;
using MassTransit;
using SimpleMediator.Core;
using SimpleMediator.Samples.Shared.Helpers;

namespace SimpleMediator.Samples.MassTransit
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var request = new SimpleMassTransitMessage()
            {
                Message = DateTime.Now.ToString()
            };

            var queueName = typeof(SimpleMassTransitMessage).Name;

            using (var container = AutofacHelper.CreateAutofacContainer())
            {
                var mediator = container.Resolve<IMediator>();

                var busControl = Bus.Factory.CreateUsingInMemory(x =>
                 {
                     x.ReceiveEndpoint(queueName, ep =>
                     {
                         ep.Consumer(() =>
                             new MassTransitMediatedConsumer<SimpleMassTransitMessage, SimpleMassTransitResponse>(mediator)
                         );
                     });
                 });

                busControl.Start();

                var context = new MassTransitSendMediationContext<SimpleMassTransitMessage, SimpleMassTransitResponse>(busControl,
                    new Uri($"loopback://localhost/{queueName}"),
                    TimeSpan.FromSeconds(10));

                var result = mediator.HandleAsync(request, context).ConfigureAwait(false).GetAwaiter().GetResult();

                Console.WriteLine(result.Message);

                Console.ReadLine();

                busControl.Stop();
            }
        }
    }
}
