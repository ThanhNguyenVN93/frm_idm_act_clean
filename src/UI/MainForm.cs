using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IDMActClean.Core;

namespace IDMActClean.UI
{
    public partial class MainForm : Form
    {
        private AppTheme _theme = AppTheme.Auto;
        private AppLanguage _lang = AppLanguage.Vi;
        private IdmState _state;
        private bool _logHasContent;

        public MainForm()
        {
            InitializeComponent();
            ApplyTheme();
            // ApplyLanguage() already calls RefreshHistory() and (when the log is still empty)
            // ShowLogPlaceholder() at its end — calling them again here was pure duplicate I/O
            // (re-reading history.log a second time) on every startup.
            ApplyLanguage();
            _ = LoadStateAsync();
        }

        // ── Language toggle ───────────────────────────────────────────────
        // Covers the persistent UI chrome (labels, buttons, banner, status, menu). Log history
        // and confirmation dialogs stay Vietnamese-only — translating those too means threading
        // the language through every call site that logs or confirms, which is a much bigger
        // change than "add a toggle"; flag it if you want that follow-up done as well.

        private enum AppLanguage { Vi, En }

        private string S(string vi, string en) => _lang == AppLanguage.En ? en : vi;

        private void swLangToggle_CheckedChanged(object sender, EventArgs e)
        {
            _lang = swLangToggle.Checked ? AppLanguage.En : AppLanguage.Vi;
            ApplyLanguage();
            // Re-render the banner/badge/status text for the new language. A fresh registry read
            // here (rather than threading cached values through) is fine — this only runs on an
            // infrequent, user-initiated toggle, not the hot startup/refresh path.
            RefreshUI(IdmHelper.GetRegistrationInfo(), IdmHelper.GetIDMExePath(), IdmHelper.GetIDMVersion());
        }

        private void ApplyLanguage()
        {
            lblFName.Text      = S("Tên (FName)", "First Name (FName)");
            lblLName.Text      = S("Họ (LName)", "Last Name (LName)");
            lblEmailLbl.Text   = S("E-mail", "E-mail");
            lblSerialLbl.Text  = S("Serial", "Serial");
            lblExePathLbl.Text = S("Đường dẫn:", "Path:");

            btnThemeLight.Text = S("Sáng", "Light");
            btnThemeDark.Text  = S("Tối", "Dark");
            int themeWidth = FlowHorizontally(0, 0, 6, btnThemeLight, btnThemeAuto, btnThemeDark);
            pnlTheme.Width  = themeWidth;

            btnExport1.Text    = S("Xuất thông tin", "Export Info");
            btnExport3.Text    = btnExport1.Text;
            btnBuyLicense.Text = S("→ Mua bản quyền chính hãng", "→ Buy a genuine license");
            btnReinstall.Text  = S("Cài lại & Kích hoạt", "Reinstall & Activate");
            FlowHorizontally(0, 6, 16, btnDeactivate, btnCheckUpdate, btnExport1);
            FlowHorizontally(0, 6, 16, btnReinstall, btnExport3);

            tsmiFeedback.Text = S("Góp ý / Báo lỗi", "Feedback / Report issue");
            tsmiFixDns.Text   = S("Đổi DNS sang Cloudflare/Google…", "Switch DNS to Cloudflare/Google…");
            string exportTip = S("Lưu thông tin đăng ký ra file", "Save registration info to a file");
            toolTip.SetToolTip(btnExport1, exportTip);
            toolTip.SetToolTip(btnExport3, exportTip);

            lblHistoryHeader.Text = S("Lịch sử gần đây", "Recent history");
            lblLogHeader.Text     = S("Nhật ký hoạt động", "Activity log");
            RefreshHistory();
            if (!_logHasContent) ShowLogPlaceholder();
        }

        // ── State detection ────────────────────────────────────────────────

        private async Task LoadStateAsync()
        {
            Log("Đang phát hiện trạng thái…");

            // Fetched once here and handed to RefreshUI/LogState — they used to each call
            // IdmHelper.GetRegistrationInfo/GetIDMExePath/GetIDMVersion independently, meaning
            // every state refresh did the registry + disk I/O twice for the same answer.
            IdmRegistrationInfo info = null;
            string path = null, ver = null;
            await Task.Run(() =>
            {
                _state = IdmHelper.DetectState();
                info   = IdmHelper.GetRegistrationInfo();
                path   = IdmHelper.GetIDMExePath();
                ver    = IdmHelper.GetIDMVersion();
            });

            RefreshUI(info, path, ver);
            LogState(info, path, ver);
            _ = LogNetworkDiagnosticsAsync();
        }

