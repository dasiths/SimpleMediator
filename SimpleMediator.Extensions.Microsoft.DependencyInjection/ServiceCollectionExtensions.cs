using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SimpleMediator.Core;

namespace SimpleMediator.Extensions.Microsoft.DependencyInjection
{
    /// <summary>
    /// Extensions to scan for Mediator handlers and registers them.
    /// - Scans for any handler interface implementations and registers them as <see cref="ServiceLifetime.Transient"/>
    /// - Scans for any <see cref="IMessagePreProcessor{TMessage}"/> and <see cref="IMessagePostProcessor{TMessage,TResponse}"/> implementations and registers them as scoped instances
    /// Registers <see cref="ServiceFactory"/> and <see cref="IMediator"/> as scoped instances
    /// After calling AddSimpleMediator you can use the container to resolve an <see cref="IMediator"/> instance.
    ///
    /// This class and methods has been *heavily* influenced by __jbogard/MediatR.Extensions.Microsoft.DependencyInjection__
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Registers handlers and the mediator types from <see cref="AppDomain.CurrentDomain"/>.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddSimpleMediator(this IServiceCollection services)
            => services.AddSimpleMediator(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic));

        /// <summary>
        /// Registers handlers and mediator types from the specified assemblies
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddSimpleMediator(this IServiceCollection services, params Assembly[] assemblies)
            => services.AddSimpleMediator(assemblies.AsEnumerable());

        /// <summary>
        /// Registers handlers and mediator types from the specified assemblies
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddSimpleMediator(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            ReflectionUtilities.AddRequiredServices(services);

            ReflectionUtilities.AddSimpleMediatorClasses(services, assemblies);

            return services;
        }

        /// <summary>
        /// Registers handlers and mediator types from the assemblies that contain the specified types
        /// </summary>
        /// <param name="services"></param>
        /// <param name="handlerAssemblyMarkerTypes"></param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddSimpleMediator(this IServiceCollection services, params Type[] handlerAssemblyMarkerTypes)
            => services.AddSimpleMediator(handlerAssemblyMarkerTypes.AsEnumerable());

        /// <summary>
        /// Registers handlers and mediator types from the assemblies that contain the specified types
        /// </summary>
        /// <param name="services"></param>
        /// <param name="handlerAssemblyMarkerTypes"></param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddSimpleMediator(this IServiceCollection services, IEnumerable<Type> handlerAssemblyMarkerTypes)
        {
            ReflectionUtilities.AddRequiredServices(services);
            ReflectionUtilities.AddSimpleMediatorClasses(services, handlerAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
            return services;
        }

        public static IServiceCollection AddSimpleMediatorMiddleware(this IServiceCollection services) =>
            services.AddSimpleMediatorMiddleware(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic));

        public static IServiceCollection AddSimpleMediatorMiddleware(this IServiceCollection services, params Assembly[] assemblies)
            => services.AddSimpleMediatorMiddleware(assemblies.AsEnumerable());

        public static IServiceCollection AddSimpleMediatorMiddleware(this IServiceCollection services,
            IEnumerable<Assembly> assembliesToScan)
        {
            return ReflectionUtilities.AddSimpleMediatorMiddleware(services, assembliesToScan);
        }

    }
}
