using System;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Middleware;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleContrainedValidator<TRequest, TResponse> : IMiddleware<TRequest, TResponse> where TRequest : IRequest<TResponse> where TResponse : ValidationResult
    {
        public async Task<TResponse> RunAsync(TRequest request, IMediationContext mediationContext, HandleRequestDelegate<TRequest, TResponse> next)
        {
            Console.WriteLine("Constrained validator hit");
            var result = await next.Invoke(request, mediationContext);
            return result;
        }
    }
}