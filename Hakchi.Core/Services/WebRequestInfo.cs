using Hakchi.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Hakchi.Core.Services
{
    [RegisterService(ServiceLifetime.Singleton)]
    public class WebRequestInfo
    {
        public string UserAgent { get; init; }
        public WebRequestInfo(IAssemblyInfo assemblyInfo)
        {
            UserAgent = $"Hakchi2 CE/{assemblyInfo.EntryAssemblyVersion.ToString()} (https://github.com/TeamShinkansen/Hakchi2-CE)";
        }
    }
}
