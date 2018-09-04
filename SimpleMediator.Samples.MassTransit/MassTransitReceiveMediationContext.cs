using MassTransit;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.MassTransit
{
    public class MassTransitReceiveMediationContext<TRequest, TResponse> : IMediationContext where TRequest : class where TResponse : class
    {
        public readonly ConsumeContext<TRequest> ConsumeContext;
        public bool IsHandled { get; set; }

        public MassTransitReceiveMediationContext(ConsumeContext<TRequest> consumeContext)
        {
            ConsumeContext = consumeContext;
            IsHandled = false;
        }
    }
}