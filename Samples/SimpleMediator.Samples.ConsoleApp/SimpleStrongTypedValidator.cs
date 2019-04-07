using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Middleware;
using SimpleMediator.Samples.Shared;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleStrongTypedValidator: IMiddleware<SimpleQuery, SimpleResponse>
    {
        public async Task<SimpleResponse> RunAsync(SimpleQuery message, IMediationContext mediationContext,
            CancellationToken cancellationToken, HandleMessageDelegate<SimpleQuery, SimpleResponse> next)
        {
            Console.WriteLine("Validation hit");
            return await next.Invoke(message, mediationContext, cancellationToken);
        }
    }
}
