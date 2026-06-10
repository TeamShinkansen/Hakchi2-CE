using Hakchi.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Hakchi.Core.Services
{
    [RegisterService(ServiceLifetime.Singleton, typeof(ISpecialLocations))]
    internal class SpecialLocations : ISpecialLocations
    {
        public string Documents => field ??= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) ?? throw new InvalidOperationException("Unable to determine documents location.");
    }
}