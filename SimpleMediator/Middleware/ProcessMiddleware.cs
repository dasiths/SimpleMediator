using System;
using System.Reflection;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Middleware
{
    public class ProcessMiddleware<TResponse> : IMiddleware<IRequest<TResponse>, TResponse>
    {
        private readonly IServiceFactory _serviceFactory;

        public ProcessMiddleware(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public async Task<TResponse> HandleAsync(IRequest<TResponse> request, RequestHandlerDelegate<TResponse> next)
        {
            var targetType = request.GetType();
            var targetHandler = typeof(IRequestHandler<,>).MakeGenericType(targetType, typeof(TResponse));

            var instance = _serviceFactory.GetInstance(targetHandler);
            var method = InvokeInstance(instance, request, targetHandler);

            return await method;
        }

        private Task<TResponse> InvokeInstance(object instance, IRequest<TResponse> request, Type targetHandler)
        {
            var method = instance.GetType()
                .GetTypeInfo()
                .GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.HandleAsync));

            if (method == null)
            {
                throw new ArgumentException($"{instance.GetType().Name} is not a known {targetHandler.Name}",
                    instance.GetType().FullName);
            }

            return (Task<TResponse>)method.Invoke(instance, new object[] { request });
        }
    }
}