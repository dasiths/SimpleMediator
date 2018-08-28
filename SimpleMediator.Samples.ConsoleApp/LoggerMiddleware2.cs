using System;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Middleware;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class LoggerMiddleware2<TRequest, TResponse> : IMiddleware<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> RunAsync(TRequest request, HandleRequestDelegate<TRequest, TResponse> next, IMediationContext mediationContext)
        {
            if (mediationContext is SimpleMediationContext context)
            {
                Console.WriteLine(context.CurrentTime);
            }

            Console.WriteLine("Request pre logged using middleware 2");
            var result = await next.Invoke(request);
            Console.WriteLine("Request post logged using middleware 2");

            return result;
        }
    }
}