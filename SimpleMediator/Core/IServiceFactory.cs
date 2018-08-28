using System;

namespace SimpleMediator.Core
{
    public delegate object ServiceFactoryDelegate(Type type);

    public interface IServiceFactory
    {
        object GetInstance(Type T);
    }
}
