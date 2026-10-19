using Microsoft.Win32;

namespace IDMActClean.Core
{
    public static class DotNetChecker
    {
        // .NET Framework 4.7.2's minimum "Release" DWORD, per Microsoft's version table:
        // https://learn.microsoft.com/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed
        private const int MinRelease472 = 461808;

        public const string DownloadUrl =
            "https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472";

        public static bool IsNet472OrNewerInstalled()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"))
                {
                    var release = key?.GetValue("Release");
                    return release is int r && r >= MinRelease472;
                }
            }
            catch
            {
                // Can't read the registry — assume it's missing rather than crash later with a
                // cryptic "could not load file or assembly" error from the runtime itself.
                return false;
            }
        }
    }
}
