using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace IDMActClean.Core
{
    public class UpdateResult
    {
        public bool   IsLatest       { get; set; }
        public string LatestFilename { get; set; }
        public string CurrentVersion { get; set; }
        public string Error          { get; set; }
    }

    public static class UpdateChecker
    {
        private const string DownloadPage = "https://www.internetdownloadmanager.com/download.html";
        private static readonly Regex VersionRegex =
            new Regex(@"idman(\d+)build(\d+)\.exe", RegexOptions.IgnoreCase);

        public static async Task<UpdateResult> CheckAsync(CancellationToken ct = default)
        {
            string current = IdmHelper.GetIDMVersion();
            var result = new UpdateResult { CurrentVersion = current };

            try
            {
                using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) })
                using (var request = new HttpRequestMessage(HttpMethod.Get, DownloadPage))
                // SendAsync + CancellationToken cancels the in-flight request immediately;
                // GetStringAsync(url) (no token overload on .NET Framework) would otherwise keep
                // the HTTP request running until the 15s timeout even after the user cancels.
                using (var response = await client.SendAsync(request, ct).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    string html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var match = VersionRegex.Match(html);
                    if (!match.Success)
                    {
                        result.Error = "Không phát hiện phiên bản mới trên trang chủ.";
                        return result;
                    }

                    result.LatestFilename = match.Value;
                    bool? latest = IsCurrentVersionLatest(current, match.Groups[1].Value, match.Groups[2].Value);
                    if (latest == null)
                        result.Error = $"Không đọc được phiên bản IDM hiện tại (\"{current}\") để so sánh.";
                    else
                        result.IsLatest = latest.Value;
                }
            }
            catch (OperationCanceledException)
            {
                result.Error = "Hết thời gian kết nối.";
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }

            return result;
        }

        // Returns null when `current` doesn't match the expected format — distinct from "false"
        // (definitely outdated), so the caller can report "couldn't compare" instead of silently
        // claiming a newer version exists when it just failed to parse.
        private static bool? IsCurrentVersionLatest(string current, string majorStr, string buildStr)
        {
            if (string.IsNullOrEmpty(current)) return null;
            // current format: "6.42 Build 3"
            var m = Regex.Match(current, @"(\d+)\.(\d+)\s+Build\s+(\d+)", RegexOptions.IgnoreCase);
            if (!m.Success) return null;

            int curMajor = int.Parse(m.Groups[1].Value);
            int curMinor = int.Parse(m.Groups[2].Value);
            int curBuild = int.Parse(m.Groups[3].Value);

            // filename: idman642build3.exe → major=642 (i.e. 6.42), build=3
            string maj = majorStr; // e.g. "642"
            int newMajor = int.Parse(maj.Length >= 3 ? maj.Substring(0, maj.Length - 2) : "0");
            int newMinor = int.Parse(maj.Length >= 2 ? maj.Substring(maj.Length - 2) : "0");
            int newBuild = int.Parse(buildStr);

            if (curMajor != newMajor) return curMajor > newMajor;
            if (curMinor != newMinor) return curMinor > newMinor;
            return curBuild >= newBuild;
        }
    }
}
