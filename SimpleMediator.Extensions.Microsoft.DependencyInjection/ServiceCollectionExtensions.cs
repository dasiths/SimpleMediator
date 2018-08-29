using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleMediator.Commands;
using SimpleMediator.Core;
using SimpleMediator.Events;
using SimpleMediator.Middleware;
using SimpleMediator.Queries;

namespace SimpleMediator.Extensions.Microsoft.DependencyInjection
{
    /// <summary>
    /// Extensions to scan for Mediator handlers and registers them.
    /// - Scans for any handler interface implementations and registers them as <see cref="ServiceLifetime.Transient"/>
    /// - Scans for any <see cref="IRequestPreProcessor{TRequest}"/> and <see cref="IRequestPostProcessor{TRequest,TResponse}"/> implementations and registers them as scoped instances
    /// Registers <see cref="ServiceFactory"/> and <see cref="IMediator"/> as scoped instances
    /// After calling AddSimpleMediator you can use the container to resolve an <see cref="IMediator"/> instance.
    ///
    /// This class and methods has been *heavily* influnced by __jbogard/MediatR.Extensions.Microsoft.DependencyInjection__
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
            AddRequiredServices(services);

            AddSimpleMediatorClasses(services, assemblies);

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
            AddRequiredServices(services);
            AddSimpleMediatorClasses(services, handlerAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
            return services;
        }

        private static void AddSimpleMediatorClasses(IServiceCollection services, IEnumerable<Assembly> assembliesToScan)
        {
            assembliesToScan = (assembliesToScan as Assembly[] ?? assembliesToScan).Distinct().ToArray();

            var openCommandAndQueryInterfaces = new[]
            {
                typeof(IQueryHandler<,>),
                typeof(ICommandHandler<>)
            };

            var openEventInterfaces = new[]
            {
                typeof(IEventHandler<>),
            };

            AddInterfacesAsTransient(openCommandAndQueryInterfaces, services, assembliesToScan, false);
            AddInterfacesAsTransient(openEventInterfaces, services, assembliesToScan, true);

            var multiOpenInterfaces = new[]
            {
                typeof(IMiddleware<,>)
            };

            foreach (var multiOpenInterface in multiOpenInterfaces)
            {
                var concretions = new List<Type>();

                foreach (var type in assembliesToScan.SelectMany(a => a.DefinedTypes))
                {
                    IEnumerable<Type> interfaceTypes = type.FindInterfacesThatClose(multiOpenInterface).ToArray();
                    if (!interfaceTypes.Any()) continue;

                    if (type.IsConcrete())
                    {
                        concretions.Add(type);
                    }
                }

                // Always add every middleware
                concretions
                    .ForEach(c =>
                    {
                        if (!c.IsGenericType)
                        {
                            IEnumerable<Type> interfaceTypes = c.FindInterfacesThatClose(multiOpenInterface).ToArray();

                            foreach (var type in interfaceTypes)
                            {
                                services.AddTransient(type, c);
                            }
                        }
                        else
                        {
                            services.AddTransient(multiOpenInterface, c);
                        }

                        // This is needed because MS DI doesn't support constrained items,
                        // the service factory method registered in this class catches the argument exception and tries to resolve implemented types
                        services.AddTransient(c);
                    });
            }
        }

        /// <summary>
        /// Helper method use to differentiate behavior between command/query/event handlers.
        /// Command/Query handlers should only be added once (so set addIfAlreadyExists to false)
        /// Event handlers should all be added (set addIfAlreadyExists to true)
        /// </summary>
        /// <param name="openRequestInterfaces"></param>
        /// <param name="services"></param>
        /// <param name="assembliesToScan"></param>
        /// <param name="addIfAlreadyExists"></param>
        private static void AddInterfacesAsTransient(Type[] openRequestInterfaces,
            IServiceCollection services,
            IEnumerable<Assembly> assembliesToScan,
            bool addIfAlreadyExists)
        {
            foreach (var openInterface in openRequestInterfaces)
            {
                var concretions = new List<Type>();
                var interfaces = new List<Type>();

                foreach (var type in assembliesToScan.SelectMany(a => a.DefinedTypes))
                {
                    IEnumerable<Type> interfaceTypes = type.FindInterfacesThatClose(openInterface).ToArray();
                    if (!interfaceTypes.Any()) continue;

                    if (type.IsConcrete())
                    {
                        concretions.Add(type);
                    }

                    foreach (Type interfaceType in interfaceTypes)
                    {
                        if (interfaceType.GetInterfaces().Any())
                        {
                            // Register the RequestHandler instead of ICommand/Query/EventHandler
                            interfaces.AddRange(interfaceType.GetInterfaces());
                        }
                        else
                        {
                            interfaces.Fill(interfaceType);
                        }
                    }
                }

                foreach (var @interface in interfaces.Distinct())
                {
                    var matches = concretions
                        .Where(t => t.CanBeCastTo(@interface))
                        .ToList();

                    if (addIfAlreadyExists)
                    {
                        matches.ForEach(match => services.AddTransient(@interface, match));
                    }
                    else
                    {
                        if (matches.Count() > 1)
                        {
                            matches.RemoveAll(m => !IsMatchingWithInterface(m, @interface));
                        }

                        matches.ForEach(match => services.TryAddTransient(@interface, match));
                    }

                    if (!@interface.IsOpenGeneric())
                    {
                        AddConcretionsThatCouldBeClosed(@interface, concretions, services);
                    }
                }
            }
        }

