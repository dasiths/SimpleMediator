using System;
using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleQueryHandler : QueryHandler<SimpleQuery, SimpleResponse>
    {
        protected override async Task<SimpleResponse> HandleQueryAsync(SimpleQuery query)
        {
            Console.WriteLine("Test query");

            return new SimpleResponse()
            {
                Message = "Test query messsage"
            };
        }
    }
}