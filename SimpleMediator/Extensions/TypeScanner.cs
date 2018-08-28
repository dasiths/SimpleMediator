using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleMediator.Core;
using SimpleMediator.Queries;

namespace SimpleMediator.Extensions
{
    public static class TypeScanner
    {
        public static IList<Tuple<Type, Type>> GetRequestHandlers(this Assembly assembly)
        {
            var registrations = new List<Tuple<Type, Type>>();

            registrations.AddRange(GetRequestHandlerTypes(assembly));
            return registrations;
        }

        private static IList<Tuple<Type, Type>> GetRequestHandlerTypes(this Assembly assembly)
        {
            var queryHandlers = GetAllTypesImplementingOpenGenericType(typeof(IRequestHandler<,>), assembly);

            return queryHandlers.Where(t => !t.IsAbstract && !t.IsGenericType).Select(t =>
            {
                var method = t.GetMethod(nameof(IRequestHandler<IQuery<object>, object>.HandleAsync));

                if (method == null || method.ReturnType.GetGenericArguments().Length != 1 || method.GetParameters().Length != 1)
                {
                    throw new ArgumentException($"{t.Name} is not a known {typeof(IRequestHandler<,>).Name}", t.AssemblyQualifiedName);
                }

                var requestType = method.GetParameters().First().ParameterType;
                var responseType = method.ReturnType.GetGenericArguments()[0];

                var genericHandlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
                return new Tuple<Type, Type>(genericHandlerType, t);
            }).ToList();
        }

        private static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly assembly)
        {
            return from x in assembly.GetTypes()
                   from z in x.GetInterfaces()
                   let y = x.BaseType
                   where
                       (y != null && y.IsGenericType &&
                        openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
                       (z.IsGenericType &&
                        openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
                   select x;
        }
    }
}
