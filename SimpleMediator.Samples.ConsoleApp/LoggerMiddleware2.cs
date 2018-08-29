using System;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Middleware;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class LoggerMiddleware2<TRequest, TResponse> : IMiddleware<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> RunAsync(TRequest request, IMediationContext mediationContext, HandleRequestDelegate<TRequest, TResponse> next)
        {
            if (mediationContext is SimpleMediationContext context)
            {
                Console.WriteLine(context.CurrentTime);
            }

            Console.WriteLine("Request pre logged using middleware 2");
            var result = await next.Invoke(request, mediationContext);
            Console.WriteLine("Request post logged using middleware 2");

            return result;
        }
    }
}