        // Read-only network diagnostic (current DNS + a ping to IDM's site) — helps explain a
        // failed Check Update without touching any system setting. Changing DNS is a separate,
        // explicit, user-confirmed action (see tsmiFixDns_Click), never automatic.
        private async Task LogNetworkDiagnosticsAsync()
        {
            var dns = await Task.Run(() => NetworkDiagnostics.GetPrimaryDnsServers());
            if (dns.Count == 0)
            {
                Log(S("Không xác định được DNS hiện tại.", "Could not determine the current DNS."));
            }
            else
            {
                bool isPublic = NetworkDiagnostics.IsUsingPublicDns(dns);
                string suffix = isPublic ? S(" (đã dùng Cloudflare/Google)", " (already Cloudflare/Google)") : "";
                Log(S($"DNS hiện tại: {string.Join(", ", dns)}{suffix}",
                      $"Current DNS: {string.Join(", ", dns)}{suffix}"));
            }

            var ping = await NetworkDiagnostics.PingHostAsync("www.internetdownloadmanager.com", 3000);
            Log(ping.Success
                ? S($"Ping internetdownloadmanager.com: {ping.RoundtripMs}ms — OK",
                    $"Ping internetdownloadmanager.com: {ping.RoundtripMs}ms — OK")
                : S("Ping internetdownloadmanager.com: thất bại — kiểm tra kết nối mạng hoặc DNS.",
                    "Ping internetdownloadmanager.com: failed — check your network or DNS."));
        }

        private void LogState(IdmRegistrationInfo info, string path, string ver)
        {
            Log($"Serial: {(string.IsNullOrEmpty(info.Serial) ? "(trống)" : "có dữ liệu")}");
            Log($"IDMan.exe: {(path != null ? path : "(không tìm thấy)")}");
            if (ver != null) Log($"Phiên bản: {ver}");
        }

        private void RefreshUI(IdmRegistrationInfo info, string path, string ver)
        {
            if (InvokeRequired) { Invoke(new Action(() => RefreshUI(info, path, ver))); return; }

            txtFName.Text  = info.FName;
            txtLName.Text  = info.LName;
            txtEmail.Text  = info.Email;
            txtSerial.Text = info.Serial;
            lblExePath.Text = path ?? "(chưa cài IDM)";
            lblVersion.Text = ver  ?? "";

            // Banner
            switch (_state)
            {
                case IdmState.Registered:
                    ShowBanner(S("✔  IDM đã kích hoạt hợp lệ", "✔  IDM is activated"), BannerKind.Ok);
                    break;
                case IdmState.InstalledOnly:
                    ShowBanner(S("⚠  IDM đã cài nhưng chưa đăng ký — khuyến cáo mua bản quyền chính hãng",
                                 "⚠  IDM installed but not registered — consider buying a genuine license"), BannerKind.Warn);
                    break;
                case IdmState.SerialOnly:
                    ShowBanner(S("⚠  Tìm thấy serial nhưng IDM chưa được cài đặt",
                                 "⚠  Serial found but IDM is not installed"), BannerKind.Warn);
                    break;
                case IdmState.Clean:
                    ShowBanner(S("ℹ  Máy sạch — chưa cài IDM", "ℹ  Clean machine — IDM not installed"), BannerKind.Info);
                    break;
            }

            // Button visibility
            pnlState1.Visible = _state == IdmState.Registered;
            pnlState2.Visible = _state == IdmState.InstalledOnly;
            pnlState3.Visible = _state == IdmState.SerialOnly;
            pnlState4.Visible = _state == IdmState.Clean;

            lblStatus.Text = S("Sẵn sàng", "Ready");
        }

        // ── Banner ─────────────────────────────────────────────────────────

        private enum BannerKind { Ok, Warn, Err, Info }

