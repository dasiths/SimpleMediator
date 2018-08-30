namespace SimpleMediator.Core
{
    public class MediationContext : IMediationContext
    {
        public static MediationContext Default => new MediationContext();
    }
}