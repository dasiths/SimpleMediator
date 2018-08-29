using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Middleware;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleValidator: IMiddleware<SimpleQuery, SimpleResponse>
    {
        public async Task<SimpleResponse> RunAsync(SimpleQuery request, HandleRequestDelegate<SimpleQuery, SimpleResponse> next, IMediationContext mediationContext)
        {
            Console.WriteLine("Validation hit");
            return await next.Invoke(request);
        }
    }
}
