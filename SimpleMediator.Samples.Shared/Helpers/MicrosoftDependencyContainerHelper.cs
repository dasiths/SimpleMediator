using Microsoft.Extensions.DependencyInjection;
using SimpleMediator.Extensions.Microsoft.DependencyInjection;

namespace SimpleMediator.Samples.Shared.Helpers
{
    public class MicrosoftDependencyContainerHelper
    {
        public static ServiceProvider CreateServiceCollection()
        {
            var services = new ServiceCollection();

            services
                .AddSimpleMediator()
                .AddSimpleMediatorMiddleware();

            return services.BuildServiceProvider();
        }
    }
}