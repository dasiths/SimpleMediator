using System;
using System.Linq;
using System.Reflection;
using Autofac;
using SimpleMediator.Core;
using SimpleMediator.Middleware;

namespace SimpleMediator.Samples.Shared.Helpers
{
    public class AutofacHelper
    {
        public static IContainer CreateAutofacContainer()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic);
            var builder = new ContainerBuilder();

            foreach (var assembly in assemblies)
            {
                builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IMessageHandler<,>)).AsImplementedInterfaces();

                AddMiddleware(assembly, builder);
            }

            builder.Register<ServiceFactoryDelegate>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return context.Resolve;
            });
            builder.RegisterType<ServiceFactory>().AsImplementedInterfaces();
            builder.RegisterType<Mediator>().AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(MessageProcessor<,>)).AsImplementedInterfaces();

            return builder.Build();
        }

        private static void AddMiddleware(Assembly assembly, ContainerBuilder builder)
        {
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
    }
}
