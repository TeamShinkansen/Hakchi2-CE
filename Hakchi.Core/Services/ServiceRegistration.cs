using Hakchi.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Hakchi.Core.Services
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddHakchiServices(this IServiceCollection services, string[] args)
        {
            return services
                .AddSingleton<IAssemblyInfo, AssemblyInfo>()
                .AddSingleton<ISpecialLocations, SpecialLocations>()
                .AddSingleton<ILaunchArguments>(new LaunchArguments(args))
                .AddSingleton<ILaunchFlags, LaunchFlags>()
                .AddSingleton<IHakchiPaths, HakchiPaths>();
        }
    }
}
