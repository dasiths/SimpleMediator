using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMediator.Core
{

    public class ServiceFactory : IServiceFactory
    {
        private readonly Func<Type, object> _factoryFunc;

        public ServiceFactory(Func<Type, object> factoryFunc)
        {
            _factoryFunc = factoryFunc;
        }

        public object GetInstance(Type T)
        {
            return _factoryFunc.Invoke(T);
        }
    }
}
