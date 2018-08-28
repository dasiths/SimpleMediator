using System;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Middleware;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class LoggerRequestFilter2<TRequest, TResponse> : IRequestFilter<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task FilterAsync(TRequest request, RequestFilterDelegate<TRequest> next)
        {
            Console.WriteLine("Request logged using filter 2");
            await next.Invoke(request);
        }
    }
}