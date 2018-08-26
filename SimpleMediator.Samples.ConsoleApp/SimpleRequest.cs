using System;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleResponse
    {
        public string Message { get; set; }
    }

    public class SimpleRequest: IRequest<SimpleResponse>
    {
    }

    public class SimpleRequestHandler : IRequestHandler<SimpleRequest, SimpleResponse>
    {
        public async Task<SimpleResponse> HandleAsync(SimpleRequest request)
        {
            return await Task.FromResult(new SimpleResponse()
            {
                Message = "Hello World"
            });
        }
    }

    public class SimpleCommand : IRequest
    {

    }

    public class SimpleCommandHandler : IRequestHandler<SimpleCommand>
    {
        public async Task HandleAsync(SimpleCommand request)
        {
            Console.WriteLine("Test Command");
        }
    }
}
