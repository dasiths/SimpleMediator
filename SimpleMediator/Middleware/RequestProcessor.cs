using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleMediator.Commands;
using SimpleMediator.Core;
using SimpleMediator.Events;
using SimpleMediator.Queries;

namespace SimpleMediator.Middleware
{
    public class RequestProcessor<TRequest, TResponse> : IRequestProcessor<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _requestHandlers;
        private readonly IEnumerable<IMiddleware<TRequest, TResponse>> _middlewares;

        public RequestProcessor(IEnumerable<IRequestHandler<TRequest, TResponse>> requestHandlers,
            IEnumerable<IMiddleware<TRequest, TResponse>> middlewares)
        {
            _requestHandlers = requestHandlers;
            _middlewares = middlewares;
        }

        public async Task<TResponse> HandleAsync(TRequest request)
        {
            return await RunMiddleware(request, HandleRequest);
        }

        private async Task<TResponse> HandleRequest(TRequest requestObject)
        {
            var type = typeof(TRequest);

            if (typeof(IEvent).IsAssignableFrom(type))
            {
                var tasks = _requestHandlers.Select(r => r.HandleAsync(requestObject));
                var results = await Task.WhenAll(tasks);

                return results.First();
            }

            if (typeof(IQuery<TResponse>).IsAssignableFrom(type) || typeof(ICommand).IsAssignableFrom(type))
            {
                return await _requestHandlers.Single().HandleAsync(requestObject);
            }

            throw new ArgumentException($"{typeof(TRequest).Name} is not a known type of {typeof(IRequest<>).Name} - Query, Command or Event", typeof(TRequest).FullName);
        }

        private async Task<TResponse> RunMiddleware(TRequest request, RequestFilterDelegate<TRequest, TResponse> requestHandlerCall)
        {
            RequestFilterDelegate<TRequest, TResponse> next = requestHandlerCall;

            foreach (var requestFilter in _requestFilters.Reverse())
            {
                var nextFunc = next;
                var chainedFunc = new RequestFilterDelegate<TRequest, TResponse>(req => requestFilter.RunAsync(request, nextFunc));
                next = chainedFunc;
            }

            return await next.Invoke(request);
        }
    }
}