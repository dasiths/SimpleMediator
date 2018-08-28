using System;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleEvent: IEvent
    {
    }

    public class SimpleEventHandler1 : Core.EventHandler<SimpleEvent>
    {
        protected override async Task HandleEventAsync(SimpleEvent @event)
        {
            Console.WriteLine("Event handler 1");
        }
    }

    public class SimpleEventHandler2 : Core.EventHandler<SimpleEvent>
    {
        protected override async Task HandleEventAsync(SimpleEvent @event)
        {
            Console.WriteLine("Event handler 1");
        }
    }
}
