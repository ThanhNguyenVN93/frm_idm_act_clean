using System;
using System.Security;
using Microsoft.Win32;

namespace IDMActClean.Core
{
    public static class RegistryHelper
    {
        private const string HkcuKey = @"Software\DownloadManager";
        private const string HklmKey = @"SOFTWARE\Internet Download Manager";
        private const string HklmKeyWow = @"SOFTWARE\WOW6432Node\Internet Download Manager";

        public static DeleteResult DeleteActivationValues()
        {
            var result = new DeleteResult();
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(HkcuKey, writable: true))
                {
                    if (key == null) { result.HkcuSkipped = true; return result; }
                    // Single source of truth is IdmHelper.DeactivateValues — this used to keep
                    // its own copy of the same 7 names, which could silently drift out of sync.
                    foreach (var name in IdmHelper.DeactivateValues)
                    {
                        try { key.DeleteValue(name, throwOnMissingValue: false); }
                        catch { }
                    }
                }
                result.HkcuSuccess = true;
            }
            catch (Exception ex)
            {
                result.HkcuError = ex.Message;
            }

            try { DeleteHklmExePath(); result.HklmSuccess = true; }
            catch (UnauthorizedAccessException) { result.HklmNeedsUac = true; }
            catch (SecurityException)           { result.HklmNeedsUac = true; }
            catch (Exception ex)               { result.HklmError = ex.Message; }

            return result;
        }

        private static void DeleteHklmExePath()
        {
            TryDeleteHklmValue(Registry.LocalMachine, HklmKey, "ExePath");
            TryDeleteHklmValue(Registry.LocalMachine, HklmKeyWow, "ExePath");
        }

        private static void TryDeleteHklmValue(RegistryKey hive, string subKey, string valueName)
        {
            using (var key = hive.OpenSubKey(subKey, writable: true))
                key?.DeleteValue(valueName, throwOnMissingValue: false);
        }

        public static void WriteActivationValues(IdmRegistrationInfo info)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(HkcuKey))
            {
                if (key == null) throw new InvalidOperationException("Cannot open HKCU\\Software\\DownloadManager");
                key.SetValue("FName",  info.FName,  RegistryValueKind.String);
                key.SetValue("LName",  info.LName,  RegistryValueKind.String);
                key.SetValue("Email",  info.Email,  RegistryValueKind.String);
                key.SetValue("Serial", info.Serial, RegistryValueKind.String);
            }
        }
    }

    public class DeleteResult
    {
        public bool   HkcuSuccess  { get; set; }
        public bool   HkcuSkipped  { get; set; }
        public string HkcuError    { get; set; }
        public bool   HklmSuccess  { get; set; }
        public bool   HklmNeedsUac { get; set; }
        public string HklmError    { get; set; }
    }
}
