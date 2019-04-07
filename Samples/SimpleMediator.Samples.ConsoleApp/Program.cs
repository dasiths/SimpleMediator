using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using SimpleMediator.Core;
using SimpleMediator.Samples.Shared;
using SimpleMediator.Samples.Shared.Helpers;

namespace SimpleMediator.Samples.ConsoleApp
{

    public class Program
    {
        public static void Main(string[] args)
        {
            RunSample().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task RunSample()
        {
            using (var container = MicrosoftDependencyContainerHelper.CreateServiceCollection())
            {
                var mediator = container.GetService<IMediator>();
                await SendCommands(mediator);
            }

            using (var container = AutofacHelper.CreateAutofacContainer())
            {
                var mediator = container.Resolve<IMediator>();
                await SendCommands(mediator);
            }
        }

        private static async Task SendCommands(IMediator mediator)
        {
            var simpleQuery = new SimpleQuery();
            var simpleCommand = new SimpleCommand();
            var simpleEvent = new SimpleEvent();

            var context = new SimpleMediationContext()
            {
                CurrentTime = DateTimeOffset.Now
            };

            var result = await mediator.HandleAsync(simpleQuery, context);
            Console.WriteLine(result.Message);
            await mediator.HandleAsync(simpleCommand);
            await mediator.HandleAsync(simpleEvent);
            Console.ReadLine();
        }
    }
}
