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

                mediator.HandleAsync(new SimpleQuery()).ConfigureAwait(false).GetAwaiter().GetResult();

                var busControl = Bus.Factory.CreateUsingInMemory(x =>
                {
                    x.ReceiveEndpoint("test_queue", ep => { ep.Consumer<RequestConsumer>(); });
                });

                busControl.Start();

                var client = CreateRequestClient(busControl);
                var result = client.Request(new SimpleMassTransitMessage()
                {
                    Message = DateTime.Now.ToString()
                }).ConfigureAwait(false).GetAwaiter().GetResult();

                Console.WriteLine(result.Message);

                Console.ReadLine();

                busControl.Stop();
            }
        }

        static IRequestClient<SimpleMassTransitMessage, SimpleMassTransitResponse> CreateRequestClient(IBusControl busControl)
        {
            var serviceAddress = new Uri("loopback://localhost/test_queue");
            var client =
                busControl.CreateRequestClient<SimpleMassTransitMessage, SimpleMassTransitResponse>(serviceAddress, TimeSpan.FromSeconds(10));

            return client;
        }
    }
}
