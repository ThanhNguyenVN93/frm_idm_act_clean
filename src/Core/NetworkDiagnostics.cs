using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace IDMActClean.Core
{
    public class PingResult
    {
        public bool Success     { get; set; }
        public long RoundtripMs { get; set; }
    }

    public class SetDnsResult
    {
        public bool   Success { get; set; }
        public string Error   { get; set; }
    }

    // Read-only by default (GetPrimaryDnsServers / PingHostAsync) — SetPublicDns is the only
    // thing that touches the system, and it's only ever called from an explicit, user-confirmed
    // menu click, never automatically. Silently rewriting a user's DNS on every launch is exactly
    // the kind of behavior antivirus heuristics (rightly) flag as hijacker-like.
    public static class NetworkDiagnostics
    {
        private static readonly string[] PublicDnsServers = { "1.1.1.1", "8.8.8.8" };

        private static NetworkInterface GetPrimaryAdapter()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up
                          && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback
                          && ni.GetIPProperties().GatewayAddresses
                                .Any(g => g.Address.AddressFamily == AddressFamily.InterNetwork))
                .OrderByDescending(ni => ni.Speed)
                .FirstOrDefault();
        }

        public static string GetPrimaryAdapterName() => GetPrimaryAdapter()?.Name;

        public static List<string> GetPrimaryDnsServers()
        {
            try
            {
                var adapter = GetPrimaryAdapter();
                if (adapter == null) return new List<string>();

                return adapter.GetIPProperties().DnsAddresses
                    .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                    .Select(ip => ip.ToString())
                    .ToList();
            }
            catch { return new List<string>(); }
        }

        public static bool IsUsingPublicDns(IEnumerable<string> dnsServers) =>
            dnsServers.Any(d => PublicDnsServers.Contains(d));

        public static async Task<PingResult> PingHostAsync(string host, int timeoutMs)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync(host, timeoutMs).ConfigureAwait(false);
                    return new PingResult
                    {
                        Success     = reply.Status == IPStatus.Success,
                        RoundtripMs = reply.Status == IPStatus.Success ? reply.RoundtripTime : 0,
                    };
                }
            }
            catch { return new PingResult { Success = false }; }
        }

        // Adapter names are attacker-controllable in principle (anyone who can rename a network
        // adapter can put arbitrary characters in it) and this string lands inside a quoted
        // segment of a cmd.exe command line. Escaping embedded quotes is what stops a crafted
        // name from breaking out of its quotes and injecting extra commands.
        private static string EscapeForCmd(string s) => (s ?? "").Replace("\"", "\\\"");

        // Requires Admin — triggers a UAC prompt via the "runas" verb. Only call this from code
        // the user explicitly triggered (a confirmed button/menu click), never automatically.
        // Both netsh calls run inside one elevated cmd.exe so the user only sees one UAC prompt.
        public static SetDnsResult SetPublicDns(string adapterName)
        {
            try
            {
                string safeName = EscapeForCmd(adapterName);
                string args =
                    $"/c netsh interface ip set dns name=\"{safeName}\" static {PublicDnsServers[0]} " +
                    $"&& netsh interface ip add dns name=\"{safeName}\" {PublicDnsServers[1]} index=2";

                var psi = new ProcessStartInfo("cmd.exe", args)
                {
                    UseShellExecute = true,
                    Verb            = "runas",
                    WindowStyle     = ProcessWindowStyle.Hidden,
                    CreateNoWindow  = true,
                };

                using (var p = Process.Start(psi))
                {
                    bool exited = p.WaitForExit(15_000);
                    if (!exited)
                    {
                        try { p.Kill(); } catch { }
                        return new SetDnsResult { Success = false, Error = "TIMEOUT" };
                    }
                    return new SetDnsResult { Success = p.ExitCode == 0 };
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // User declined the UAC prompt.
                return new SetDnsResult { Success = false, Error = "UAC_DECLINED" };
            }
            catch (Exception ex)
            {
                return new SetDnsResult { Success = false, Error = ex.Message };
            }
        }
    }
}
