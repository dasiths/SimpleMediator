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
            using (var container = CreateServiceCollection())
            {
                var mediator = container.GetService<IMediator>();
                var simpleQuery = new SimpleQuery();
                var simpleCommand = new SimpleCommand();
                var simpleEvent = new SimpleEvent();

                var result = await mediator.SendAsync(simpleQuery);
                Console.WriteLine(result.Message);
                await mediator.SendAsync(simpleCommand);
                await mediator.SendAsync(simpleEvent);
                Console.ReadLine();
            }
        }

        private static ServiceProvider CreateServiceCollection()
        {
            var services = new ServiceCollection();

            foreach (var requestHandler in Assembly.GetEntryAssembly().GetRequestHandlers())
            {
                services.AddSingleton(requestHandler.Item1, requestHandler.Item2);
            }

            services.AddScoped(typeof(IRequestProcessor<,>), typeof(RequestProcessor<,>));
            services.AddScoped(typeof(IMiddleware<,>), typeof(LoggerMiddleware1<,>));
            services.AddScoped(typeof(IMiddleware<,>), typeof(LoggerMiddleware2<,>));
            services.AddScoped<ServiceFactoryDelegate>(s => s.GetService);
            services.AddScoped<IServiceFactory, ServiceFactory>();
            services.AddScoped<IMediator, Mediator>();
            return services.BuildServiceProvider();
        }
    }
}
