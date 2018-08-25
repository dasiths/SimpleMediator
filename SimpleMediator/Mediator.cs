using System;
using System.Collections.Generic;
using System.Linq;
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
            var targetType = typeof(IRequest<>).MakeGenericType(typeof(TResponse));
            var instance = _factoryFunc.Invoke(targetType);

            return await (Task<TResponse>)instance.GetType()
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.HandleAsync))
                .Invoke(instance, new[] { request });
        }

        public async Task ExecuteAsync<TRequest>(TRequest request) where TRequest : IRequest
        {
            var targetType = typeof(TRequest);
            var instance = (IRequestHandler<TRequest>)_factoryFunc.Invoke(targetType);

            await (Task)instance.GetType()
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(IRequestHandler<IRequest>.HandleAsync))
                .Invoke(instance, null);
        }
    }
}
