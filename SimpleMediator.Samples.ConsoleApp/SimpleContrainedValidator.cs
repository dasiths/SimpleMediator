﻿using System;
using System.Threading.Tasks;
using SimpleMediator.Core;
using SimpleMediator.Middleware;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleContrainedValidator<TRequest, TResponse> : IMiddleware<TRequest, TResponse> where TRequest : IRequest<TResponse> 
    // Microsoft DI doesn't support closed type registrations with constrained parameters
    {
        public async Task<TResponse> RunAsync(TRequest request, HandleRequestDelegate<TRequest, TResponse> next, IMediationContext mediationContext)
        {
            if (typeof(ValidationResult).IsAssignableFrom(typeof(TResponse)))
            {
                Console.WriteLine("Constrained validator hit");
            }
            
            var result = await next.Invoke(request);

            return result;
        }
    }
}