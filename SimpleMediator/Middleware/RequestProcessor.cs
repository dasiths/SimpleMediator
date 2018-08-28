using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Middleware
{
    public class RequestProcessor<TRequest, TResponse> : IRequestProcessor<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _requestHandlers;

        public RequestProcessor(IEnumerable<IRequestHandler<TRequest, TResponse>> requestHandlers)
        {
            _requestHandlers = requestHandlers;
        }

        public async Task<TResponse> HandleAsync(TRequest request, IServiceFactory serviceFactory)
        {
            var type = typeof(TRequest);

            if(typeof(IEvent).IsAssignableFrom(type))
            {
                var tasks = _requestHandlers.Select(r => r.HandleAsync(request));
                var results = await Task.WhenAll(tasks);

                return results.First();
            }

            if (typeof(IQuery<TResponse>).IsAssignableFrom(type) || typeof(ICommand).IsAssignableFrom(type))
            {
                return await _requestHandlers.Single().HandleAsync(request);
            }

            throw new ArgumentException($"{typeof(TRequest).Name} is not a known type of {typeof(IRequest<>).Name} - Query, Command or Event",
                typeof(TRequest).FullName);
        }
    }
}