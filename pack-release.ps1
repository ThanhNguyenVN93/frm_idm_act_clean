<#
.SYNOPSIS
  Builds the Release configuration and merges Guna.UI2.dll into the output exe with ILRepack,
  producing a single dependency-free IDMActClean.exe in .\release\.
#>

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
Set-Location $root

function Find-MSBuild {
    $candidates = Get-ChildItem "C:\Program Files*\Microsoft Visual Studio" -Recurse -Filter MSBuild.exe -ErrorAction SilentlyContinue
    if (-not $candidates) { throw "MSBuild.exe not found. Install Visual Studio Build Tools." }
    return $candidates[0].FullName
}

function Find-Nuget {
    $local = Join-Path $root "tools\nuget.exe"
    if (Test-Path $local) { return $local }
    New-Item -ItemType Directory -Force -Path (Split-Path $local) | Out-Null
    Write-Host "Downloading nuget.exe..."
    Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $local -UseBasicParsing
    return $local
}

function Find-ILRepack {
    $exe = Get-ChildItem (Join-Path $root "tools\ILRepack*\tools\ilrepack.exe") -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($exe) { return $exe.FullName }

    $nuget = Find-Nuget
    Write-Host "Installing ILRepack via NuGet..."
    & $nuget install ILRepack -ExcludeVersion -OutputDirectory (Join-Path $root "tools") -NonInteractive | Out-Null
    $exe = Get-ChildItem (Join-Path $root "tools\ILRepack*\tools\ilrepack.exe") -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $exe) { throw "ILRepack install failed — ilrepack.exe not found after install." }
    return $exe.FullName
}

Write-Host "=== 1/3: Restoring NuGet packages ==="
$nuget = Find-Nuget
if (-not (Test-Path "$root\packages\Guna.UI2.WinForms.2.0.4.6\lib\net40\Guna.UI2.dll")) {
    & $nuget restore "$root\packages.config" -PackagesDirectory "$root\packages" -NonInteractive
}

Write-Host "=== 2/3: Building Release ==="
$msbuild = Find-MSBuild
& $msbuild "$root\frm_idm_act_clean.csproj" /t:Build /p:Configuration=Release /nologo /v:minimal
if ($LASTEXITCODE -ne 0) { throw "Build failed." }

Write-Host "=== 3/3: Merging into a single exe with ILRepack ==="
$ilrepack = Find-ILRepack
$releaseDir = Join-Path $root "release"
New-Item -ItemType Directory -Force -Path $releaseDir | Out-Null

$binDir = Join-Path $root "bin\Release"
& $ilrepack `
    /out:"$releaseDir\IDMActClean.exe" `
    /target:winexe `
    /lib:"$binDir" `
    "$binDir\IDMActClean.exe" `
    "$binDir\Guna.UI2.dll"

if ($LASTEXITCODE -ne 0) { throw "ILRepack merge failed." }

Write-Host ""
Write-Host "Done: $releaseDir\IDMActClean.exe" -ForegroundColor Green
