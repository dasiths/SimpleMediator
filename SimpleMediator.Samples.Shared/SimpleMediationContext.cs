using System;
using SimpleMediator.Core;

namespace SimpleMediator.Samples.Shared
{
    public class SimpleMediationContext : IMediationContext
    {
        public DateTimeOffset CurrentTime { get; set; }
    }
}