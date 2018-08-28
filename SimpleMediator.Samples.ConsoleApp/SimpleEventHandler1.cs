using System;
using System.Threading.Tasks;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleEventHandler1 : Events.EventHandler<SimpleEvent>
    {
        protected override async Task HandleEventAsync(SimpleEvent @event)
        {
            Console.WriteLine("Event handler 1");
        }
    }
}