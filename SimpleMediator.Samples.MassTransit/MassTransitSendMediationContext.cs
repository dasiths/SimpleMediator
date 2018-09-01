using System;
using MassTransit;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.MassTransit
{
    public class MassTransitSendMediationContext<TRequest, TResponse> : IMediationContext where TRequest : class where TResponse : class
    {
        public readonly IRequestClient<TRequest, TResponse> Client;

        public MassTransitSendMediationContext(IRequestClient<TRequest, TResponse> client)
        {
            Client = client;
        }

        public MassTransitSendMediationContext(IBusControl busControl, Uri serviceAddress, TimeSpan timeout)
        {
            Client = busControl.CreateRequestClient<TRequest, TResponse>(serviceAddress, timeout);
        }
    }
}