using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Samples.Shared;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleEventHandler2 : Events.EventHandler<SimpleEvent>
    {
        protected override async Task HandleEventAsync(SimpleEvent @event, IMediationContext mediationContext,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("Event handler 2");
        }
    }
}