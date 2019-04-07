using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Middleware;

namespace SimpleMediator.Core
{
    public class Mediator : IMediator
    {
        private readonly IServiceFactory _serviceFactory;

        public Mediator(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public Task<TResponse> HandleAsync<TResponse>(IMessage<TResponse> message,
            IMediationContext mediationContext = default(MediationContext), CancellationToken cancellationToken = default(CancellationToken))
        {
            if (mediationContext == null)
            {
                mediationContext = MediationContext.Default;
            }

            var targetType = message.GetType();
            var targetHandler = typeof(IMessageProcessor<,>).MakeGenericType(targetType, typeof(TResponse));
            var instance = _serviceFactory.GetInstance(targetHandler);

            var result = InvokeInstanceAsync(instance, message, targetHandler, mediationContext, cancellationToken);

            return result;
        }

        private Task<TResponse> InvokeInstanceAsync<TResponse>(object instance, IMessage<TResponse> message, Type targetHandler, 
            IMediationContext mediationContext, CancellationToken cancellationToken)
        {
            var method = instance.GetType()
                .GetTypeInfo()
                .GetMethod(nameof(IMessageProcessor<IMessage<TResponse>, TResponse>.HandleAsync));

            if (method == null)
            {
                throw new ArgumentException($"{instance.GetType().Name} is not a known {targetHandler.Name}",
                    instance.GetType().FullName);
            }

            return (Task<TResponse>) method.Invoke(instance, new object[] {message, mediationContext, cancellationToken});
        }
    }
}