        private static bool IsMatchingWithInterface(Type handlerType, Type handlerInterface)
        {
            if (handlerType == null || handlerInterface == null)
            {
                return false;
            }

            if (handlerType.IsInterface)
            {
                if (handlerType.GenericTypeArguments.SequenceEqual(handlerInterface.GenericTypeArguments))
                {
                    return true;
                }
            }
            else
            {
                return IsMatchingWithInterface(handlerType.GetInterface(handlerInterface.Name), handlerInterface);
            }

            return false;
        }

        private static void AddConcretionsThatCouldBeClosed(Type @interface, List<Type> concretions, IServiceCollection services)
        {
            foreach (var type in concretions
                .Where(x => x.IsOpenGeneric() && x.CouldCloseTo(@interface)))
            {
                try
                {
                    services.TryAddTransient(@interface, type.MakeGenericType(@interface.GenericTypeArguments));
                }
                catch (Exception)
                {
                }
            }
        }

        private static bool CouldCloseTo(this Type openConcretion, Type closedInterface)
        {
            var openInterface = closedInterface.GetGenericTypeDefinition();
            var arguments = closedInterface.GenericTypeArguments;

            var concreteArguments = openConcretion.GenericTypeArguments;
            return arguments.Length == concreteArguments.Length && openConcretion.CanBeCastTo(openInterface);
        }

        private static bool CanBeCastTo(this Type pluggedType, Type pluginType)
        {
            if (pluggedType == null) return false;

            if (pluggedType == pluginType) return true;

            return pluginType.GetTypeInfo().IsAssignableFrom(pluggedType.GetTypeInfo());
        }

        public static bool IsOpenGeneric(this Type type)
        {
            return type.GetTypeInfo().IsGenericTypeDefinition || type.GetTypeInfo().ContainsGenericParameters;
        }

        private static IEnumerable<Type> FindInterfacesThatClose(this Type pluggedType, Type templateType)
        {
            if (!pluggedType.IsConcrete()) yield break;

            if (templateType.GetTypeInfo().IsInterface)
            {
                foreach (
                    var interfaceType in
                        pluggedType.GetTypeInfo().ImplementedInterfaces
                            .Where(type => type.GetTypeInfo().IsGenericType && (type.GetGenericTypeDefinition() == templateType)))
                {
                    yield return interfaceType;
                }
            }
            else if (pluggedType.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType &&
                     (pluggedType.GetTypeInfo().BaseType.GetGenericTypeDefinition() == templateType))
            {
                yield return pluggedType.GetTypeInfo().BaseType;
            }

            if (pluggedType == typeof(object)) yield break;
            if (pluggedType.GetTypeInfo().BaseType == typeof(object)) yield break;

            foreach (var interfaceType in FindInterfacesThatClose(pluggedType.GetTypeInfo().BaseType, templateType))
            {
                yield return interfaceType;
            }
        }

        private static bool IsConcrete(this Type type)
        {
            return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
        }

        private static void Fill<T>(this IList<T> list, T value)
        {
            if (list.Contains(value)) return;
            list.Add(value);
        }

        private static void AddRequiredServices(IServiceCollection services)
        {
            services.AddScoped<ServiceFactoryDelegate>(p => (type =>
            {
                try
                {
                    return p.GetService(type);
                }
                catch (ArgumentException)
                {
                    // Let's assume it's a constrained generic type
                    if (type.IsConstructedGenericType &&
                        type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        var serviceType = type.GenericTypeArguments.Single();
                        var serviceTypes = new List<Type>();
                        foreach (var service in services)
                        {
                            if (serviceType.IsConstructedGenericType &&
                                serviceType.GetGenericTypeDefinition() == service.ServiceType)
                            {
                                try
                                {
                                    var closedImplType = service.ImplementationType.MakeGenericType(serviceType.GenericTypeArguments);
                                    serviceTypes.Add(closedImplType);
                                }
                                catch { }
                            }
                        }

                        services.Replace(new ServiceDescriptor(type, sp =>
                        {
                            return serviceTypes.Select(sp.GetService).ToArray();
                        }, ServiceLifetime.Transient));

                        var resolved = Array.CreateInstance(serviceType, serviceTypes.Count);

                        Array.Copy(serviceTypes.Select(p.GetService).ToArray(), resolved, serviceTypes.Count);

                        return resolved;
                    }

                    throw;
                }
            }));

            services.AddScoped<IServiceFactory, ServiceFactory>();
            services.AddScoped(typeof(IRequestProcessor<,>), typeof(RequestProcessor<,>));
            services.AddScoped<IMediator, Mediator>();
        }
    }
}
