using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace IDMActClean.Core
{
    /// <summary>Trang thai phat hien IDM tren may hien tai.</summary>
    public enum IdmState
    {
        /// <summary>Serial + IDMan.exe -- IDM da kich hoat hop le.</summary>
        Registered,
        /// <summary>No serial, IDMan.exe found -- IDM trial / het han.</summary>
        InstalledOnly,
        /// <summary>Serial found, no IDMan.exe -- serial con, IDM da go.</summary>
        SerialOnly,
        /// <summary>No serial, no IDMan.exe -- may sach.</summary>
        Clean
    }

    /// <summary>Thong tin dang ky doc tu HKCU\Software\DownloadManager.</summary>
    public class IdmRegistrationInfo
    {
        public string FName  { get; set; } = string.Empty;
        public string LName  { get; set; } = string.Empty;
        public string Email  { get; set; } = string.Empty;
        public string Serial { get; set; } = string.Empty;

        public bool HasSerial => !string.IsNullOrWhiteSpace(Serial);

        public string FullName
        {
            get
            {
                var parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(FName)) parts.Add(FName);
                if (!string.IsNullOrWhiteSpace(LName)) parts.Add(LName);
                return string.Join(" ", parts.ToArray());
            }
        }
    }

    /// <summary>
    /// Logic phat hien va thao tac IDM.
    /// Pure logic -- khong phu thuoc bat ky thu vien UI nao.
    /// </summary>
    public static class IdmHelper
    {
        // ─── Registry paths ─────────────────────────────────────────────────
        private const string HkcuDm       = @"Software\DownloadManager";
        private const string HklmIdm      = @"SOFTWARE\Internet Download Manager";
        private const string HklmWow      = @"SOFTWARE\WOW6432Node\Internet Download Manager";
        private const string HklmUninst64 =
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Internet Download Manager";
        private const string HklmUninst32 =
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Internet Download Manager";

        /// <summary>7 gia tri registry can xoa khi deactivate.</summary>
        public static readonly string[] DeactivateValues =
        {
            "FName", "LName", "Email", "Serial",
            "scansk", "tvfrdt", "radxcnt"
        };

        // ─── Public API ─────────────────────────────────────────────────────

        /// <summary>
        /// Doc thong tin dang ky tu HKCU\Software\DownloadManager.
        /// Khong nem exception -- tra ve object rong neu key khong ton tai.
        /// </summary>
        public static IdmRegistrationInfo GetRegistrationInfo()
        {
            var info = new IdmRegistrationInfo();
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(HkcuDm, writable: false))
                {
                    if (key == null) return info;
                    info.FName  = (key.GetValue("FName")  as string) ?? string.Empty;
                    info.LName  = (key.GetValue("LName")  as string) ?? string.Empty;
                    info.Email  = (key.GetValue("Email")  as string) ?? string.Empty;
                    info.Serial = (key.GetValue("Serial") as string) ?? string.Empty;
                }
            }
            catch { /* registry khong truy cap duoc */ }
            return info;
        }

        /// <summary>
        /// Tim duong dan IDMan.exe theo 5 nguon du phong doc lap.
        /// Tra ve null neu khong tim thay.
        /// </summary>
        public static string GetIDMExePath()
        {
            // Nguon 1: HKCU (IDM ghi ExePath o day tu v6.x)
            string path = ReadExePathFromRegistry(Registry.CurrentUser, HkcuDm);
            if (IsValidExePath(path)) return path;

            // Nguon 2: HKLM x64
            path = ReadExePathFromRegistry(Registry.LocalMachine, HklmIdm);
            if (IsValidExePath(path)) return path;

            // Nguon 3: HKLM WOW64
            path = ReadExePathFromRegistry(Registry.LocalMachine, HklmWow);
            if (IsValidExePath(path)) return path;

            // Nguon 4: Uninstall registry (InstallLocation)
            path = ReadInstallLocationFromUninstall(HklmUninst64);
            if (IsValidExePath(path)) return path;

            path = ReadInstallLocationFromUninstall(HklmUninst32);
            if (IsValidExePath(path)) return path;

            // Nguon 5: Thu muc cai dat mac dinh
            foreach (string candidate in DefaultInstallCandidates())
            {
                if (IsValidExePath(candidate)) return candidate;
            }

            return null;
        }

        /// <summary>Kiem tra IDMan.exe co dang chay khong (theo ten process).</summary>
        public static bool IsIDMProcessRunning()
        {
            try   { return Process.GetProcessesByName("IDMan").Length > 0; }
            catch { return false; }
        }

        /// <summary>
        /// Phat hien trang thai IDM theo ma tran 4 o:
        ///   (Serial) x (IDMan.exe ton tai tren disk hoac dang chay).
        /// </summary>
        public static IdmState DetectState()
        {
            var  info      = GetRegistrationInfo();
            bool hasSerial = info.HasSerial;
            bool hasExe    = (GetIDMExePath() != null) || IsIDMProcessRunning();

            if ( hasSerial &&  hasExe) return IdmState.Registered;
            if ( hasSerial && !hasExe) return IdmState.SerialOnly;
            if (!hasSerial &&  hasExe) return IdmState.InstalledOnly;
            return IdmState.Clean;
        }

        /// <summary>
        /// Lay phien ban IDM tu FileVersionInfo cua IDMan.exe.
        /// Tra ve null neu khong doc duoc.
        /// </summary>
        public static string GetIDMVersion()
        {
            try
            {
                string exePath = GetIDMExePath();
                if (exePath == null) return null;
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(exePath);
                return fvi.FileBuildPart > 0
                    ? string.Format("{0}.{1} Build {2}",
                        fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart)
                    : string.Format("{0}.{1}",
                        fvi.FileMajorPart, fvi.FileMinorPart);
            }
            catch { return null; }
        }

        /// <summary>
        /// Dung tien trinh IDMan.exe (neu dang chay) truoc khi xoa registry.
        /// WaitForExit(3000) de tranh race condition.
        /// </summary>
        public static void KillIDM()
        {
            try
            {
                foreach (var p in Process.GetProcessesByName("IDMan"))
                {
                    try
                    {
                        p.Kill();
                        p.WaitForExit(3000);
                    }
                    catch { }
                    finally { p.Dispose(); }
                }
            }
            catch { }
        }

        // ─── Private helpers ────────────────────────────────────────────────

        private static string ReadExePathFromRegistry(RegistryKey hive, string subKeyPath)
        {
            try
            {
                using (RegistryKey key = hive.OpenSubKey(subKeyPath, writable: false))
                {
                    if (key == null) return null;
                    return key.GetValue("ExePath") as string;
                }
            }
            catch { return null; }
        }

        private static string ReadInstallLocationFromUninstall(string subKeyPath)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(subKeyPath, writable: false))
                {
                    if (key == null) return null;
                    string loc = key.GetValue("InstallLocation") as string;
                    if (string.IsNullOrWhiteSpace(loc)) return null;
                    return Path.Combine(loc.TrimEnd('\\', '/'), "IDMan.exe");
                }
            }
            catch { return null; }
        }

        private static IEnumerable<string> DefaultInstallCandidates()
        {
            string pf64 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string pf32 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            var roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { pf64, pf32 };
            string[] folderNames = { "Internet Download Manager", "IDM" };

            foreach (string root in roots)
            foreach (string folder in folderNames)
            {
                yield return Path.Combine(root, folder, "IDMan.exe");
            }
        }

        private static bool IsValidExePath(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && File.Exists(path);
        }
    }
}
