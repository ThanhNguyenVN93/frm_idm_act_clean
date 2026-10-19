using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IDMActClean.Core
{
    public class ActivityEntry
    {
        public DateTime Time   { get; set; }
        public string   Action { get; set; }
    }

    // Small persisted log of state-changing actions (deactivate, reinstall+activate, export) —
    // separate from the on-screen activity log, which only holds the current session's lines.
    // This is what lets the "Lịch sử gần đây" card survive an app restart.
    public static class ActivityHistory
    {
        private const int MaxEntries = 50;

        private static string FilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "IDMActClean", "history.log");

        public static void Add(string action)
        {
            try
            {
                string dir = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var lines = ReadLines().ToList();
                lines.Add($"{DateTime.Now:O}|{action.Replace("|", " ")}");
                if (lines.Count > MaxEntries)
                    lines = lines.Skip(lines.Count - MaxEntries).ToList();

                File.WriteAllLines(FilePath, lines);
            }
            catch { /* best-effort — this history is a nicety, not critical data */ }
        }

        /// <summary>Most recent entries first (newest → oldest), up to <paramref name="count"/>.</summary>
        public static List<ActivityEntry> GetRecent(int count)
        {
            var result = new List<ActivityEntry>();
            try
            {
                foreach (var line in ReadLines().Reverse().Take(count))
                {
                    int sep = line.IndexOf('|');
                    if (sep < 0) continue;
                    if (DateTime.TryParse(line.Substring(0, sep), out var t))
                        result.Add(new ActivityEntry { Time = t, Action = line.Substring(sep + 1) });
                }
            }
            catch { }
            return result;
        }

        private static IEnumerable<string> ReadLines() =>
            File.Exists(FilePath) ? File.ReadAllLines(FilePath) : Enumerable.Empty<string>();
    }
}
