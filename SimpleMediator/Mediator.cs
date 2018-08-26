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

        public async Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> request)
        {
            var targetType = request.GetType();
            var targetHandler = typeof(IRequestHandler<,>).MakeGenericType(targetType, typeof(TResponse));

            var instance = _factoryFunc.Invoke(targetHandler);

            var method = instance.GetType()
                .GetTypeInfo()
                .GetMethod(nameof(IRequestHandler<IQuery<TResponse>, TResponse>.HandleAsync));

            if (method == null)
            {
                throw new ArgumentException($"{instance.GetType().Name} is not a known {targetHandler.Name}", instance.GetType().FullName);
            }

            return await (Task<TResponse>)method.Invoke(instance, new object[] { request });
        }
    }
}
