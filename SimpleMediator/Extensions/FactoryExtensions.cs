using System;
using System.Collections.Generic;
using System.Text;
using SimpleMediator.Core;

namespace SimpleMediator.Extensions
{
    public static class FactoryExtensions
    {
        public static ServiceFactory ToServiceFactory(this Func<Type, object> factoryFunc)
        {
            return new ServiceFactory(factoryFunc);
        }
    }
}
