using System;

namespace SimpleMediator.Samples.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<Type, Object> factory = (t) =>
            {
                return new SimpleRequestHandler();
            };

            var mediator = new Mediator(factory);
            var simpleRequest = new SimpleRequest();

            var result =  mediator.SendAsync(simpleRequest).GetAwaiter().GetResult();
            Console.WriteLine(result.Message);
            Console.ReadLine();
        }
    }
}
