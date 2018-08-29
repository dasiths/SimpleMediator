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

                var busControl = Bus.Factory.CreateUsingInMemory(x =>
                 {
                     x.ReceiveEndpoint("test_queue", ep => { ep.Consumer<RequestConsumer>(); });
                 });

                busControl.Start();

                var request = new SimpleMassTransitMessage()
                {
                    Message = DateTime.Now.ToString()
                };

                var client = CreateRequestClient(busControl);
                var result = client.Request(request).ConfigureAwait(false).GetAwaiter().GetResult();

                Console.WriteLine(result.Message);

                var response = mediator.HandleAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
                Console.WriteLine(response.Message);

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
