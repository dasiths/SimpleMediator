using System;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Middleware;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class LoggerMiddleware1<TRequest, TResponse> : IMiddleware<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> RunAsync(TRequest request, HandleRequestDelegate<TRequest, TResponse> next, IMediationContext mediationContext)
        {
            Console.WriteLine("Request pre logged using middleware 1");
            var result = await next.Invoke(request);
            Console.WriteLine("Request post logged using middleware 1");

            return result;
        }
    }
}