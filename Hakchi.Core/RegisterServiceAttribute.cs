namespace Hakchi.Core
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using System.Reflection;

    

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RegisterServiceAttribute : Attribute
    {
        public RegisterServiceAttribute(
            ServiceLifetime lifetime = ServiceLifetime.Transient,
            Type? serviceType = null)
        {
            Lifetime = lifetime;
            ServiceType = serviceType;
        }

        public ServiceLifetime Lifetime { get; }

        public Type? ServiceType { get; }
    }

    public static partial class RegisterServiceAttributeExtensions
    {
        public static IServiceCollection AddAttributedServices(
       this IServiceCollection services,
       Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false });

            foreach (var implementationType in types)
            {
                var attributes = implementationType
                    .GetCustomAttributes<RegisterServiceAttribute>();

                foreach (var attribute in attributes)
                {
                    var serviceType = attribute.ServiceType ?? implementationType;

                    var descriptor = attribute.Lifetime switch
                    {
                        ServiceLifetime.Singleton =>
                            ServiceDescriptor.Singleton(serviceType, implementationType),

                        ServiceLifetime.Scoped =>
                            ServiceDescriptor.Scoped(serviceType, implementationType),

                        _ =>
                            ServiceDescriptor.Transient(serviceType, implementationType)
                    };

                    services.Add(descriptor);
                }
            }

            return services;
        }
    }
}
