using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Commands;
using SimpleMediator.Core;
using SimpleMediator.Samples.Shared;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleCommandHandler : CommandHandler<SimpleCommand>
    {
        protected override async Task HandleCommandAsync(SimpleCommand message, IMediationContext mediationContext,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("Test Command");
        }
    }
}