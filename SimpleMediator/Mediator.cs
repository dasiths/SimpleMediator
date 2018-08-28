using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Middleware;

namespace SimpleMediator
{
    public class Mediator
    {
        private readonly IServiceFactory _serviceFactory;

        public Mediator(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            var targetType = request.GetType();
            var targetHandler = typeof(IRequestProcessor<,>).MakeGenericType(targetType, typeof(TResponse));
            var instance = _serviceFactory.GetInstance(targetHandler);

            var method = InvokeInstance(instance, request, targetHandler);

            return await method;
        }

        private Task<TResponse> InvokeInstance<TResponse>(object instance, IRequest<TResponse> request, Type targetHandler)
        {
            var method = instance.GetType()
                .GetTypeInfo()
                .GetMethod(nameof(IRequestProcessor<IRequest<TResponse>, TResponse>.HandleAsync));

            if (method == null)
            {
                throw new ArgumentException($"{instance.GetType().Name} is not a known {targetHandler.Name}",
                    instance.GetType().FullName);
            }

            return (Task<TResponse>) method.Invoke(instance, new object[] {request, _serviceFactory});
        }
    }
}
