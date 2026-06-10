using Hakchi.Core.Interfaces;
using Hakchi.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hakchi.Core
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddHakchiServices(this IServiceCollection services, string[] args)
        {
            return services
                .AddSingleton<ILaunchArguments>((_) => new LaunchArguments(args))
                .AddAttributedServices(typeof(ServiceRegistration).Assembly);
        }
    }
}
