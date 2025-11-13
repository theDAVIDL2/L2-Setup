# 📋 Changelog

All notable changes to **L2 Setup** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/), and this project adheres to [Semantic Versioning](https://semver.org/).

---

## [Unreleased]

### ✨ New Features
- **50+ Enhanced Windows Optimizations** - Inspired by the best GitHub repos
  - 🚀 Performance (9 tweaks): Background Apps, Transparency, Animations
  - 🔒 Privacy & Telemetry (7 tweaks): Activity History, Web Search
  - ⚙️ Services & Features (5 tweaks): Superfetch, Windows Search control
  - 🎮 Gaming Optimizations (7 tweaks): Fullscreen Opt, CPU Scheduling, Nagle Algorithm
  - 🌐 Network Optimizations (5 tweaks): TCP/IP Stack, Cloudflare DNS, Throttling
  - 🗑️ Debloat & Cleanup (7 tweaks): Remove 27 Bloatware Apps, Widgets, CoPilot
  - 💾 Storage & Memory (5 tweaks): SSD TRIM, Search Indexing, Compact OS
  - 🖥️ CPU & Memory (5 tweaks): Core Parking, Spectre/Meltdown (EXPERT)
  - 🎨 UI Tweaks (7 tweaks): Classic Context Menu, Lock Screen, Taskbar
  - ⚡ Advanced & Expert (6 tweaks): Fast Startup, Remote Assistance, Error Reporting

- **Completely Redesigned Optimization UI**
  - Modern expandable sections for each optimization category
  - Quick presets: Recommended (Safe), Gaming Optimized, Max Performance
  - Real-time optimization counter
  - Color-coded options (Green = NEW, Red = DANGEROUS)
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
