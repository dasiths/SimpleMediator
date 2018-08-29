using System;
using System.Threading.Tasks;
using SimpleMediator.Commands;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleCommandHandler : CommandHandler<SimpleCommand>
    {
        protected override async Task HandleCommandAsync(SimpleCommand request, IMediationContext mediationContext)
        {
            Console.WriteLine("Test Command");
        }
    }
}