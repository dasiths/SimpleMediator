using SimpleMediator.Core;

namespace SimpleMediator.Events
{
    public interface IEventHandler<in TEvent>: IMessageHandler<TEvent, Unit> where TEvent : IMessage<Unit>
    {
    }
}