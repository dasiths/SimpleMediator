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
        public async Task<SimpleResponse> RunAsync(SimpleQuery request, IMediationContext mediationContext,
            CancellationToken cancellationToken, HandleRequestDelegate<SimpleQuery, SimpleResponse> next)
        {
            Console.WriteLine("Validation hit");
            return await next.Invoke(request, mediationContext, cancellationToken);
        }
    }
}
