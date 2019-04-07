using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Middleware;
using SimpleMediator.Samples.Shared;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class LoggerMiddleware2<TMessage, TResponse> : IMiddleware<TMessage, TResponse> where TMessage : IMessage<TResponse>
    {
        public async Task<TResponse> RunAsync(TMessage message, IMediationContext mediationContext,
            CancellationToken cancellationToken, HandleMessageDelegate<TMessage, TResponse> next)
        {
            if (mediationContext is SimpleMediationContext context)
            {
                Console.WriteLine(context.CurrentTime);
            }

            Console.WriteLine("Request pre logged using middleware 2");
            var result = await next.Invoke(message, mediationContext, cancellationToken);
            Console.WriteLine("Request post logged using middleware 2");

            return result;
        }
    }
}