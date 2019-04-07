using MassTransit;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.MassTransit
{
    public class MassTransitReceiveMediationContext<TMessage, TResponse> : IMediationContext where TMessage : class where TResponse : class
    {
        public readonly ConsumeContext<TMessage> ConsumeContext;
        public bool IsHandled { get; set; }

        public MassTransitReceiveMediationContext(ConsumeContext<TMessage> consumeContext)
        {
            ConsumeContext = consumeContext;
            IsHandled = false;
        }
    }
}