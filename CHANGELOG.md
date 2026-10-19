# Changelog

## [2.0.0] — 2026-10-19

### Fixed
- **IDM detection**: the real bug behind "IDM installed but reported as missing" — the app was
  reading `ExePath` from `HKLM\SOFTWARE\Internet Download Manager`, but IDM actually writes it
  to `HKCU\Software\DownloadManager`. Detection now checks HKCU first, with HKLM, the running
  process, the uninstall registry key, and default install paths as independent fallbacks.
- Close/Minimize titlebar buttons disappearing off-screen (an `Anchor = Top|Right` snapshot
  taken before the parent panel had its real size).
- A large empty gap between the action buttons and the log panel (`Dock = Fill` on a panel
  whose content was far shorter than the remaining form height).
- Reinstall flow could write activation registry values even when the silent install didn't
  actually place `IDMan.exe` on disk; it now verifies the exe exists first.
- IDM installer download buffered the whole file in memory and ignored its own cancellation
  token; now streams straight to disk and honors cancellation.
- `netsh` DNS-switch command didn't escape the network adapter name.
- Downloaded installer `.exe` was never cleaned up from `%TEMP%` after install.
- A shared `CancellationTokenSource` field could be clobbered by two concurrent operations
  (Check Update vs. Reinstall); each operation now uses its own, and action buttons are
  disabled while one is running.
- `.NET Framework missing` dialog always exited the app regardless of which button was
  pressed, including on a false negative (e.g. registry blocked by Group Policy on a machine
  that actually has 4.7.2+).
- Assorted resource-leak and robustness fixes: undisposed `Process` handles, `PictureBox.Image`
  not disposed with the dialog, `Process.Start(url)` calls with no exception handling,
  duplicate registry/disk reads on every state refresh.

### Added
- UI rebuilt on Guna.UI2.WinForms — rounded cards, pill status badge, rounded buttons/inputs,
  flow-based button layout (positions computed from each button's real measured width instead
  of hard-coded offsets).
- Bilingual UI (Vietnamese/English) with a live toggle switch.
- Recent History card — persists the last state-changing actions across restarts.
- Colored, timestamped activity log with an empty-state placeholder.
- Donate dialog with an embedded MoMo QR code and a Ko-fi button.
- Read-only network diagnostics (current DNS, ping to IDM's site) logged on launch, plus an
  explicit, confirmation-gated "Switch DNS to Cloudflare/Google" action.
- `.NET Framework 4.7.2` prerequisite check on startup with a clear message and download link.
- AES-256 encrypted `.idmbak` export.
- 4-state detection matrix (registered / installed-only / serial-only / clean) with the
  matching action row for each.
- One-click reinstall flow: clear stale registry → download latest IDM → silent install
  (elevated) → restore registration.

## [1.0.0] — Original

- `d19d3651-CheckIDM.exe` (.NET 4.0, 17,920 bytes).
