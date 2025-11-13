# 📋 Changelog

All notable changes to **L2 Setup** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/), and this project adheres to [Semantic Versioning](https://semver.org/).

---

## [1.1.1] - 2025-11-13

### 🐛 CRITICAL FIX: Network Optimization Restore
**This fixes the browser slowness issue after optimization restore!**

#### Fixed
- **Network settings now properly backed up and restored**
  - DNS configurations are now backed up before changes
  - Nagle's Algorithm settings (TcpAckFrequency, TCPNoDelay) now backed up
  - TCP/IP stack settings (netsh commands) now backed up
  - Network throttling index now uses original value, not hardcoded default
  
- **Browser slowness after restore** - Root cause identified and fixed
  - DNS was changed to Cloudflare (1.1.1.1) without backing up original
  - Network interface settings were modified in HKLM without backup
  - Restore used hardcoded "safe defaults" instead of original values
  
#### Added
- `NetworkSettingBackupEntry` - Tracks network interface registry changes
- `DnsBackupEntry` - Backs up DNS server configurations per adapter
- `TcpIpBackupEntry` - Backs up TCP/IP stack settings
- `BackupNetworkInterfaceSettings()` method
- `BackupDnsSettings()` method  
- `BackupTcpIpSettings()` method
- Comprehensive network restore methods
- Documentation: `docs/NETWORK_OPTIMIZATION_FIX.md` - Full explanation and recovery guide

#### Changed
- All network optimizations now call backup before modification
- `ResetToSafeDefaults()` now warns about using backup restore instead
- Backup save log now includes network, DNS, and TCP/IP counts
- Restore completion message now emphasizes restart requirement for network changes

#### Important Notes for Users
- **If experiencing slow browser NOW**: See `docs/NETWORK_OPTIMIZATION_FIX.md`
- Old backups don't contain network settings (system limitation)
- New optimizations will properly backup everything
- Always restart after network restore for changes to take effect

---

## [Unreleased]

### ⚠️ IMPORTANT: Optimization Stability Changes (Latest)

**Most Windows optimizations have been DISABLED by default due to stability concerns.**

#### ✅ What's ENABLED (Safe & Stable):
- ✅ **Power Management** - High Performance power plan
- ✅ **Mouse Settings** - Disable mouse acceleration  
- ✅ **Privacy & Telemetry** (5/7 tweaks) - Disable Windows telemetry, Cortana, advertising ID, location tracking, diagnostics
- ✅ **Gaming Basics** (3/7 tweaks) - Game Mode, disable Game Bar/DVR
- ✅ **UI Tweaks** (2/7 tweaks) - Show file extensions, show hidden files
- ✅ **Cleanup** - Temporary files cleanup

#### ⚠️ What's DISABLED by Default (Unstable/Risky):
- ⚠️ **Visual Effects** - Can affect UI appearance
- ⚠️ **Network Optimizations** - All TCP/IP, DNS, and network tweaks (can break connectivity)
- ⚠️ **CPU/Memory** - Core parking, scheduling, memory management
- ⚠️ **Storage** - SSD optimization, indexing, prefetch
- ⚠️ **Services** - All service modifications (Print Spooler, Windows Search, etc.)
- ⚠️ **Debloat** - App removal, widgets, CoPilot (can remove needed components)
- ⚠️ **Advanced Tweaks** - OneDrive, hibernation, fast startup

#### 📋 Reason for Changes:
Advanced system optimizations can cause:
- System instability and crashes
- Network connectivity issues
- Application compatibility problems
- Difficult-to-diagnose issues requiring troubleshooting

**Focus:** Prioritizing reliability and user experience over aggressive optimization.

💡 **System restore points are ALWAYS created before applying optimizations.**

### ✨ New Features (Prior Updates)
- **50+ Enhanced Windows Optimizations** - Available but disabled by default
  - 🚀 Performance (2 safe / 7 disabled)
  - 🔒 Privacy & Telemetry (5 safe / 2 disabled)
  - ⚙️ Services & Features (0 safe / 5 disabled)
  - 🎮 Gaming Optimizations (3 safe / 4 disabled)
  - 🌐 Network Optimizations (0 safe / 5 disabled - all disabled)
  - 🗑️ Debloat & Cleanup (1 safe / 6 disabled)
  - 💾 Storage & Memory (0 safe / 5 disabled - all disabled)
  - 🖥️ CPU & Memory (0 safe / 5 disabled - all disabled)
  - 🎨 UI Tweaks (2 safe / 5 disabled)
  - ⚡ Advanced & Expert (1 safe / 6 disabled)

- **Optimization UI with Stability Warnings**
  - ⚠️ **Prominent stability notice** at the top
  - Modern expandable sections showing safe/disabled count
  - ✅ Green indicators for safe options
  - ⚠️ Gray/disabled styling for risky options
  - Only "Recommended (Safe)" preset remains (Gaming/Max Performance removed)
  - Enhanced warnings and tooltips explaining risks
  - Select All / Deselect All buttons

- **Improved Windows Activation System**
  - Clear pre-execution warnings about manual interaction
  - 3-second countdown with step-by-step instructions
  - Professional batch wrapper with colored output (Green CMD)
  - Real-time monitoring without blocking UI
  - Automatic temp file cleanup
  - Detailed post-activation next steps
  - Fallback instructions for manual activation if needed

### 🐛 Bug Fixes
- **Removed Duplicate Brave Browser Entry** - Was appearing twice in selection list
- **Fixed Windows Activation Manual Interaction** - Now clearly warns user and provides instructions

### 🔧 Technical Changes
- `WindowsOptimizerService_EnhancedOptimizations.cs`: New partial class with 50+ optimization methods
- `OptimizationWindow.xaml`: Completely redesigned with expandable sections and presets
- `WindowsActivationService.cs`: Rewritten with batch wrapper and background monitoring

### 📚 Documentation
- Updated all GitHub URLs from `grilojr09br` to `theDAVIDL2`
- Fixed repository URLs in `CONTRIBUTING.md`, `BUILDING.md`, `CHANGELOG.md`
- Corrected `YOUR-USERNAME` and `ORIGINAL-OWNER` placeholders

---

## [1.0.1] - 2025-11-13

### 🐛 Bug Fixes
- **Fixed Windows Optimization Error**: Replaced deprecated `wmic` command with modern PowerShell CIM cmdlets
  - Issue: "Error trying to start process 'wmic'" on Windows 11
  - Solution: Using `Get-CimInstance` and `Set-CimInstance` for page file optimization
  - Benefit: Full compatibility with Windows 10 and 11

- **Updated GitHub URLs**: Changed repository owner from `grilojr09br` to `theDAVIDL2`
  - All documentation, installers, and config files updated
  - New repository URL: https://github.com/theDAVIDL2/L2-Setup

### 🔧 Technical Changes
- `WindowsOptimizerService_CustomOptimizations.cs`:
  - Method `OptimizePageFile()` now uses PowerShell CIM API
  - Better error handling with try-catch blocks
  - More descriptive logging messages

---

## [1.0.0] - 2025-11-13

### 🎉 Initial Release - L2 Setup

**Brand:** L2 - All-in-One Windows Post-Format Automation

### 🌟 Core Features
- ✅ **WPF Application** with Material Design UI and tabbed interface
- ✅ **44+ Development Tools** with custom selection
- ✅ **30+ Runtimes Installation** (All-in-One Package)
- ✅ **GPU Auto-Detection** (NVIDIA/AMD/Intel) and driver installation
- ✅ **Brave Browser Backup/Restore** with ZIP compression
- ✅ **Customizable Windows Optimizations** (30+ options)
- ✅ **Windows Activation** (MAS integration)
- ✅ **WinRAR Auto-Activation** with license key
- ✅ **Installer with .NET 8 Auto-Installation**
- ✅ **Error 740 Fix** (Administrator privileges enforced)

### 🛠️ All Runtimes (30+)
- Visual C++ 2005, 2008, 2010, 2012, 2013, 2015-2022 (x86 & x64)
- .NET Framework 3.5, 4.5.2, 4.6.2, 4.7.2, 4.8, 4.8.1
- .NET Core/Modern 5.0, 6.0, 7.0, 8.0
- DirectX End-User Runtime, XNA 4.0, OpenAL
- Java Runtime 8 & 21, Visual Studio Tools for Office
- K-Lite Codec Pack (optional)

### 💻 Development Tools (44+)

**Languages & Runtimes:**
- Git, Python 3.13, Node.js LTS
- Java 21 (Minecraft-compatible), Rust, Go
- .NET 8 SDK

**IDEs & Editors:**
- Visual Studio Code, Cursor IDE
- Visual Studio 2022 Community, Notepad++
- IntelliJ IDEA Community, PyCharm Community

**Browsers:**
- Brave Browser (auto-configured as default)
- Comet (Perplexity AI Browser)

**Essential Applications:**
- Discord, Steam, WinRAR (auto-activated)
- Lightshot, AdsPower, JDownloader 2
- System Informer, IObit Unlocker
- MSI Afterburner, Logitech G Hub

**Development Tools:**
- Postman, DBeaver, FileZilla, PuTTY
- GitHub Desktop, Inno Setup 6
- Yarn, pnpm, Bun, Composer
- Amazon Corretto JDK 8, 17, 21

### ⚡ Windows Optimization Categories
- **Performance**: Power Plan, Mouse Acceleration, Visual Effects, Explorer, Startup, Page File
- **Privacy**: Telemetry, Cortana, Advertising ID, Location, Diagnostics
- **Services**: Print Spooler, Fax, Windows Search, Superfetch, Windows Update
- **Gaming**: Game Mode, Game Bar, Game DVR, Hardware Accelerated GPU
- **Cleanup**: Temp Files, Recycle Bin, Windows.old, Downloads
- **Advanced**: Restore Points, OneDrive, Hibernation

### 🏷️ Branding
- **Product Name**: L2 Setup
- **Executable**: L2Setup.exe
- **Installer**: L2Setup-Installer.exe
- **Repository**: L2-Setup
- **Publisher**: L2 - theDAVIDL2
- **Namespace**: L2.Setup
- **Version**: 1.0.0

### 🚫 Removed from Original Scope
- Docker Desktop (too large, niche use case)
- Obsidian (not essential for most users)
- Cloudflare WARP (replaced with GPU driver system)

### 🔧 Technical Implementation
- ✅ Admin privilege enforcement via app manifest
- ✅ Robust error handling with typed exceptions
- ✅ Asynchronous multi-threaded operations
- ✅ Download & cleanup system for runtimes
- ✅ Configurable release manager for GitHub workflow
- ✅ Self-contained single executable
- ✅ GitHub Actions CI/CD for automated builds

---

## 📊 Comparison with Competitors

| Feature | L2 Setup | ET-Optimizer | windows-11-debloat | RyTuneX |
|---------|----------|--------------|---------------------|---------|
| Optimizations | **50+** | 35 | 20 | 28 |
| Interface | Material Design WPF | C# WinForms | PowerShell CLI | WinForms |
| Customizable | ✅ Full | ✅ Yes | ❌ Limited | ⚠️ Partial |
| Browser Backup | ✅ Yes | ❌ No | ❌ No | ❌ No |
| Multi-tool Install | ✅ 44+ apps | ❌ No | ❌ No | ❌ No |
| Runtimes Install | ✅ 30+ runtimes | ❌ No | ❌ No | ❌ No |
| GPU Drivers | ✅ Auto-detect | ❌ No | ❌ No | ❌ No |
| Windows Activation | ✅ MAS integrated | ❌ No | ❌ No | ❌ No |

---

## 🎯 Inspirations

L2 Setup's optimization system was inspired by the best practices from:
- [ET-Optimizer](https://github.com/semazurek/ET-Optimizer) (507⭐) - CPU & Gaming tweaks
- [windows-11-debloat](https://github.com/teeotsa/windows-11-debloat) (610⭐) - Debloat strategies
- [RyTuneX](https://github.com/rayenghanmi/RyTuneX) - Network optimizations
- [XToolbox](https://github.com/nyxiereal/XToolbox) - UI tweaks
- [vacisdev/windows11](https://github.com/vacisdev/windows11) - Privacy tweaks

We combined their best features and added:
- ✅ Material Design modern UI
- ✅ Complete customization with checkboxes
- ✅ Automatic restore points
- ✅ Detailed logging system
- ✅ One-click presets (Safe, Gaming, Max Performance)

---

## 🔗 Links

- 🐛 [Report a Bug](https://github.com/theDAVIDL2/L2-Setup/issues)
- 💡 [Request a Feature](https://github.com/theDAVIDL2/L2-Setup/issues)
- 📖 [Documentation](https://github.com/theDAVIDL2/L2-Setup/tree/main/docs)
- 🚀 [Latest Release](https://github.com/theDAVIDL2/L2-Setup/releases)

---

**Made with ❤️ by [L2 - theDAVIDL2](https://github.com/theDAVIDL2)**

**Repository:** https://github.com/theDAVIDL2/L2-Setup
