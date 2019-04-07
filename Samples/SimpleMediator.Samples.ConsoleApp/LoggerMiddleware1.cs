using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Middleware;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class LoggerMiddleware1<TMessage, TResponse> : IMiddleware<TMessage, TResponse> where TMessage : IMessage<TResponse>
    {
        public async Task<TResponse> RunAsync(TMessage message, IMediationContext mediationContext,
            CancellationToken cancellationToken, HandleMessageDelegate<TMessage, TResponse> next)
        {
            Console.WriteLine("Message pre logged using middleware 1");
            var result = await next.Invoke(message, mediationContext, cancellationToken);
            Console.WriteLine("Message post logged using middleware 1");

            return result;
        }
    }
}