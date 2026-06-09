using Hakchi.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hakchi.Core.Services
{
    internal class LaunchFlags : ILaunchFlags
    {
        public bool IsPortable { get; private set; }

        public bool IsDebug { get; private set; }
        public string? VersionFormat { get; private set; }
        public string? VersionFile { get; private set; }

        public LaunchFlags(ILaunchArguments launchArguments, IAssemblyInfo assemblyInfo)
        {
            var args = launchArguments.Arguments;
            IsPortable = !args.Contains("/nonportable") || args.Contains("/portable");

            if (File.Exists(Path.Combine(assemblyInfo.EntryAssemblyDirectory, "nonportable.flag")))
            {
                IsPortable = false;
            }

            (VersionFormat, VersionFile) = ParseVersionFormat(args);
        }

        private static (string?, string?) ParseVersionFormat(string[] args)
        {
            string? versionFormat = null;
            string? versionFile = null;


            int versionFormatArgIndex;
            if (args != null && (versionFormatArgIndex = Array.IndexOf(args, "--versionFormat")) != -1)
            {
                versionFormat = args[versionFormatArgIndex + 1];

                var versionFileArgIndex = -1;
                if (args != null && (versionFileArgIndex = Array.IndexOf(args, "--versionFile")) != -1)
                {
                    versionFile = args[versionFileArgIndex + 1];
                }
            }

            return (versionFormat, versionFile);
        }
    }
}
