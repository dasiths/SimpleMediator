using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Events
{
    public interface IEventHandler<in TEvent>: IRequestHandler<TEvent, Unit> where TEvent : IRequest<Unit>
    {
        Task<Unit> HandleAsync(TEvent request);
    }
}