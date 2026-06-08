using System;
using System.Net;
using System.ComponentModel;

namespace com.clusterrr.hakchi_gui
{
    class HakchiWebClient : WebClient
    {
        public static readonly string UserAgent = $"Hakchi2 CE/{Shared.AppVersion.ToString()} (https://github.com/TeamShinkansen/Hakchi2-CE)";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Method
        {
            get;
            set;
        }

        public HakchiWebClient() {
            Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
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
