using System;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleCommandHandler : CommandHandler<SimpleCommand>
    {
        protected override async Task ExecuteAsync(SimpleCommand request)
        {
            Console.WriteLine("Test Command");
        }
    }
}