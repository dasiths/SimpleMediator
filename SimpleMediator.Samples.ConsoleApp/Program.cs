using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using SimpleMediator.Core;
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
                await SendCommands(mediator);
            }

            using (var container = CreateAutofacContainer())
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

        public static ServiceProvider CreateServiceCollection()
        {
            var services = new ServiceCollection();
            services.AddSimpleMediator();

            return services.BuildServiceProvider();
        }

        public static IContainer CreateAutofacContainer()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic);
            var builder = new ContainerBuilder();

            foreach (var assembly in assemblies)
            {
                builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IRequestHandler<,>)).AsImplementedInterfaces();

                var middlewareTypes = assembly.GetTypes().Where(t =>
                {
                    return t.GetTypeInfo()
                        .ImplementedInterfaces.Any(
                            i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMiddleware<,>));
                });

                foreach (var middlewareType in middlewareTypes)
                {
                    if (middlewareType.IsGenericType)
                    {
                        builder.RegisterGeneric(middlewareType).AsImplementedInterfaces();
                    }
                    else
                    {
                        builder.RegisterType(middlewareType).AsImplementedInterfaces();
                    }
                }
            }

            builder.Register<ServiceFactoryDelegate>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return context.Resolve;
            });
            builder.RegisterType<ServiceFactory>().AsImplementedInterfaces();
            builder.RegisterType<Mediator>().AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(RequestProcessor<,>)).AsImplementedInterfaces();

            return builder.Build();
        }
    }
}
