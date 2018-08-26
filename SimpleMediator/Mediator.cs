using System;
using System.Reflection;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator
{
    public class Mediator
    {
        private readonly Func<Type, object> _factoryFunc;

        public Mediator(Func<Type, object> factoryFunc)
        {
            _factoryFunc = factoryFunc;
        }

        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            var targetType = request.GetType();
            var targetHandler = typeof(IRequestHandler<,>).MakeGenericType(targetType, typeof(TResponse));

            var instance = _factoryFunc.Invoke(targetHandler);

            return await (Task<TResponse>)instance.GetType()
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.HandleAsync))
                .Invoke(instance, new object[] { request });
        }

        public async Task SendAsync<TRequest>(TRequest request) where TRequest : IRequest
        {
            var targetType = typeof(TRequest);
            var targetHandler = typeof(IRequestHandler<>).MakeGenericType(targetType);

            var instance = (IRequestHandler<TRequest>)_factoryFunc.Invoke(targetHandler);

            await (Task)instance.GetType()
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(IRequestHandler<IRequest>.HandleAsync))
                .Invoke(instance, new object[] { request });
        }
    }
}