        private void ShowBanner(string text, BannerKind kind)
        {
            var p = ThemeTokens.Resolve(_theme);
            lblBanner.Text = text;
            switch (kind)
            {
                case BannerKind.Ok:   pnlBanner.FillColor = p.Ok;   break;
                case BannerKind.Warn: pnlBanner.FillColor = p.Warn; break;
                case BannerKind.Err:  pnlBanner.FillColor = p.Err;  break;
                default:              pnlBanner.FillColor = p.Info;  break;
            }
            lblBanner.ForeColor = Color.White;

            lblBannerBadge.Text = BadgeTextFor(kind);
            lblBannerBadge.ForeColor = Color.White;
            pnlBannerBadge.FillColor = Color.FromArgb(55, 0, 0, 0);
        }

        private string BadgeTextFor(BannerKind kind)
        {
            switch (kind)
            {
                case BannerKind.Ok:   return S("Đã kích hoạt", "Activated");
                case BannerKind.Warn: return S("Chưa kích hoạt", "Not activated");
                case BannerKind.Err:  return S("Lỗi", "Error");
                default:              return S("Chưa cài đặt", "Not installed");
            }
        }

        // ── State 1: Deactivate ────────────────────────────────────────────

        private async void btnDeactivate_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xác nhận gỡ kích hoạt IDM?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            SetBusy(true);
            Log("Đang dừng IDMan.exe…");
            await Task.Run(() => IdmHelper.KillIDM());

            Log("Đang xóa registry…");
            var result = await Task.Run(() => RegistryHelper.DeleteActivationValues());

            if (result.HkcuSuccess)
                Log("Đã xóa 7 giá trị HKCU thành công.");
            else if (result.HkcuError != null)
                Log($"Lỗi HKCU: {result.HkcuError}");

            if (result.HklmNeedsUac)
                Log("⚠ HKLM: cần quyền Admin — vui lòng chạy lại với Run as Administrator.");
            else if (result.HklmSuccess)
                Log("Đã xóa ExePath HKLM.");
            else if (result.HklmError != null)
                Log($"Lỗi HKLM: {result.HklmError}");

            Log("Deactivate hoàn tất.");
            ActivityHistory.Add(S("Gỡ kích hoạt IDM", "Deactivated IDM"));
            RefreshHistory();
            SetBusy(false);
            await LoadStateAsync();
        }

        private async void btnCheckUpdate_Click(object sender, EventArgs e)
        {
            SetBusy(true);
            Log("Đang kiểm tra phiên bản mới…");

            UpdateResult result;
            using (var cts = new CancellationTokenSource(15_000))
                result = await UpdateChecker.CheckAsync(cts.Token);

            if (result.Error != null)
                Log($"Lỗi: {result.Error}");
            else if (result.IsLatest)
                Log($"IDM đang dùng phiên bản mới nhất ({result.CurrentVersion}).");
            else
                Log($"Phát hiện phiên bản mới: {result.LatestFilename}  (hiện tại: {result.CurrentVersion})");

            SetBusy(false);
        }

        private void btnExport1_Click(object sender, EventArgs e) => DoExport();

        // ── State 2 ────────────────────────────────────────────────────────

        private void btnBuyLicense_Click(object sender, EventArgs e) =>
            OpenUrl("https://www.internetdownloadmanager.com/buy.html");

        // ── State 3 ────────────────────────────────────────────────────────

