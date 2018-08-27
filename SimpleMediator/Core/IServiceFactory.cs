using System;

namespace SimpleMediator.Core
{
    public interface IServiceFactory
    {
        object GetInstance(Type T);
    }
}
