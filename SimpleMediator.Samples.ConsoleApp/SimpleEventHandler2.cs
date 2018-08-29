using System;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleEventHandler2 : Events.EventHandler<SimpleEvent>
    {
        protected override async Task HandleEventAsync(SimpleEvent @event, IMediationContext mediationContext)
        {
            Console.WriteLine("Event handler 2");
        }
    }
}