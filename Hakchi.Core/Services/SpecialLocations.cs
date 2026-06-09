using Hakchi.Core.Interfaces;

namespace Hakchi.Core.Services
{
    public class SpecialLocations : ISpecialLocations
    {
        public string Documents => field ??= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) ?? throw new InvalidOperationException("Unable to determine documents location.");
    }
}