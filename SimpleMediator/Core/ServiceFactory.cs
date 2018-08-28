using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMediator.Core
{

    public class ServiceFactory : IServiceFactory
    {
        private readonly ServiceFactoryDelegate _serviceFactoryDelegate;

        public ServiceFactory(ServiceFactoryDelegate serviceFactoryDelegate)
        {
            _serviceFactoryDelegate = serviceFactoryDelegate;
        }

        public object GetInstance(Type T)
        {
            return _serviceFactoryDelegate.Invoke(T);
        }
    }
}
