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

        public async Task<TResponse> HandleAsync<TResponse>(IRequest<TResponse> request,
            IMediationContext mediationContext = default(MediationContext), CancellationToken cancellationToken = default(CancellationToken))
        {
            if (mediationContext == null)
            {
                mediationContext = MediationContext.Default;
            }

            var targetType = request.GetType();
            var targetHandler = typeof(IRequestProcessor<,>).MakeGenericType(targetType, typeof(TResponse));
            var instance = _serviceFactory.GetInstance(targetHandler);

            var result = InvokeInstance(instance, request, targetHandler, mediationContext, cancellationToken);

            return await result;
        }

        private Task<TResponse> InvokeInstance<TResponse>(object instance, IRequest<TResponse> request, Type targetHandler, 
            IMediationContext mediationContext, CancellationToken cancellationToken)
        {
            var method = instance.GetType()
                .GetTypeInfo()
                .GetMethod(nameof(IRequestProcessor<IRequest<TResponse>, TResponse>.HandleAsync));

            if (method == null)
            {
                throw new ArgumentException($"{instance.GetType().Name} is not a known {targetHandler.Name}",
                    instance.GetType().FullName);
            }

            return (Task<TResponse>) method.Invoke(instance, new object[] {request, mediationContext, cancellationToken});
        }
    }
}
