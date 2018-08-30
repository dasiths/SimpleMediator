﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public RequestProcessor(IServiceFactory serviceFactory)
        {
            _requestHandlers = (IEnumerable<IRequestHandler<TRequest, TResponse>>)
                serviceFactory.GetInstance(typeof(IEnumerable<IRequestHandler<TRequest, TResponse>>));

            _middlewares = (IEnumerable<IMiddleware<TRequest, TResponse>>)
                serviceFactory.GetInstance(typeof(IEnumerable<IMiddleware<TRequest, TResponse>>));
        }

        public async Task<TResponse> HandleAsync(TRequest request, IMediationContext mediationContext,
            CancellationToken cancellationToken)
        {
            return await RunMiddleware(request, HandleRequest, mediationContext, cancellationToken);
        }

        private async Task<TResponse> HandleRequest(TRequest requestObject, IMediationContext mediationContext, CancellationToken cancellationToken)
        {
            var type = typeof(TRequest);

            if (!_requestHandlers.Any())
            {
                throw new ArgumentException($"No handler of signature {typeof(IRequestHandler<,>).Name} was found for {typeof(TRequest).Name}", typeof(TRequest).FullName);
            }

            if (typeof(IEvent).IsAssignableFrom(type))
            {
                var tasks = _requestHandlers.Select(r => r.HandleAsync(requestObject, mediationContext, cancellationToken));
                var results = await Task.WhenAll(tasks);

                return results.First();
            }

            if (typeof(IQuery<TResponse>).IsAssignableFrom(type) || typeof(ICommand).IsAssignableFrom(type))
            {
                return await _requestHandlers.Single().HandleAsync(requestObject, mediationContext, cancellationToken);
            }

            throw new ArgumentException($"{typeof(TRequest).Name} is not a known type of {typeof(IRequest<>).Name} - Query, Command or Event", typeof(TRequest).FullName);
        }

        private async Task<TResponse> RunMiddleware(TRequest request, HandleRequestDelegate<TRequest, TResponse> handleRequestHandlerCall, 
            IMediationContext mediationContext, CancellationToken cancellationToken)
        {
            HandleRequestDelegate<TRequest, TResponse> next = null;

            next = _middlewares.Reverse().Aggregate(handleRequestHandlerCall, (requestDelegate, middleware) =>
                ((req, ctx, ct) => middleware.RunAsync(req, ctx, ct, requestDelegate)));

            return await next.Invoke(request, mediationContext, cancellationToken);
        }
    }
}