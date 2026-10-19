using System;
using System.Diagnostics;
using System.Windows.Forms;
using IDMActClean.Core;

namespace IDMActClean
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Checked here (before any WinForms code that needs 4.7.2 runs) rather than relying
            // on the OS to refuse to launch the exe — that failure mode is a bare "this app
            // can't run on your PC" dialog with no indication of what's missing or how to fix it.
            if (!DotNetChecker.IsNet472OrNewerInstalled() && !PromptInstallDotNet())
                return;

            Application.Run(new UI.MainForm());
        }

        // Returns true if the user should be let through anyway. Previously this always
        // returned false regardless of the button pressed — harmless on a real miss, but
        // DotNetChecker treats "can't read the registry" (e.g. blocked by Group Policy) the same
        // as "not installed", so a false negative on a machine that actually has 4.7.2+ used to
        // block the app with no way through. Cancel now means "I'm sure it's installed, run anyway".
        private static bool PromptInstallDotNet()
        {
            var result = MessageBox.Show(
                "Ứng dụng này cần .NET Framework 4.7.2 trở lên, nhưng máy bạn chưa có (thường gặp trên Windows 7 hoặc bản Windows 10/11 cài tối giản).\n\n" +
                "Yes = mở trang tải .NET Framework 4.7.2 rồi thoát.\n" +
                "No = thoát.\n" +
                "Cancel = vẫn mở ứng dụng (nếu bạn chắc máy đã có .NET 4.7.2+).",
                "Thiếu .NET Framework 4.7.2",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Cancel) return true;

            if (result == DialogResult.Yes)
            {
                try { Process.Start(DotNetChecker.DownloadUrl); }
                catch { /* best-effort — user can still navigate there manually */ }
            }

            return false;
        }
    }
}
