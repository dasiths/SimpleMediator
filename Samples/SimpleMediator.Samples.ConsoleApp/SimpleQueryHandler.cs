﻿using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Queries;
using SimpleMediator.Samples.Shared;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleQueryHandler : QueryHandler<SimpleQuery, SimpleResponse>
    {
        protected override async Task<SimpleResponse> HandleQueryAsync(SimpleQuery query,
            IMediationContext mediationContext, CancellationToken cancellationToken)
        {
            Console.WriteLine("Test query");

            return new SimpleResponse()
            {
                Message = "Test query messsage"
            };
        }
    }
}