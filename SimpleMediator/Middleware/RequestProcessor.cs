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
        private readonly IEnumerable<IMiddleware<TRequest, TResponse>> _requestFilters;

        public RequestProcessor(IEnumerable<IRequestHandler<TRequest, TResponse>> requestHandlers,
            IEnumerable<IMiddleware<TRequest, TResponse>> requestFilters)
        {
            _requestHandlers = requestHandlers;
            _requestFilters = requestFilters;
        }

        public async Task<TResponse> HandleAsync(TRequest request, IServiceFactory serviceFactory)
        {
            var type = typeof(TRequest);

            async Task<TResponse> RequestHandlerCall(TRequest requestObject)
            {
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

            return await CallRequestFilters(request, RequestHandlerCall);
        }

        private async Task<TResponse> CallRequestFilters(TRequest request, RequestFilterDelegate<TRequest, TResponse> requestHandlerCall)
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