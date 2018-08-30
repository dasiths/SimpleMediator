using System;
using Autofac;
using MassTransit;
using SimpleMediator.Core;
using SimpleMediator.Samples.ConsoleApp;

namespace SimpleMediator.Samples.MassTransit
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            using (var container = ConsoleApp.Program.CreateAutofacContainer())
            {
                var mediator = container.Resolve<IMediator>();

                var request = new SimpleMassTransitMessage()
                {
                    Message = DateTime.Now.ToString()
                };

                var queueName = typeof(SimpleMassTransitMessage).Name;

                var busControl = Bus.Factory.CreateUsingInMemory(x =>
                 {
                     x.ReceiveEndpoint(queueName, ep =>
                     {
                         ep.Consumer(() =>
                             new MassTransitConnectedConsumer<SimpleMassTransitMessage, SimpleMassTransitResponse>(mediator)
                         );
                     });
                 });

                busControl.Start();

                var context = new MassTransitMediationContext<SimpleMassTransitMessage, SimpleMassTransitResponse>(busControl,
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
