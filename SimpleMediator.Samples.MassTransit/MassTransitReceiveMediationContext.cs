using SimpleMediator.Core;

namespace SimpleMediator.Samples.MassTransit
{
    public class MassTransitReceiveMediationContext<TRequest, TResponse> : IMediationContext where TRequest : class where TResponse : class
    {
        public static MassTransitReceiveMediationContext<TRequest, TResponse> Default()
        {
            return new MassTransitReceiveMediationContext<TRequest, TResponse>();
        }
    }
}