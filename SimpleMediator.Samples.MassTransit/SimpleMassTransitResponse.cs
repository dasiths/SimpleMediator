using SimpleMediator.Samples.Shared;

namespace SimpleMediator.Samples.MassTransit
{
    public class SimpleMassTransitResponse: ValidationResult
    {
        public string Message { get; set; }
    }
}