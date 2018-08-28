using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SimpleMediator.Core;
using SimpleMediator.Extensions;
using SimpleMediator.Middleware;

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

            services.AddScoped(typeof(IPipeline<,>), typeof(ProcessPipeline<,>));
            services.AddScoped<IServiceFactory>(s => new Func<Type, object>(s.GetService).ToServiceFactory());
            services.AddScoped<Mediator>();

            using (var container = services.BuildServiceProvider())
            {
                var mediator = container.GetService<Mediator>();
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
