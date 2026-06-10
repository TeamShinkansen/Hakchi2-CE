using Hakchi.Core.Interfaces;
using System.ComponentModel;
using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Hakchi.Core.Services
{
    [RegisterService(ServiceLifetime.Transient, typeof(WebClient))]
    public class HakchiWebClient : WebClient
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Method
        {
            get;
            set;
        }

        public HakchiWebClient(WebRequestInfo webRequestInfo)
        {
            Headers.Add(HttpRequestHeader.UserAgent, webRequestInfo.UserAgent);
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = base.GetWebRequest(address);

            if (!string.IsNullOrEmpty(Method))
                webRequest.Method = Method;

            return webRequest;
        }
    }
}
