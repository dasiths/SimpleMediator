using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SimpleMediator.Core;
using SimpleMediator.Extensions;
using SimpleMediator.Extensions.Microsoft.DependencyInjection;
using SimpleMediator.Middleware;

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
            using (var container = CreateServiceCollection())
            {
                var mediator = container.GetService<IMediator>();
                var simpleQuery = new SimpleQuery();
                var simpleCommand = new SimpleCommand();
                var simpleEvent = new SimpleEvent();

                var context = new SimpleMediationContext()
                {
                    CurrentTime = DateTimeOffset.Now
                };

                var result = await mediator.SendAsync(simpleQuery, context);
                Console.WriteLine(result.Message);
                await mediator.SendAsync(simpleCommand);
                await mediator.SendAsync(simpleEvent);
                Console.ReadLine();
            }
        }

        private static ServiceProvider CreateServiceCollection()
        {
            var services = new ServiceCollection();

            /* If doing manually
            services.AddScoped(typeof(IRequestProcessor<,>), typeof(RequestProcessor<,>));
            services.AddScoped<ServiceFactoryDelegate>(s => s.GetService);
            services.AddScoped<IServiceFactory, ServiceFactory>();
            services.AddScoped<IMediator, Mediator>();

            foreach (var requestHandler in Assembly.GetEntryAssembly().GetRequestHandlers())
            {
                services.AddTransient(requestHandler.Item1, requestHandler.Item2);
            }

            services.AddTransient(typeof(IMiddleware<,>), typeof(LoggerMiddleware1<,>));
            services.AddTransient(typeof(IMiddleware<,>), typeof(LoggerMiddleware2<,>));
            */

            services.AddSimpleMediator();

            return services.BuildServiceProvider();
        }
    }
}
