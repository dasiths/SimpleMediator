using System.Threading.Tasks;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleQueryHandler : IQueryHandler<SimpleQuery, SimpleResponse>
    {
        public async Task<SimpleResponse> HandleAsync(SimpleQuery query)
        {
            return await Task.FromResult(new SimpleResponse()
            {
                Message = "Hello World"
            });
        }
    }
}