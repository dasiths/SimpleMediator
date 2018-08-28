using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Middleware
{
    public class ProcessPipeline<TRequest, TResponse> : IPipeline<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _requestHandlers;

        public ProcessPipeline(IEnumerable<IRequestHandler<TRequest, TResponse>> requestHandlers)
        {
            _requestHandlers = requestHandlers;
        }

        public async Task<TResponse> HandleAsync(TRequest request, IServiceFactory serviceFactory)
        {
            var type = typeof(TRequest);

            if (typeof(IQuery<TResponse>).IsAssignableFrom(type) || typeof(ICommand).IsAssignableFrom(type))
            {
                return await _requestHandlers.First().HandleAsync(request);
            }
            else if(typeof(IEvent).IsAssignableFrom(type))
            {
                var tasks = _requestHandlers.Select(r => r.HandleAsync(request));
                var results = await Task.WhenAll(tasks);

                return results.First();
            }

            throw new ArgumentException();
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