# IDM Check Tool

A WinForms utility (.NET Framework 4.7.2) that detects Internet Download Manager's
activation state on the current machine and offers the matching next step — deactivate,
reinstall & reactivate, buy a genuine license, or download IDM — with a modern, rounded,
dark/light-themed UI.

> ⚠️ This tool reads/writes IDM's own registration registry keys. It does not generate or
> distribute license keys. Supporting genuine IDM is always the recommended path when a
> valid license is available.

## Download

Grab the latest single-file `IDMActClean.exe` from the
[Releases page](https://github.com/ThanhNguyenVN93/frm_idm_act_clean/releases) — no installer,
no separate DLLs, just run it. See [Requirements](#requirements) below.

## What it does

The app reads `HKCU\Software\DownloadManager` and IDM's install-location registry keys to
classify the machine into one of four states, then shows the one action row that applies:

| State | Serial present | IDM installed | Action shown |
|---|---|---|---|
| Registered | ✅ | ✅ | Deactivate · Check Update · Export Info |
| Installed only | ❌ | ✅ | Buy a genuine license |
| Serial only | ✅ | ❌ | Reinstall & Activate · Export Info |
| Clean | ❌ | ❌ | Download IDM |

## Features

- **Rounded, theme-aware UI** built on [Guna.UI2.WinForms](https://github.com/Saadi-Khan/Guna.UI2) —
  dark / light / auto (follows Windows theme) via a pill toggle in the titlebar.
- **Robust IDM detection** — checks `HKCU\Software\DownloadManager\ExePath` (where IDM
  actually writes it), falls back to HKLM, the running process, the uninstall registry key,
  and default install paths. Doesn't depend on whether IDM is set to launch with Windows.
- **Bilingual UI (VI/EN)** — a toggle switch in Settings instantly swaps the persistent UI
  chrome (labels, buttons, banner, status) between Vietnamese and English.
- **Colored, timestamped activity log** with an empty-state placeholder, plus a separate
  **Recent History** card that persists the last few state-changing actions
  (`%AppData%\IDMActClean\history.log`) across restarts.
- **AES-256 encrypted export** (`.idmbak`) of the current registration info, keyed off the
  machine name + username.
- **One-click reinstall flow** (state 3): deletes stale registry data, downloads the latest
  IDM installer from the official site, silently installs it (elevated), then restores the
  registration info.
- **Read-only network diagnostics** on launch — current DNS servers, whether they're already
  Cloudflare/Google, and a ping to `internetdownloadmanager.com` — logged alongside the IDM
  check. An explicit, confirmation-gated "Switch DNS to Cloudflare/Google…" menu item is
  available separately; nothing about networking changes automatically.
- **.NET Framework prerequisite check** — on a machine missing .NET 4.7.2+ (common on
  Windows 7 or a minimal Windows 10/11 install), shows a clear message with a link to the
  official download instead of letting Windows refuse to launch the exe with no explanation.
- **Donate dialog** — embedded MoMo QR code plus a Ko-fi button, shown as a small dialog
  instead of leaving the app to a browser immediately.

## Requirements

- Windows 7 or later
- .NET Framework 4.7.2 or newer (the app detects and warns if it's missing)

## Building from source

1. Open `frm_idm_act_clean.slnx` in Visual Studio (not the `.csproj` directly — NuGet package
   management needs a saved solution).
2. Restore NuGet packages (right-click the solution → *Restore NuGet Packages*) to pull down
   `Guna.UI2.WinForms`.
3. Build (`Debug` or `Release`, `Any CPU`).

A regular build's output exe depends on `Guna.UI2.dll` sitting next to it. For a single
dependency-free exe (what's published on the Releases page), see **Packaging a release** below.

## Packaging a release

```bash
pack-release.ps1
```

This builds in `Release` config, then uses [ILRepack](https://github.com/gluck/il-repack) to
merge `Guna.UI2.dll` into `IDMActClean.exe`, producing a single self-contained exe at
`release\IDMActClean.exe`. See [Distribution](#distribution--why-guna.ui2-stays) for why this
merges Guna.UI2 in rather than dropping it.

## Project layout

```
src/
  Core/              Registry, detection, export, update-check, network, and history logic
    IdmHelper.cs          State detection + IDM exe/version lookup
    RegistryHelper.cs     Activation value read/write/delete
    ExportHelper.cs       AES-256 .idmbak export
    UpdateChecker.cs      Checks the official site for a newer IDM build
    DotNetChecker.cs      .NET Framework 4.7.2 prerequisite check
    NetworkDiagnostics.cs DNS read, ping, and (explicit, confirmed) DNS switch
    ActivityHistory.cs    Persisted recent-actions log
  UI/
    MainForm.cs            Event handlers, theme/language application, logging
    MainForm.Designer.cs   Layout (Guna.UI2 controls, flow-based positioning)
    ThemeTokens.cs         Light/dark color palette
    DonateForm.cs          QR + Ko-fi donate dialog
  Program.cs            Entry point + .NET Framework prerequisite gate
pack-release.ps1       Builds Release and ILRepack-merges it into one exe
```

## Distribution — why Guna.UI2 stays

Considered dropping Guna.UI2 for a dependency-free, plain-WinForms build. Decided against it:
reproducing the rounded panels/buttons/textboxes without the library means hand-rolling GDI+
custom painting — effectively rebuilding a chunk of what Guna.UI2 already provides — and a
plain-WinForms fallback would lose the rounded/card look entirely. Instead, the released exe
merges `Guna.UI2.dll` into the main exe at package time with **ILRepack** (see
**Packaging a release** above), so end users get one file with no loose DLL.

## Localization

The UI currently ships with Vietnamese and English built in, toggled live from Settings.
Contributing a new language isn't wired up yet — planned approach: an "export translation
template" button producing an English-reference JSON file, which the app will auto-detect
from a local `lang/` folder for personal use, and which can be submitted as a GitHub PR for
inclusion in a future release (not auto-fetched at runtime).

## Support

- Feedback / bug reports: [GitHub Issues](https://github.com/ThanhNguyenVN93/frm_idm_act_clean/issues)
- Donate: via the in-app Donate dialog (MoMo QR or [Ko-fi](https://ko-fi.com/thanhnguyen150993))

## License

[MIT](LICENSE)

## Author

ThanhNguyenVN93
