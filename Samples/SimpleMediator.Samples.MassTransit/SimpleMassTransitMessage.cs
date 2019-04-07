using SimpleMediator.Queries;

namespace SimpleMediator.Samples.MassTransit
{
    public class SimpleMassTransitMessage: IQuery<SimpleMassTransitResponse>
    {
        public string Message { get; set; }
    }
}
