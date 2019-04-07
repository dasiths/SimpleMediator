using System;
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
    public class MessageProcessor<TMessage, TResponse> : IMessageProcessor<TMessage, TResponse> where TMessage : IMessage<TResponse>
    {
        private readonly IEnumerable<IMessageHandler<TMessage, TResponse>> _messageHandlers;
        private readonly IEnumerable<IMiddleware<TMessage, TResponse>> _middlewares;

        public MessageProcessor(IServiceFactory serviceFactory)
        {
            _messageHandlers = (IEnumerable<IMessageHandler<TMessage, TResponse>>)
                serviceFactory.GetInstance(typeof(IEnumerable<IMessageHandler<TMessage, TResponse>>));

            _middlewares = (IEnumerable<IMiddleware<TMessage, TResponse>>)
                serviceFactory.GetInstance(typeof(IEnumerable<IMiddleware<TMessage, TResponse>>));
        }

        public Task<TResponse> HandleAsync(TMessage message, IMediationContext mediationContext,
            CancellationToken cancellationToken)
        {
            return RunMiddleware(message, HandleMessageAsync, mediationContext, cancellationToken);
        }

        private async Task<TResponse> HandleMessageAsync(TMessage messageObject, IMediationContext mediationContext, CancellationToken cancellationToken)
        {
            var type = typeof(TMessage);

            if (!_messageHandlers.Any())
            {
                throw new ArgumentException($"No handler of signature {typeof(IMessageHandler<,>).Name} was found for {typeof(TMessage).Name}", typeof(TMessage).FullName);
            }

            if (typeof(IEvent).IsAssignableFrom(type))
            {
                var tasks = _messageHandlers.Select(r => r.HandleAsync(messageObject, mediationContext, cancellationToken));
                var result = default(TResponse);

                foreach (var task in tasks)
                {
                    result = await task;
                }

                return result;
            }

            if (typeof(IQuery<TResponse>).IsAssignableFrom(type) || typeof(ICommand).IsAssignableFrom(type))
            {
                return await _messageHandlers.Single().HandleAsync(messageObject, mediationContext, cancellationToken);
            }

            throw new ArgumentException($"{typeof(TMessage).Name} is not a known type of {typeof(IMessage<>).Name} - Query, Command or Event", typeof(TMessage).FullName);
        }

        private Task<TResponse> RunMiddleware(TMessage message, HandleMessageDelegate<TMessage, TResponse> handleMessageHandlerCall, 
            IMediationContext mediationContext, CancellationToken cancellationToken)
        {
            HandleMessageDelegate<TMessage, TResponse> next = null;

            next = _middlewares.Reverse().Aggregate(handleMessageHandlerCall, (messageDelegate, middleware) =>
                ((req, ctx, ct) => middleware.RunAsync(req, ctx, ct, messageDelegate)));

            return next.Invoke(message, mediationContext, cancellationToken);
        }
    }
}