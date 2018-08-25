using System;
using System.Collections.Generic;
using System.Text;
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
            return new SimpleResponse()
            {
                Message = "Hello World"
            };
        }
    }
}
