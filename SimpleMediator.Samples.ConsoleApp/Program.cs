using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SimpleMediator.Extensions;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            foreach (var requestHandler in Assembly.GetEntryAssembly().GetRequestHandlers())
            {
                services.AddSingleton(requestHandler.Item1, requestHandler.Item2);
            }

            using (var container = services.BuildServiceProvider())
            {
                var func = new Func<Type, object>(container.GetService);
                var mediator = new Mediator(func.ToServiceFactory());
                var simpleQuery = new SimpleQuery();
                var simpleCommand = new SimpleCommand();

                var result = await mediator.SendAsync(simpleQuery);
                Console.WriteLine(result.Message);
                await mediator.SendAsync(simpleCommand);
                Console.ReadLine();
            }
        }
    }
}
