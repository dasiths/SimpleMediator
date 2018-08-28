using System;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.ConsoleApp
{
    public class SimpleMediationContext : IMediationContext
    {
        public DateTimeOffset CurrentTime { get; set; }
    }
}