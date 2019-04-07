using System;
using MassTransit;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.MassTransit
{
    public class MassTransitSendMediationContext<TMessage, TResponse> : IMediationContext where TMessage : class where TResponse : class
    {
        public readonly IRequestClient<TMessage, TResponse> Client;

        public MassTransitSendMediationContext(IRequestClient<TMessage, TResponse> client)
        {
            Client = client;
        }

        public MassTransitSendMediationContext(IBusControl busControl, Uri serviceAddress, TimeSpan timeout)
        {
            Client = busControl.CreateRequestClient<TMessage, TResponse>(serviceAddress, timeout);
        }
    }
}