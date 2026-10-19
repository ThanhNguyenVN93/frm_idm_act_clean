using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace IDMActClean.UI
{
    public class DonateForm : Form
    {
        private const string KofiUrl = "https://ko-fi.com/thanhnguyen150993";
        private readonly PictureBox _qrPicture;

        public DonateForm(AppTheme theme, bool english)
        {
            var p = ThemeTokens.Resolve(theme);

            Text            = english ? "Support the author" : "Ủng hộ tác giả";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            StartPosition   = FormStartPosition.CenterParent;
            ClientSize      = new Size(300, 420);
            BackColor       = p.Bg;
            Font            = new Font("Segoe UI", 9F);

            _qrPicture = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size     = new Size(240, 262),
                Location = new Point(30, 20),
                BackColor = p.Surface,
            };
            // Embedded so the dialog still works if someone copies just the exe elsewhere —
            // momo.jpg won't necessarily travel alongside it as a loose file.
            try
            {
                using (var stream = Assembly.GetExecutingAssembly()
                           .GetManifestResourceStream("IDMActClean.momo.jpg"))
                {
                    if (stream != null) _qrPicture.Image = Image.FromStream(stream);
                }
            }
            catch { /* missing/corrupt resource — dialog still works without the QR image */ }

            var lblCaption = new Label
            {
                Text      = english
                    ? "Scan the MoMo QR code, or support via Ko-fi"
                    : "Quét mã QR MoMo, hoặc ủng hộ qua Ko-fi",
                ForeColor = p.SubText,
                TextAlign = ContentAlignment.MiddleCenter,
                Location  = new Point(20, 292),
                Size      = new Size(260, 36),
            };

            var btnKofi = new Guna2Button
            {
                Text         = "Ko-fi",
                Location     = new Point(70, 336),
                Size         = new Size(160, 36),
                BorderRadius = 8,
                FillColor    = p.Accent,
                ForeColor    = p.BtnText,
            };
            btnKofi.Click += (s, e) =>
            {
                try { Process.Start(KofiUrl); } catch { }
            };

            Controls.AddRange(new Control[] { _qrPicture, lblCaption, btnKofi });
        }

        // PictureBox.Dispose() does NOT dispose the Image it's holding on .NET Framework (unlike
        // .NET 5+) — without this, each time the dialog opens it leaks the decoded QR Bitmap.
        protected override void Dispose(bool disposing)
        {
            if (disposing) _qrPicture?.Image?.Dispose();
            base.Dispose(disposing);
        }
    }
}