        private async void btnReinstall_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Tự động tải và cài lại IDM rồi kích hoạt lại?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            SetBusy(true);
            var info = IdmHelper.GetRegistrationInfo();
            string setupPath = null;
            try
            {
                Log("[1/4] Dọn registry…");
                await Task.Run(() => RegistryHelper.DeleteActivationValues());
                Log("Xong.");

                Log("[2/4] Tải IDM…");
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120)))
                    setupPath = await DownloadIDMAsync(cts.Token);
                if (setupPath == null) return;
                Log($"Đã tải: {setupPath}");

                Log("[3/4] Cài đặt silent…");
                bool installed = await InstallIDMAsync(setupPath);
                if (!installed) { Log("Cài thất bại."); return; }
                Log("Cài thành công.");

                Log("[4/4] Kích hoạt lại…");
                // Confirm IDMan.exe actually exists on disk before writing registration values —
                // without this, a silent-install that reported success but didn't actually place
                // the exe (interrupted, blocked by AV, etc.) would still leave Serial/FName/...
                // written to the registry, matching a "Registered" state the app can't back up.
                if (IdmHelper.GetIDMExePath() == null)
                {
                    Log(S("⚠ Cài xong nhưng không tìm thấy IDMan.exe — bỏ qua kích hoạt lại.",
                          "⚠ Install finished but IDMan.exe wasn't found — skipping reactivation."));
                }
                else
                {
                    try
                    {
                        RegistryHelper.WriteActivationValues(info);
                        Log("Kích hoạt lại hoàn tất.");
                        ActivityHistory.Add(S("Cài lại & kích hoạt IDM", "Reinstalled & activated IDM"));
                        RefreshHistory();
                    }
                    catch (Exception ex)
                    {
                        Log($"Lỗi kích hoạt: {ex.Message}");
                    }
                }
            }
            finally
            {
                if (setupPath != null)
                {
                    try { if (System.IO.File.Exists(setupPath)) System.IO.File.Delete(setupPath); }
                    catch { /* best-effort cleanup of the downloaded installer */ }
                }
                SetBusy(false);
            }

            await LoadStateAsync();
        }

        private void btnExport3_Click(object sender, EventArgs e) => DoExport();

        // ── State 4 ────────────────────────────────────────────────────────

        private void btnDownloadIDM_Click(object sender, EventArgs e) =>
            OpenUrl("https://www.internetdownloadmanager.com/download.html");

        // ── Download / install helpers ─────────────────────────────────────

        // Streams the download straight to disk instead of buffering the whole ~10MB installer
        // in a byte[], and actually honors `ct` (the old version created a CancellationTokenSource
        // but never passed its token into GetByteArrayAsync, so it did nothing).
        private async Task<string> DownloadIDMAsync(CancellationToken ct)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient { Timeout = Timeout.InfiniteTimeSpan })
                {
                    string html;
                    using (var listReq = new System.Net.Http.HttpRequestMessage(
                               System.Net.Http.HttpMethod.Get, "https://www.internetdownloadmanager.com/download.html"))
                    using (var listResp = await client.SendAsync(listReq, ct).ConfigureAwait(false))
                    {
                        listResp.EnsureSuccessStatusCode();
                        html = await listResp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }

                    var m = System.Text.RegularExpressions.Regex.Match(
                        html, @"href=""([^""]*idman\d+build\d+\.exe)""",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    if (!m.Success) { Log("Không tìm thấy link tải."); return null; }

                    string url = m.Groups[1].Value;
                    if (!url.StartsWith("http")) url = "https://www.internetdownloadmanager.com/" + url.TrimStart('/');

                    Log($"Đang tải từ: {url}");
                    string dest = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
                        $"idm_setup_{DateTime.Now:yyyyMMddHHmmss}.exe");

                    using (var dlReq = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url))
                    using (var dlResp = await client.SendAsync(dlReq,
                               System.Net.Http.HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
                    {
                        dlResp.EnsureSuccessStatusCode();
                        using (var httpStream = await dlResp.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        using (var fileStream = System.IO.File.Create(dest))
                        {
                            await httpStream.CopyToAsync(fileStream, 81920, ct).ConfigureAwait(false);
                        }
                    }
                    return dest;
                }
            }
            catch (OperationCanceledException)
            {
                Log(S("Tải IDM bị huỷ do hết thời gian chờ.", "Download timed out / was cancelled."));
                return null;
            }
            catch (Exception ex)
            {
                Log($"Lỗi tải: {ex.Message}");
                return null;
            }
        }

        private async Task<bool> InstallIDMAsync(string exePath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var p = Process.Start(new ProcessStartInfo(exePath, "/silent")
                    {
                        UseShellExecute = true,
                        Verb = "runas"
                    }))
                    {
                        return p != null && p.WaitForExit(120_000) && p.ExitCode == 0;
                    }
                }
                catch (Exception ex)
                {
                    Log($"Lỗi cài đặt: {ex.Message}");
                    return false;
                }
            });
        }

        // ── Export ─────────────────────────────────────────────────────────

        private void DoExport()
        {
            var info = IdmHelper.GetRegistrationInfo();
            if (string.IsNullOrEmpty(info.Serial))
            {
                MessageBox.Show("Không có thông tin serial để xuất.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string path = ExportHelper.ExportToFile(info);
            if (path != null)
            {
                Log($"Đã xuất: {path}");
                ActivityHistory.Add(S("Xuất thông tin đăng ký", "Exported registration info"));
                RefreshHistory();
            }
        }

        // ── Theme toggle ───────────────────────────────────────────────────

        private void btnThemeLight_Click(object sender, EventArgs e) { _theme = AppTheme.Light;  ApplyTheme(); }
        private void btnThemeDark_Click(object sender, EventArgs e)  { _theme = AppTheme.Dark;   ApplyTheme(); }
        private void btnThemeAuto_Click(object sender, EventArgs e)  { _theme = AppTheme.Auto;   ApplyTheme(); }

        private void ApplyTheme()
        {
            var p = ThemeTokens.Resolve(_theme);

            BackColor                    = p.Bg;
            pnlContent.BackColor         = p.Bg;
            pnlTitlebar.BackColor        = p.Surface;
            pnlInfo.FillColor            = p.Surface;
            pnlInfo.BorderColor          = p.Border;
            pnlHistory.FillColor         = p.Surface;
            pnlHistory.BorderColor       = p.Border;
            pnlLog.FillColor             = p.Surface;
            pnlLog.BorderColor           = p.Border;
            pnlFooter.BackColor          = p.Surface;

            lblTitle.ForeColor           = p.Text;
            lblInfoHeader.ForeColor      = p.SubText;
            lblFName.ForeColor           = p.SubText;
            lblLName.ForeColor           = p.SubText;
            lblEmailLbl.ForeColor        = p.SubText;
            lblSerialLbl.ForeColor       = p.SubText;
            lblExePathLbl.ForeColor      = p.SubText;
            lblExePath.ForeColor         = p.Text;
            lblVersion.ForeColor         = p.SubText;
            lblHistoryHeader.ForeColor   = p.SubText;
            lblHistoryBody.ForeColor     = p.SubText;
            lblLogHeader.ForeColor       = p.SubText;

            foreach (var tb in new[] { txtFName, txtLName, txtEmail, txtSerial })
            {
                tb.FillColor   = p.Bg;
                tb.ForeColor   = p.Text;
                tb.BorderColor = p.Border;
            }
            txtSerial.ForeColor = p.Accent;

            rtbLog.BackColor             = p.Surface;
            rtbLog.ForeColor             = p.SubText;

            lblStatus.ForeColor          = p.SubText;

            // Theme buttons active state
            btnThemeLight.FillColor = _theme == AppTheme.Light ? p.Accent : p.Border;
            btnThemeDark.FillColor  = _theme == AppTheme.Dark  ? p.Accent : p.Border;
            btnThemeAuto.FillColor  = _theme == AppTheme.Auto  ? p.Accent : p.Border;
            btnThemeLight.ForeColor = _theme == AppTheme.Light ? p.BtnText : p.Text;
            btnThemeDark.ForeColor  = _theme == AppTheme.Dark  ? p.BtnText : p.Text;
            btnThemeAuto.ForeColor  = _theme == AppTheme.Auto  ? p.BtnText : p.Text;
        }

        // ── Log helper ─────────────────────────────────────────────────────
        // Plain uniform-color text read as a flat wall compared to the mockup's colored log
        // lines. Color-codes by cues already present in the message text (⚠, "lỗi", "thành
        // công"/"hoàn tất") instead of threading a severity parameter through every call site.

        private void Log(string message)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Log), message); return; }
            var p = ThemeTokens.Resolve(_theme);

            if (!_logHasContent) { rtbLog.Clear(); _logHasContent = true; }

            AppendColored($"  {DateTime.Now:HH:mm:ss}  ", p.SubText);
            AppendColored(message + "\n", LogMessageColor(message, p));
            rtbLog.ScrollToCaret();
        }

        private void ShowLogPlaceholder()
        {
            var p = ThemeTokens.Resolve(_theme);
            rtbLog.Clear();
            AppendColored(S("—  Chưa có hoạt động nào  —", "—  No activity yet  —"), p.SubText);
            _logHasContent = false;
        }

        // ── Recent activity history ───────────────────────────────────────

        private void RefreshHistory()
        {
            var entries = ActivityHistory.GetRecent(4);
            lblHistoryBody.ForeColor = ThemeTokens.Resolve(_theme).SubText;
            lblHistoryBody.Text = entries.Count == 0
                ? S("—  Chưa có hoạt động nào  —", "—  No activity yet  —")
                : string.Join("\n", entries.Select(x => $"{x.Time:dd/MM HH:mm}   {x.Action}"));
        }

        private static Color LogMessageColor(string message, Palette p)
        {
            string m = message.ToLowerInvariant();
            if (message.StartsWith("⚠") || m.Contains("lỗi") || m.Contains("thất bại")) return p.Warn;
            if (m.Contains("hoàn tất") || m.Contains("thành công") || m.Contains("đã xóa")) return p.Ok;
            return p.Text;
        }

        private void AppendColored(string text, Color color)
        {
            rtbLog.SelectionStart  = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor  = color;
            rtbLog.AppendText(text);
        }

        private void SetBusy(bool busy)
        {
            if (InvokeRequired) { Invoke(new Action<bool>(SetBusy), busy); return; }
            lblStatus.Text   = busy ? S("Đang xử lý…", "Processing…") : S("Sẵn sàng", "Ready");
            Cursor           = busy ? Cursors.WaitCursor : Cursors.Default;

            // Previously only the cursor/label changed here, so nothing stopped e.g. clicking
            // Check Update while a Reinstall was mid-flight — two concurrent operations racing
            // over the same registry keys / shared state. Disable the action buttons for the
            // duration instead.
            bool enabled = !busy;
            foreach (var btn in new Guna.UI2.WinForms.Guna2Button[]
                     { btnDeactivate, btnCheckUpdate, btnExport1, btnBuyLicense,
                       btnReinstall, btnExport3, btnDownloadIDM })
                btn.Enabled = enabled;
        }

        // ── Titlebar drag (borderless form) ───────────────────────────────

        private bool _dragging;
        private Point _dragStart;

        private void pnlTitlebar_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging  = true;
            _dragStart = e.Location;
        }

        private void pnlTitlebar_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging) return;
            Left += e.X - _dragStart.X;
            Top  += e.Y - _dragStart.Y;
        }

        private void pnlTitlebar_MouseUp(object sender, MouseEventArgs e) => _dragging = false;

        private void btnClose_Click(object sender, EventArgs e)    => Close();
        private void btnMinimize_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        // ── Settings dropdown ──────────────────────────────────────────────

        private void btnSettings_Click(object sender, EventArgs e)
        {
            ctxSettings.Show(btnSettings, new Point(0, -ctxSettings.Height));
        }

        private void tsmiDonate_Click(object sender, EventArgs e)
        {
            using (var f = new DonateForm(_theme, _lang == AppLanguage.En))
                f.ShowDialog(this);
        }

        private void tsmiFeedback_Click(object sender, EventArgs e) =>
            OpenUrl("https://github.com/ThanhNguyenVN93/frm_idm_act_clean/issues");

        // Process.Start(url) throws (Win32Exception) when there's no handler registered for the
        // URL scheme — rare, but an unhandled exception here would crash the whole app since
        // these are UI event handlers with no surrounding try/catch of their own.
        private void OpenUrl(string url)
        {
            try { Process.Start(url); }
            catch (Exception ex)
            {
                Log(S($"Không mở được trình duyệt: {ex.Message}", $"Could not open the browser: {ex.Message}"));
            }
        }

        private async void tsmiFixDns_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                S("Đổi DNS của mạng hiện tại sang 1.1.1.1 (Cloudflare) và 8.8.8.8 (Google)?\n\nCần quyền Admin — sẽ hiện hộp thoại UAC.",
                  "Switch the current network's DNS to 1.1.1.1 (Cloudflare) and 8.8.8.8 (Google)?\n\nRequires Admin rights — a UAC prompt will appear."),
                S("Xác nhận", "Confirm"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            string adapter = NetworkDiagnostics.GetPrimaryAdapterName();
            if (adapter == null)
            {
                MessageBox.Show(S("Không tìm thấy card mạng đang hoạt động.", "No active network adapter found."),
                    S("Lỗi", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetBusy(true);
            Log(S($"Đang đổi DNS cho \"{adapter}\"…", $"Switching DNS for \"{adapter}\"…"));
            var result = await Task.Run(() => NetworkDiagnostics.SetPublicDns(adapter));

            if (result.Success)
            {
                Log(S("Đã đổi DNS thành công.", "DNS switched successfully."));
                await LogNetworkDiagnosticsAsync();
            }
            else if (result.Error == "UAC_DECLINED")
            {
                Log(S("Đã huỷ — bạn từ chối cấp quyền Admin.", "Cancelled — Admin permission was declined."));
            }
            else
            {
                Log(S($"Đổi DNS thất bại: {result.Error}", $"Failed to switch DNS: {result.Error}"));
            }
            SetBusy(false);
        }
    }
}
