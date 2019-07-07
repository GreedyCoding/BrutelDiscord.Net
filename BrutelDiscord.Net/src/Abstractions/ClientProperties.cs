using BrutelDiscord.Clients;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;

namespace BrutelDiscord.Abstractions
{
    class ClientProperties
    {
        [JsonProperty("$os")]
        public string OperatingSystem
        {
            get
            {
                #if !HAS_ENVIRONMENT
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        return "windows";
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        return "linux";
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        return "osx";

                    var plat = RuntimeInformation.OSDescription.ToLowerInvariant();
                #else
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.WinCE)
                        return "windows";
                    else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
                        return "osx";
                    else if (Environment.OSVersion.Platform == PlatformID.Win32S || Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.Xbox)
                        return "potato";
                    else if (Environment.OSVersion.Platform == PlatformID.Unix)
                        return "unix";

                    var plat = Environment.OSVersion.VersionString;
                #endif

                if (plat.Contains("freebsd"))
                    return "freebsd";
                else if (plat.Contains("openbsd"))
                    return "openbsd";
                else if (plat.Contains("netbsd"))
                    return "netbsd";
                else if (plat.Contains("dragonfly"))
                    return "dragonflybsd";
                else if (plat.Contains("miros bsd") || plat.Contains("mirbsd"))
                    return "miros bsd";
                else if (plat.Contains("desktopbsd"))
                    return "desktopbsd";
                else if (plat.Contains("darwin"))
                    return "osx";
                else if (plat.Contains("unix"))
                    return "unix";
                else return "unknown operating system";
            }
        }

        [JsonProperty("$browser")]
        public string Browser => "BrutelDiscord.Net";

        [JsonProperty("$device")]
        public string Device => "BrutelDevice v0.9";

        [JsonProperty("$referrer")]
        public string Referrer => "";

        [JsonProperty("$referring_domain")]
        public string ReferringDomain => "";

    }
}
