using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            //services.AddSingleton(typeof(IRequestHandler<IRequest<SimpleResponse>, SimpleResponse>), typeof(SimpleRequestHandler));

            foreach (var requestHandler in Assembly.GetEntryAssembly().GetRequestHandlers())
            {
                services.AddSingleton(requestHandler.Item1, requestHandler.Item2);
            }

            using (var container = services.BuildServiceProvider())
            {
                Func<Type, object> factory = (t) => container.GetService(t);

                var mediator = new Mediator(factory);
                var simpleRequest = new SimpleRequest();
                var simpleCommand = new SimpleCommand();

                var result = await mediator.SendAsync<SimpleResponse>(simpleRequest);
                Console.WriteLine(result.Message);
                await mediator.SendAsync(simpleCommand);
                Console.ReadLine();
            }
        }
    }
}
