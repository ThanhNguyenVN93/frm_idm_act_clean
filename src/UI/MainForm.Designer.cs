using Guna.UI2.WinForms;

namespace IDMActClean.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlTitlebar   = new System.Windows.Forms.Panel();
            this.lblTitle      = new System.Windows.Forms.Label();
            this.btnClose      = new System.Windows.Forms.Button();
            this.btnMinimize   = new System.Windows.Forms.Button();
            this.pnlTheme      = new System.Windows.Forms.Panel();
            this.btnThemeLight = new Guna2Button();
            this.btnThemeAuto  = new Guna2Button();
            this.btnThemeDark  = new Guna2Button();
            this.pnlBanner     = new Guna2Panel();
            this.lblBanner     = new System.Windows.Forms.Label();
            this.pnlBannerBadge= new Guna2Panel();
            this.lblBannerBadge= new System.Windows.Forms.Label();
            this.pnlContent    = new System.Windows.Forms.Panel();
            this.pnlInfo       = new Guna2Panel();
            this.lblInfoHeader = new System.Windows.Forms.Label();
            this.lblFName      = new System.Windows.Forms.Label();
            this.txtFName      = new Guna2TextBox();
            this.lblLName      = new System.Windows.Forms.Label();
            this.txtLName      = new Guna2TextBox();
            this.lblEmailLbl   = new System.Windows.Forms.Label();
            this.txtEmail      = new Guna2TextBox();
            this.lblSerialLbl  = new System.Windows.Forms.Label();
            this.txtSerial     = new Guna2TextBox();
            this.lblExePathLbl = new System.Windows.Forms.Label();
            this.lblExePath    = new System.Windows.Forms.Label();
            this.lblVersion    = new System.Windows.Forms.Label();
            // State panels
            this.pnlState1     = new System.Windows.Forms.Panel();
            this.btnDeactivate = new Guna2Button();
            this.btnCheckUpdate= new Guna2Button();
            this.btnExport1    = new Guna2Button();
            this.pnlState2     = new System.Windows.Forms.Panel();
            this.btnBuyLicense = new Guna2Button();
            this.pnlState3     = new System.Windows.Forms.Panel();
            this.btnReinstall  = new Guna2Button();
            this.btnExport3    = new Guna2Button();
            this.pnlState4     = new System.Windows.Forms.Panel();
            this.btnDownloadIDM= new Guna2Button();
            // Recent activity history (persisted across restarts — see ActivityHistory)
            this.pnlHistory       = new Guna2Panel();
            this.lblHistoryHeader = new System.Windows.Forms.Label();
            this.lblHistoryBody   = new System.Windows.Forms.Label();
            // Log
            this.pnlLog        = new Guna2Panel();
            this.lblLogHeader  = new System.Windows.Forms.Label();
            this.rtbLog        = new System.Windows.Forms.RichTextBox();
            // Footer
            this.pnlFooter     = new System.Windows.Forms.Panel();
            this.lblStatus     = new System.Windows.Forms.Label();
            this.btnSettings   = new System.Windows.Forms.Button();
            this.ctxSettings   = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiDonate    = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFeedback  = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFixDns    = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlLangToggle = new System.Windows.Forms.Panel();
            this.lblLangVi     = new System.Windows.Forms.Label();
            this.lblLangEn     = new System.Windows.Forms.Label();
            this.swLangToggle  = new Guna2ToggleSwitch();
            this.tsmiSep       = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiVersion     = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip         = new System.Windows.Forms.ToolTip(this.components);

            // ── Form ──────────────────────────────────────────────────────
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(560, 700);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.None;
            this.Name                = "MainForm";
            this.Text                = "IDM Check Tool";
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font                = new System.Drawing.Font("Segoe UI", 9F);

            // ── Titlebar ──────────────────────────────────────────────────
            this.pnlTitlebar.Dock      = System.Windows.Forms.DockStyle.Top;
            // Size must be set explicitly (not left at the Panel default of 200x100) before the
            // Close/Minimize buttons below are anchored Top|Right — WinForms snapshots each
            // control's "distance from parent's right edge" at the moment it's added to this
            // panel's Controls collection, using the panel's CURRENT size at that time, not its
            // eventual docked size. Leaving the default 200px width made that distance negative,
            // so once Dock stretched the panel to the form's real width the buttons landed off
            // the right edge of the form (invisible).
            this.pnlTitlebar.Size      = new System.Drawing.Size(this.ClientSize.Width, 40);
            this.pnlTitlebar.Padding   = new System.Windows.Forms.Padding(12, 0, 4, 0);
            this.pnlTitlebar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlTitlebar_MouseDown);
            this.pnlTitlebar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlTitlebar_MouseMove);
            this.pnlTitlebar.MouseUp   += new System.Windows.Forms.MouseEventHandler(this.pnlTitlebar_MouseUp);

            this.lblTitle.Text      = "IDM Check Tool";
            this.lblTitle.AutoSize  = true;
            this.lblTitle.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location  = new System.Drawing.Point(12, 10);

            // Theme pill — right side of titlebar
            // Each button is AutoSize + flowed left-to-right by its own real (measured) width
            // instead of hard-coded x offsets — fixed offsets like the old "54"/"108" assumed a
            // width that didn't fit "Sáng", clipping it to "Sán".
            SetThemeButton(this.btnThemeLight, "Sáng");
            SetThemeButton(this.btnThemeAuto,  "Auto");
            SetThemeButton(this.btnThemeDark,  "Tối");
            this.btnThemeLight.Click += new System.EventHandler(this.btnThemeLight_Click);
            this.btnThemeAuto.Click  += new System.EventHandler(this.btnThemeAuto_Click);
            this.btnThemeDark.Click  += new System.EventHandler(this.btnThemeDark_Click);

            this.pnlTheme.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnThemeLight, this.btnThemeAuto, this.btnThemeDark });
            int themeWidth = FlowHorizontally(0, 0, 6, this.btnThemeLight, this.btnThemeAuto, this.btnThemeDark);

            this.pnlTheme.Height  = 28;
            this.pnlTheme.Width   = themeWidth;
            this.pnlTheme.Anchor  = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.pnlTheme.Location= new System.Drawing.Point(452 - themeWidth, 6);

            // Close / Minimize
            // Computed from ClientSize.Width (not hard-coded) so they stay correct if the form's
            // width ever changes; width=560 previously gave 506/478 — same result, just derived.
            SetWinButton(this.btnClose,    "✕", this.ClientSize.Width - 54);
            SetWinButton(this.btnMinimize, "─", this.ClientSize.Width - 82);
            this.btnClose.Click    += new System.EventHandler(this.btnClose_Click);
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);

            this.pnlTitlebar.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblTitle, this.pnlTheme, this.btnMinimize, this.btnClose });

            // ── Banner ────────────────────────────────────────────────────
            // Rounded "card" style (Guna2Panel) with a margin on all sides so it floats over the
            // form background instead of spanning edge-to-edge like a flat strip.
            this.pnlBanner.Dock         = System.Windows.Forms.DockStyle.Top;
            this.pnlBanner.Height       = 40;
            this.pnlBanner.Margin       = new System.Windows.Forms.Padding(12, 10, 12, 8);
            this.pnlBanner.Padding      = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.pnlBanner.BorderRadius = 10;

            this.lblBanner.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Font      = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblBanner.BackColor = System.Drawing.Color.Transparent;

            // Small rounded status chip on the right of the banner ("Đã kích hoạt", …)
            this.pnlBannerBadge.Dock         = System.Windows.Forms.DockStyle.Right;
            this.pnlBannerBadge.Width        = 112;
            this.pnlBannerBadge.Margin       = new System.Windows.Forms.Padding(0, 7, 12, 7);
            this.pnlBannerBadge.BorderRadius = 12;

            this.lblBannerBadge.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblBannerBadge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBannerBadge.Font      = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblBannerBadge.BackColor = System.Drawing.Color.Transparent;
            this.pnlBannerBadge.Controls.Add(this.lblBannerBadge);

            this.pnlBanner.Controls.Add(this.lblBanner);
            this.pnlBanner.Controls.Add(this.pnlBannerBadge);

            // ── Content panel ───────────────────────────────────────────────
            // Docked Top with a fixed height (info row + one 44px action row) instead of Fill —
            // Fill left a large dead gray gap between the action buttons and the log panel,
            // since the content here is always much shorter than the remaining form height.
            // The freed-up space now goes to the log panel (Dock = Fill) below.
            this.pnlContent.Dock          = System.Windows.Forms.DockStyle.Top;
            this.pnlContent.Height        = 380; // top padding (20) + pnlInfo (302) + bottom margin (14) + one state action row (44)
            this.pnlContent.AutoScroll    = true;
            // Top padding (not the banner's own Margin) is what actually creates the gap between
            // the banner and the card below — Control.Margin isn't honored by the plain
            // DefaultLayout engine for Dock siblings sitting directly on the Form, only Padding
            // on an actual parent container is. pnlContent's background matches the form (p.Bg),
            // so this reads as empty space, same as if it were a real gap between the two.
            this.pnlContent.Padding       = new System.Windows.Forms.Padding(12, 20, 12, 8);

            // Info panel — rounded card, like the other panels
            this.pnlInfo.Dock         = System.Windows.Forms.DockStyle.Top;
            this.pnlInfo.Height       = 302;
            this.pnlInfo.Margin       = new System.Windows.Forms.Padding(0, 0, 0, 14);
            this.pnlInfo.Padding      = new System.Windows.Forms.Padding(12);
            this.pnlInfo.BorderRadius = 10;

            // Header hidden — "Thông tin đăng ký (HKCU\Software\DownloadManager)" sat right
            // against the banner above it with no breathing room, so it's off by default. Set
            // lblInfoHeader.Visible = true (and move the rows below back down by ~24px) to bring
            // it back if that registry-source context is wanted again later.
            this.lblInfoHeader.AutoSize = true;
            this.lblInfoHeader.Location = new System.Drawing.Point(12, 0);
            this.lblInfoHeader.Font     = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblInfoHeader.Text     = "Thông tin đăng ký  (HKCU\\Software\\DownloadManager)";
            this.lblInfoHeader.Visible  = false;

            // Children are positioned by absolute Location, so pnlInfo.Padding alone never insets
            // them — Padding only auto-applies to Docked/Anchored children. InfoTopInset is what
            // actually keeps "Tên (FName)" clear of the card's rounded top border instead of
            // sitting right against it.
            AddInfoRow(this.pnlInfo, this.lblFName,     "Tên (FName)",  this.txtFName,  InfoTopInset);
            AddInfoRow(this.pnlInfo, this.lblLName,     "Họ (LName)",   this.txtLName,  InfoTopInset + InfoRowPitch);
            AddInfoRow(this.pnlInfo, this.lblEmailLbl,  "E-mail",       this.txtEmail,  InfoTopInset + InfoRowPitch * 2);
            AddInfoRow(this.pnlInfo, this.lblSerialLbl, "Serial",       this.txtSerial, InfoTopInset + InfoRowPitch * 3);
            foreach (Guna2TextBox tb in new[] { txtFName, txtLName, txtEmail, txtSerial })
                tb.ReadOnly = true;

            int pathRowY = InfoTopInset + InfoRowPitch * 4;
            this.lblExePathLbl.AutoSize = true;
            this.lblExePathLbl.Location = new System.Drawing.Point(12, pathRowY);
            this.lblExePathLbl.Text     = "Đường dẫn:";

            this.lblExePath.AutoSize  = true;
            this.lblExePath.Location  = new System.Drawing.Point(100, pathRowY);
            this.lblExePath.MaximumSize = new System.Drawing.Size(340, 0);

            this.lblVersion.AutoSize  = true;
            this.lblVersion.Location  = new System.Drawing.Point(100, pathRowY + 16);

            this.pnlInfo.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblInfoHeader, this.lblExePathLbl, this.lblExePath, this.lblVersion });

            // State 1 — buttons are flowed left-to-right by their real AutoSize width (see
            // FlowHorizontally) instead of hard-coded x offsets, which used to leave them either
            // overlapping or jammed with no visible gap whenever label text didn't match the
            // width the offsets assumed.
            this.pnlState1.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlState1.Height = 44;
            this.pnlState1.Padding= new System.Windows.Forms.Padding(12, 6, 12, 6);
            AddActionButton(this.btnDeactivate,  "Deactivate");
            AddActionButton(this.btnCheckUpdate, "Check Update");
            AddActionButton(this.btnExport1,     "Xuất thông tin");
            this.btnDeactivate.FillColor  = System.Drawing.Color.FromArgb(220, 38, 38);
            this.btnDeactivate.ForeColor  = System.Drawing.Color.White;
            this.btnDeactivate.Click     += new System.EventHandler(this.btnDeactivate_Click);
            this.btnCheckUpdate.Click    += new System.EventHandler(this.btnCheckUpdate_Click);
            this.btnExport1.Click        += new System.EventHandler(this.btnExport1_Click);
            this.pnlState1.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnDeactivate, this.btnCheckUpdate, this.btnExport1 });
            FlowHorizontally(0, 6, 16, this.btnDeactivate, this.btnCheckUpdate, this.btnExport1);

            // State 2
            this.pnlState2.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlState2.Height = 44;
            this.pnlState2.Padding= new System.Windows.Forms.Padding(12, 6, 12, 6);
            AddActionButton(this.btnBuyLicense, "→ Mua bản quyền chính hãng");
            this.btnBuyLicense.Location = new System.Drawing.Point(0, 6);
            this.btnBuyLicense.Click += new System.EventHandler(this.btnBuyLicense_Click);
            this.pnlState2.Controls.Add(this.btnBuyLicense);

            // State 3
            this.pnlState3.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlState3.Height = 44;
            this.pnlState3.Padding= new System.Windows.Forms.Padding(12, 6, 12, 6);
            AddActionButton(this.btnReinstall, "Cài lại & Kích hoạt");
            AddActionButton(this.btnExport3,   "Xuất thông tin");
            this.btnReinstall.FillColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnReinstall.ForeColor = System.Drawing.Color.White;
            this.btnReinstall.Click += new System.EventHandler(this.btnReinstall_Click);
            this.btnExport3.Click   += new System.EventHandler(this.btnExport3_Click);
            this.pnlState3.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnReinstall, this.btnExport3 });
            FlowHorizontally(0, 6, 16, this.btnReinstall, this.btnExport3);

            // State 4
            this.pnlState4.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlState4.Height = 44;
            this.pnlState4.Padding= new System.Windows.Forms.Padding(12, 6, 12, 6);
            AddActionButton(this.btnDownloadIDM, "Download IDM");
            this.btnDownloadIDM.Location  = new System.Drawing.Point(0, 6);
            this.btnDownloadIDM.FillColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnDownloadIDM.ForeColor = System.Drawing.Color.White;
            this.btnDownloadIDM.Click += new System.EventHandler(this.btnDownloadIDM_Click);
            this.pnlState4.Controls.Add(this.btnDownloadIDM);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.pnlState4, this.pnlState3, this.pnlState2, this.pnlState1, this.pnlInfo });

            // ── Recent activity history ──────────────────────────────────
            // A compact card for the last few state-changing actions (deactivate, reinstall,
            // export) persisted across restarts — separate from the scrollable session log
            // below, which only holds the current run's lines.
            this.pnlHistory.Dock         = System.Windows.Forms.DockStyle.Top;
            this.pnlHistory.Height       = 110;
            this.pnlHistory.Margin       = new System.Windows.Forms.Padding(12, 0, 12, 10);
            this.pnlHistory.Padding      = new System.Windows.Forms.Padding(12, 10, 12, 10);
            this.pnlHistory.BorderRadius = 10;

            this.lblHistoryHeader.AutoSize = true;
            this.lblHistoryHeader.Location = new System.Drawing.Point(0, 0);
            this.lblHistoryHeader.Font     = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);

            this.lblHistoryBody.Location = new System.Drawing.Point(0, 22);
            this.lblHistoryBody.Size     = new System.Drawing.Size(512, 72);
            this.lblHistoryBody.Font     = new System.Drawing.Font("Segoe UI", 8.5F);

            this.pnlHistory.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblHistoryHeader, this.lblHistoryBody });

            // ── Log ───────────────────────────────────────────────────────
            this.pnlLog.Dock         = System.Windows.Forms.DockStyle.Fill;
            this.pnlLog.Margin       = new System.Windows.Forms.Padding(12, 0, 12, 10);
            this.pnlLog.Padding      = new System.Windows.Forms.Padding(10);
            this.pnlLog.BorderRadius = 10;

            this.lblLogHeader.AutoSize = true;
            this.lblLogHeader.Dock     = System.Windows.Forms.DockStyle.Top;
            this.lblLogHeader.Height   = 20;
            this.lblLogHeader.Font     = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);

            this.rtbLog.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.ReadOnly  = true;
            this.rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLog.Font      = new System.Drawing.Font("Consolas", 8.5F);
            this.rtbLog.ScrollBars= System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            // Added before lblLogHeader so the header (Top) is resolved first and the log
            // (Fill) only takes what's left — same reversed-z-order rule as the content/log
            // split above.
            this.pnlLog.Controls.Add(this.rtbLog);
            this.pnlLog.Controls.Add(this.lblLogHeader);

            // ── Footer ────────────────────────────────────────────────────
            this.pnlFooter.Dock    = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Height  = 32;
            this.pnlFooter.Padding = new System.Windows.Forms.Padding(12, 0, 8, 0);

            this.lblStatus.AutoSize  = true;
            this.lblStatus.Dock      = System.Windows.Forms.DockStyle.Left;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatus.Text      = "Sẵn sàng";

            this.btnSettings.Text      = "⚙ Cài đặt";
            this.btnSettings.AutoSize  = true;
            this.btnSettings.Dock      = System.Windows.Forms.DockStyle.Right;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.Click    += new System.EventHandler(this.btnSettings_Click);

            this.pnlFooter.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblStatus, this.btnSettings });

            // Context menu
            this.tsmiDonate.Text     = "Donate";
            this.tsmiFeedback.Text   = "Góp ý / Báo lỗi";

            // Language row — a real VI|EN toggle switch instead of a disabled placeholder item.
            this.lblLangVi.Text      = "VI";
            this.lblLangVi.AutoSize  = true;
            this.lblLangVi.Location  = new System.Drawing.Point(0, 4);
            this.lblLangEn.Text      = "EN";
            this.lblLangEn.AutoSize  = true;
            this.lblLangEn.Location  = new System.Drawing.Point(66, 4);
            this.swLangToggle.Location     = new System.Drawing.Point(22, 0);
            this.swLangToggle.Size         = new System.Drawing.Size(40, 20);
            this.swLangToggle.CheckedChanged += new System.EventHandler(this.swLangToggle_CheckedChanged);
            this.pnlLangToggle.Size   = new System.Drawing.Size(92, 24);
            this.pnlLangToggle.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblLangVi, this.swLangToggle, this.lblLangEn });
            var langHost = new System.Windows.Forms.ToolStripControlHost(this.pnlLangToggle)
            {
                AutoSize = false,
                Size     = this.pnlLangToggle.Size,
                Margin   = new System.Windows.Forms.Padding(18, 2, 2, 2)
            };

            this.tsmiFixDns.Text  = "Đổi DNS sang Cloudflare/Google…";
            this.tsmiFixDns.Click += new System.EventHandler(this.tsmiFixDns_Click);

            // One compact line instead of four separate rows (version / .NET / UI / author).
            this.tsmiVersion.Text    = "v2.0.0 · .NET 4.7.2 · Guna.UI2 · ThanhNguyenVN93";
            this.tsmiVersion.Enabled = false;
            this.ctxSettings.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.tsmiDonate, this.tsmiFeedback, this.tsmiFixDns, langHost, this.tsmiSep, this.tsmiVersion });
            this.tsmiDonate.Click   += new System.EventHandler(this.tsmiDonate_Click);
            this.tsmiFeedback.Click += new System.EventHandler(this.tsmiFeedback_Click);

            // Tooltips
            this.toolTip.SetToolTip(this.btnExport1, "Lưu thông tin đăng ký ra file");
            this.toolTip.SetToolTip(this.btnExport3, "Lưu thông tin đăng ký ra file");

            // ── Add to form ───────────────────────────────────────────────
            // Dock layout resolves controls in reverse of this list (last entry claims its edge
            // first). pnlLog is Dock.Fill and must be resolved LAST so it only takes whatever
            // space pnlContent's and pnlHistory's fixed-height Top docks leave behind — hence
            // they come after pnlLog here, even though they're visually above it.
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.pnlLog,
                this.pnlHistory,
                this.pnlContent,
                this.pnlFooter,
                this.pnlBanner,
                this.pnlTitlebar
            });

            this.ResumeLayout(false);
        }

        // ── Layout helpers ─────────────────────────────────────────────────

        private static void SetThemeButton(Guna2Button btn, string text)
        {
            btn.Text         = text;
            btn.AutoSize     = true;
            btn.Height       = 28;
            btn.Padding      = new System.Windows.Forms.Padding(12, 0, 12, 0);
            btn.BorderRadius = 14;
            btn.Font         = new System.Drawing.Font("Segoe UI", 8F);
        }

        // Positions controls left-to-right using each one's own (already-measured) Width —
        // avoids the clipped-text / overlapping-button bugs that hard-coded x offsets caused
        // whenever a button's real width didn't match the guessed offset. Returns the total
        // width consumed (last control's right edge), handy for sizing the parent panel.
        private static int FlowHorizontally(int x, int y, int gap, params System.Windows.Forms.Control[] controls)
        {
            foreach (var c in controls)
            {
                c.Location = new System.Drawing.Point(x, y);
                x += c.Width + gap;
            }
            return x - gap;
        }

        private static void SetWinButton(System.Windows.Forms.Button btn, string text, int x)
        {
            btn.Text      = text;
            btn.Location  = new System.Drawing.Point(x, 0);
            btn.Size      = new System.Drawing.Size(28, 40);
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Anchor    = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        }

        // Stacked layout (label above, box below spanning the card width) — matches the mockup
        // and avoids the cramped look a label-left/box-right row had with Guna2TextBox's 36px
        // default height: that height didn't leave enough room in a 36px-tall row, so each row
        // visually ran into the next.
        private const int InfoRowPitch     = 64;
        private const int InfoFieldBoxH    = 32;
        private const int InfoTopInset     = 12;

        private static void AddInfoRow(System.Windows.Forms.Panel parent,
            System.Windows.Forms.Label lbl, string labelText,
            Guna2TextBox txt, int y)
        {
            lbl.AutoSize = true;
            lbl.Text     = labelText;
            lbl.Location = new System.Drawing.Point(12, y);

            txt.Location     = new System.Drawing.Point(12, y + 18);
            txt.Width        = 512;
            txt.Height        = InfoFieldBoxH;
            txt.BorderRadius = 6;
            txt.BorderThickness = 1;

            parent.Controls.AddRange(new System.Windows.Forms.Control[] { lbl, txt });
        }

        private static void AddActionButton(Guna2Button btn, string text)
        {
            btn.Text         = text;
            btn.Height       = 32;
            btn.AutoSize     = true;
            btn.BorderRadius = 8;
            btn.Padding      = new System.Windows.Forms.Padding(14, 0, 14, 0);
        }

        #endregion

        // Controls
        private System.Windows.Forms.Panel          pnlTitlebar;
        private System.Windows.Forms.Label          lblTitle;
        private System.Windows.Forms.Button         btnClose;
        private System.Windows.Forms.Button         btnMinimize;
        private System.Windows.Forms.Panel          pnlTheme;
        private Guna2Button                         btnThemeLight;
        private Guna2Button                         btnThemeAuto;
        private Guna2Button                         btnThemeDark;
        private Guna2Panel                          pnlBanner;
        private System.Windows.Forms.Label          lblBanner;
        private Guna2Panel                          pnlBannerBadge;
        private System.Windows.Forms.Label          lblBannerBadge;
        private System.Windows.Forms.Panel          pnlContent;
        private Guna2Panel                          pnlInfo;
        private System.Windows.Forms.Label          lblInfoHeader;
        private System.Windows.Forms.Label          lblFName;
        private Guna2TextBox                        txtFName;
        private System.Windows.Forms.Label          lblLName;
        private Guna2TextBox                        txtLName;
        private System.Windows.Forms.Label          lblEmailLbl;
        private Guna2TextBox                        txtEmail;
        private System.Windows.Forms.Label          lblSerialLbl;
        private Guna2TextBox                        txtSerial;
        private System.Windows.Forms.Label          lblExePathLbl;
        private System.Windows.Forms.Label          lblExePath;
        private System.Windows.Forms.Label          lblVersion;
        private System.Windows.Forms.Panel          pnlState1;
        private Guna2Button                         btnDeactivate;
        private Guna2Button                         btnCheckUpdate;
        private Guna2Button                         btnExport1;
        private System.Windows.Forms.Panel          pnlState2;
        private Guna2Button                         btnBuyLicense;
        private System.Windows.Forms.Panel          pnlState3;
        private Guna2Button                         btnReinstall;
        private Guna2Button                         btnExport3;
        private System.Windows.Forms.Panel          pnlState4;
        private Guna2Button                         btnDownloadIDM;
        private Guna2Panel                          pnlHistory;
        private System.Windows.Forms.Label          lblHistoryHeader;
        private System.Windows.Forms.Label          lblHistoryBody;
        private Guna2Panel                          pnlLog;
        private System.Windows.Forms.Label          lblLogHeader;
        private System.Windows.Forms.RichTextBox    rtbLog;
        private System.Windows.Forms.Panel          pnlFooter;
        private System.Windows.Forms.Label          lblStatus;
        private System.Windows.Forms.Button         btnSettings;
        private System.Windows.Forms.ContextMenuStrip       ctxSettings;
        private System.Windows.Forms.ToolStripMenuItem      tsmiDonate;
        private System.Windows.Forms.ToolStripMenuItem      tsmiFeedback;
        private System.Windows.Forms.ToolStripMenuItem      tsmiFixDns;
        private System.Windows.Forms.Panel                  pnlLangToggle;
        private System.Windows.Forms.Label                  lblLangVi;
        private System.Windows.Forms.Label                  lblLangEn;
        private Guna2ToggleSwitch                            swLangToggle;
        private System.Windows.Forms.ToolStripSeparator     tsmiSep;
        private System.Windows.Forms.ToolStripMenuItem      tsmiVersion;
        private System.Windows.Forms.ToolTip                toolTip;
    }
}
