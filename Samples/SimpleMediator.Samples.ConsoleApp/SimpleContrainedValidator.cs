using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Middleware;
using SimpleMediator.Samples.Shared;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleContrainedValidator<TMessage, TResponse> : IMiddleware<TMessage, TResponse> where TMessage : IMessage<TResponse> where TResponse : ValidationResult
    {
        public async Task<TResponse> RunAsync(TMessage message, IMediationContext mediationContext,
            CancellationToken cancellationToken, HandleMessageDelegate<TMessage, TResponse> next)
        {
            Console.WriteLine("Constrained validator hit");
            var result = await next.Invoke(message, mediationContext, cancellationToken);
            return result;
        }
    }
}