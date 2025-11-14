<div align="center">

# 🚀 L2 Setup
### All-in-One Windows Post-Format Automation Tool

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/platform-Windows-blue.svg)](https://www.microsoft.com/windows)
[![GitHub release](https://img.shields.io/github/v/release/theDAVIDL2/L2-Setup)](https://github.com/theDAVIDL2/L2-Setup/releases)
[![Stars](https://img.shields.io/github/stars/theDAVIDL2/L2-Setup?style=social)](https://github.com/theDAVIDL2/L2-Setup/stargazers)

**Save hours of post-format setup with automated tool installation, browser profile restoration, and 50+ Windows optimizations!**

[📥 Download Latest Release](https://github.com/theDAVIDL2/L2-Setup/releases) • [📖 Documentation](docs/) • [🐛 Report Bug](https://github.com/theDAVIDL2/L2-Setup/issues) • [💡 Request Feature](https://github.com/theDAVIDL2/L2-Setup/issues)

KNOWN ISSUES: 

• Installer isn't being capable of downloading NET 8 runtime, so the production exe is being attached to the releases

• Process Hacker 2 installation process isn't working

• Only Brave is suported for the browsers tab for now

</div>

---

## 📋 Table of Contents

- [About](#-about)
- [Features](#-features)
- [Screenshots](#-screenshots)
- [Installation](#-installation)
- [Usage](#-usage)
- [What Gets Installed](#-what-gets-installed)
- [Optimizations](#-optimizations)
- [Building from Source](#-building-from-source)
- [Contributing](#-contributing)
- [License](#-license)
- [Support](#-support)

---

## 🎯 About

**L2 Setup** is a professional Windows post-format automation tool designed to **save hours** of manual configuration. Whether you're a developer, gamer, or power user, this tool handles everything from installing 44+ essential applications to applying 50+ system optimizations.

**⚠️ USE BEFORE & AFTER FORMATTING:**
- **BEFORE**: Backup your browser profiles (Brave, Chrome, Edge, Firefox)
- **AFTER**: Restore profiles and auto-install all your tools in one click!

### 🏆 Why L2 Setup?

| Feature | L2 Setup | Manual Setup | Other Tools |
|---------|----------|--------------|-------------|
| **Time Required** | 10-15 minutes | 3-5 hours | 1-2 hours |
| **Tools Installed** | 44+ apps | One by one | Limited selection |
| **Runtimes Included** | 30+ runtimes | Manual downloads | Not included |
| **Browser Backup** | ✅ Full profile | ❌ Manual | ❌ Not supported |
| **Windows Optimization** | 50+ tweaks | Manual registry | 20-35 tweaks |
| **GPU Drivers** | Auto-detect & install | Manual search | ❌ Not supported |
| **Windows Activation** | Integrated (MAS) | Manual scripting | ❌ Not included |
| **Interface** | Modern WPF (Material Design) | N/A | Basic forms |

---

## ⚠️ Optimizations Status

**IMPORTANT UPDATE:** Most Windows optimizations are currently **DISABLED by default** due to stability concerns.

### ✅ What's ENABLED (Safe Options Only):
- ✅ **Power Management** - High Performance power plan
- ✅ **Mouse Settings** - Disable mouse acceleration
- ✅ **Privacy & Telemetry** - Disable Windows telemetry, Cortana, advertising ID, location tracking
- ✅ **Gaming Basics** - Game Mode, disable Game Bar/DVR
- ✅ **UI Tweaks** - Show file extensions, show hidden files
- ✅ **Cleanup** - Temporary files cleanup

### ⚠️ What's DISABLED (Potentially Unstable):
- ⚠️ Visual effects, transparency, animations
- ⚠️ All network optimizations (TCP/IP, DNS, etc.)
- ⚠️ CPU/Memory optimizations (core parking, scheduling)
- ⚠️ Storage optimizations (SSD, indexing, prefetch)
- ⚠️ Service modifications (Windows Search, Superfetch, etc.)
- ⚠️ Debloat features (app removal, widgets, etc.)

**Why?** Advanced optimizations can cause system instability, break functionality, or require troubleshooting. We're focusing on reliability.

💡 **A system restore point is ALWAYS created before applying any optimizations.**

---

## ✨ Features

### 🌐 Browser Management (PRE & POST FORMAT)

<details>
<summary><b>Click to expand Browser Features</b></summary>

#### Before Formatting:
- ✅ **Full Profile Backup** (Brave, Chrome, Edge, Firefox)
  - Extensions, bookmarks, passwords, cookies, history
  - Automatic ZIP compression for space efficiency
  - Save to USB, external drive, or cloud storage
  - Backup validation and integrity checks

#### After Formatting:
- ✅ **One-Click Profile Restore**
  - Auto-detection of backup files on USB drives
  - Complete profile restoration with all settings
  - Automatic browser installation if not present
  - Set Brave as default browser automatically

</details>

### 🛠️ Application Installation

<details>
<summary><b>Click to expand 44+ Tools</b></summary>

#### Development Tools (18 apps):
- **Version Control**: Git, GitHub Desktop
- **Languages**: Python 3.13, Node.js LTS, Rust, Go
- **Java**: Amazon Corretto 8, 17, 21 LTS
- **IDEs**: VS Code, Cursor, Visual Studio 2022 Community
- **Editors**: Notepad++, IntelliJ IDEA, PyCharm Community
- **Package Managers**: Yarn, pnpm, Bun, Composer
- **Tools**: .NET 8 SDK, Inno Setup 6

#### Networking & API (4 apps):
- Postman, DBeaver, FileZilla, PuTTY

#### Browsers (2 apps):
- Brave Browser (auto-configured as default)
- Comet (Perplexity AI Browser)

#### Gaming & Communication (2 apps):
- Discord, Steam

#### Utilities (11 apps):
- WinRAR (auto-activated with license key)
- Lightshot (screenshot tool)
- JDownloader 2 (download manager)
- System Informer (task manager alternative)
- IObit Unlocker (file unlocker)
- MSI Afterburner (GPU monitoring)
- Logitech G Hub (peripheral software)
- AdsPower (multi-account browser)
- 7-Zip, Everything (search)
- ShareX (advanced screenshots)

#### Java & Minecraft:
- Java 21 (Minecraft-compatible runtime)
- OpenJDK 21 with JavaFX

</details>

### ⚡ Windows Optimizations (50+ Tweaks)

<details>
<summary><b>Click to expand Optimization Categories</b></summary>

#### 🚀 Performance (9 tweaks):
- High Performance Power Plan
- Disable Mouse Acceleration
- Optimize Visual Effects
- Optimize File Explorer
- Disable Background Apps ⭐ NEW
- Disable Transparency Effects ⭐ NEW
- Disable Windows Animations ⭐ NEW
- Optimize Page File/Virtual Memory
- Disable Unnecessary Startup Programs

#### 🔒 Privacy & Telemetry (7 tweaks):
- Disable Windows Telemetry
- Disable Cortana
- Disable Advertising ID
- Disable Location Tracking
- Disable Diagnostic Data Collection
- Disable Activity History ⭐ NEW
- Disable Web Search in Start Menu ⭐ NEW

#### 🎮 Gaming Optimizations (7 tweaks):
- Enable Game Mode
- Disable Game Bar
- Disable Game DVR
- Enable Hardware-accelerated GPU Scheduling
- Disable Fullscreen Optimizations ⭐ NEW
- Optimize CPU Scheduling for Games ⭐ NEW
- Disable Nagle Algorithm (Lower Latency) ⭐ NEW

#### 🌐 Network Optimizations (5 tweaks): ⭐ ALL NEW
- Optimize TCP/IP Stack
- Set Cloudflare DNS (1.1.1.1)
- Disable Network Throttling
- Optimize Network Adapter Settings
- Disable Large Send Offload

#### 🗑️ Debloat & Cleanup (7 tweaks):
- Clean Temporary Files
- Remove Bloatware Apps (27 apps: Xbox, Teams, OneDrive, etc.) ⭐ NEW
- Disable Windows 11 Widgets ⭐ NEW
- Remove Windows CoPilot ⭐ NEW
- Disable Chat Icon (Teams) ⭐ NEW
- Empty Recycle Bin
- Remove Windows.old Folder

#### 💾 Storage & Memory (5 tweaks): ⭐ ALL NEW
- Disable Search Indexing
- Optimize SSD (Enable TRIM)
- Disable System Restore (Frees Space)
- Compact OS (Compress Windows)
- Disable Prefetch

#### 🖥️ CPU & Memory (5 tweaks): ⭐ ALL NEW
- Disable CPU Core Parking
- Optimize Processor Scheduling
- Optimize Memory Management
- Disable Write Cache Buffer Flushing
- ⚠️ Disable Spectre/Meltdown (EXPERT - Security Risk!)

#### 🎨 UI & Visual Tweaks (7 tweaks):
- Show File Extensions
- Show Hidden Files
- Disable Lock Screen ⭐ NEW
- Disable Action Center
- Classic Context Menu (Windows 11) ⭐ NEW
- Move Taskbar to Left (Windows 11)
- Disable Snap Assist

#### ⚡ Advanced & Expert (6 tweaks):
- ✅ Create System Restore Point (RECOMMENDED)
- Disable OneDrive
- Disable Hibernation (Frees Space)
- Disable Fast Startup ⭐ NEW
- Disable Remote Assistance ⭐ NEW
- Disable Error Reporting ⭐ NEW

**Total: 50+ Optimizations!** (⭐ NEW = 22 new tweaks in latest version)

</details>

### 🎯 All-in-One Runtimes (30+ Packages)

<details>
<summary><b>Click to expand Runtimes List</b></summary>

#### Visual C++ Redistributables:
- 2005, 2008, 2010, 2012, 2013, 2015-2022 (x86 & x64)

#### .NET Framework:
- 3.5, 4.5.2, 4.6.2, 4.7.2, 4.8, 4.8.1

#### .NET Modern Runtimes:
- .NET 5.0, 6.0, 7.0, 8.0 (Desktop & ASP.NET Core)

#### Graphics & Gaming:
- DirectX End-User Runtime (June 2010)
- XNA Framework 4.0 Redistributable
- OpenAL (audio library)

#### Java Runtimes:
- Java 8 LTS (legacy support)
- Java 21 LTS (latest, Minecraft-compatible)

#### Development:
- Visual Studio Tools for Office (VSTO)

#### Optional:
- K-Lite Codec Pack (media codecs)

**Automatic Installation & Cleanup:**
- Silent installations (no user interaction)
- Multi-threaded downloads for speed
- Automatic cleanup of installer files
- Progress tracking and error handling

</details>

### 🎮 GPU Driver Detection

- **Auto-detects GPU vendor**: NVIDIA, AMD, Intel
- **Opens driver download page** automatically
- **Provides direct links** to latest drivers
- **Manual installation guidance** included

### 🔑 Windows Activation

- **Microsoft Activation Scripts (MAS)** integration
- **HWID Activation** (permanent, digital license)
- **Clear instructions** for manual interaction if needed
- **Status verification** after activation

### ⚙️ Additional Features

- ✅ **WinRAR Auto-Activation**: Automatically creates license key file
- ✅ **Material Design UI**: Modern, intuitive interface
- ✅ **Tabbed Interface**: Quick Setup, Custom Selection, Browser, Optimization
- ✅ **Progress Tracking**: Real-time logs and status updates
- ✅ **Error Handling**: Robust error recovery and detailed logging
- ✅ **Admin Privileges**: Automatic elevation via RunAsAdmin.bat
- ✅ **Self-Contained**: Single executable, no dependencies

---

## 📸 Screenshots

<div align="center">

### Main Interface
![Main Window](docs/images/main-window.png)

### Optimization Settings
![Optimization Window](docs/images/optimization-window.png)

### Browser Backup
![Browser Backup](docs/images/browser-backup.png)

</div>

---

## 📥 Installation

### Option 1: Installer (Recommended)

1. Download **[L2Setup-Installer.exe](https://github.com/theDAVIDL2/L2-Setup/releases/latest)** (~49 MB)
2. Run the installer (requires Administrator privileges)
3. **Auto-installs .NET 8 Runtime** if not present (no user interaction required)
4. Launch from Start Menu or Desktop shortcut

### Option 2: Standalone Executable

1. Download **[L2Setup.exe](https://github.com/theDAVIDL2/L2-Setup/releases/latest)** (~166 MB)
2. Requires **.NET 8 Runtime** (auto-detected, download link provided if missing)
3. Run directly (no installation needed)

### System Requirements

- **OS**: Windows 10 (1809+) or Windows 11
- **RAM**: 4 GB minimum (8 GB recommended)
- **Storage**: 10 GB free space for tools and runtimes
- **Internet**: Required for downloads
- **.NET**: 8.0 Runtime (auto-installed by installer)

---

## 🚀 Usage

### Quick Start Guide

#### Before Formatting:

```
1. Launch L2 Setup
2. Go to "Browser Management" tab
3. Click "Backup Brave Profile" (or other browsers)
4. Save to USB/External Drive/Cloud
5. Format your PC confidently!
```

#### After Formatting:

```
1. Install L2 Setup
2. Go to "Quick Setup" tab
3. Click "Install All" (or select custom tools)
4. Go to "Browser Management" tab
5. Click "Restore Brave Profile"
6. Go to "Windows Optimization" tab
7. Select optimizations (or use preset)
8. Click "Apply Optimizations"
9. Optional: Activate Windows
10. Reboot and enjoy!
```

### Optimization Presets

- **Recommended (Safe)**: Balanced performance and stability
- **Gaming Optimized**: Maximum gaming performance
- **Max Performance**: All performance tweaks (expert users)

---

## 📦 What Gets Installed

### Essential Apps (Auto-Install)
- Git (version control)
- Python 3.13 (programming language)
- Node.js LTS (JavaScript runtime)
- Visual Studio Code (code editor)

### Optional Apps (Custom Selection)
- **44+ tools** across categories (see [Features](#-features))
- **30+ runtimes** for compatibility
- **GPU drivers** based on your hardware

---

## ⚙️ Optimizations

### How Optimizations Work

1. **System Restore Point** created automatically (if enabled)
2. **Registry tweaks** applied with error handling
3. **Services disabled/modified** as per selection
4. **Cleanup tasks** executed (temp files, cache)
5. **Network settings** optimized
6. **Detailed logs** for troubleshooting

### Safety Features

- ✅ **Restore Point** created before changes
- ✅ **Selective application** (choose what you want)
- ✅ **Undo instructions** provided
- ✅ **No dangerous tweaks** enabled by default
- ⚠️ **Expert options** clearly marked with warnings

---

## 🔨 Building from Source

### Prerequisites

- **Windows 10/11** (build environment)
- **.NET 8 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Visual Studio 2022** or **VS Code** (optional, but recommended)
- **Inno Setup 6** (for installer creation)
- **Git** (version control)

### Clone & Build

```bash
# Clone the repository
git clone https://github.com/theDAVIDL2/L2-Setup.git
cd L2-Setup

# Restore dependencies
dotnet restore src/WindowsSetup.App/WindowsSetup.App.csproj

# Build (Debug)
dotnet build src/WindowsSetup.App/WindowsSetup.App.csproj --configuration Debug

# Build (Release)
dotnet build src/WindowsSetup.App/WindowsSetup.App.csproj --configuration Release

# Run
cd src/WindowsSetup.App/bin/Release/net8.0-windows/win-x64
./L2Setup.exe
```

### Create Installer

```bash
# Compile with Inno Setup
iscc setup.iss

# Output: Release/L2Setup-Installer.exe
```

For detailed build instructions, see [BUILDING.md](docs/BUILDING.md).

---

## 🤝 Contributing

We welcome contributions! Whether it's bug reports, feature requests, or code contributions, we appreciate your help.

### How to Contribute

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/AmazingFeature`)
3. **Commit** your changes (`git commit -m 'Add some AmazingFeature'`)
4. **Push** to the branch (`git push origin feature/AmazingFeature`)
5. **Open** a Pull Request

For detailed guidelines, see [CONTRIBUTING.md](docs/CONTRIBUTING.md).

### Code of Conduct

This project adheres to a [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

---

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

### Third-Party Licenses

- **Material Design In XAML Toolkit**: MIT License
- **SharpCompress**: MIT License
- **Newtonsoft.Json**: MIT License
- **Microsoft Activation Scripts (MAS)**: GPL-3.0 (external, not bundled)

---

## 💬 Support

### Need Help?

- 🐛 **Bug Reports**: [GitHub Issues](https://github.com/theDAVIDL2/L2-Setup/issues)
- 💡 **Feature Requests**: [GitHub Issues](https://github.com/theDAVIDL2/L2-Setup/issues)
- 📖 **Documentation**: [docs/](docs/)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/theDAVIDL2/L2-Setup/discussions)

### FAQ

<details>
<summary><b>Q: Do I need to install .NET 8 separately?</b></summary>

A: No! The installer (`L2Setup-Installer.exe`) automatically detects and installs .NET 8 Runtime if not present. If using the standalone executable, a download link will be provided if needed.
</details>

<details>
<summary><b>Q: Are the optimizations safe?</b></summary>

A: Yes! All optimizations are tested and safe. Expert/dangerous options are clearly marked with warnings. A system restore point is created before applying changes.
</details>

<details>
<summary><b>Q: Can I undo optimizations?</b></summary>

A: Yes! You can restore from the system restore point created before optimizations. Some tweaks can also be manually reverted (documented in logs).
</details>

<details>
<summary><b>Q: Why does it need Administrator privileges?</b></summary>

A: Admin rights are required for:
- Installing applications
- Modifying system settings
- Applying registry tweaks
- Changing services
</details>

<details>
<summary><b>Q: How long does the full setup take?</b></summary>

A: Typically 10-15 minutes for:
- Installing 44+ apps: 5-8 minutes
- Installing 30+ runtimes: 3-5 minutes
- Applying optimizations: 1-2 minutes
</details>

---

## 🌟 Acknowledgments

This project was inspired by and combines best practices from:

- [ET-Optimizer](https://github.com/semazurek/ET-Optimizer) - CPU & Gaming tweaks
- [windows-11-debloat](https://github.com/teeotsa/windows-11-debloat) - Debloat strategies
- [RyTuneX](https://github.com/rayenghanmi/RyTuneX) - Network optimizations
- [XToolbox](https://github.com/nyxiereal/XToolbox) - UI tweaks
- [vacisdev/windows11](https://github.com/vacisdev/windows11) - Privacy tweaks
- [Microsoft Activation Scripts](https://massgrave.dev/) - Windows activation

Special thanks to all contributors and the open-source community!

---

## 📊 Project Stats

![GitHub repo size](https://img.shields.io/github/repo-size/theDAVIDL2/L2-Setup)
![GitHub code size](https://img.shields.io/github/languages/code-size/theDAVIDL2/L2-Setup)
![GitHub language count](https://img.shields.io/github/languages/count/theDAVIDL2/L2-Setup)
![GitHub top language](https://img.shields.io/github/languages/top/theDAVIDL2/L2-Setup)
![GitHub last commit](https://img.shields.io/github/last-commit/theDAVIDL2/L2-Setup)

---

<div align="center">

## 🎉 Made with ❤️ by [L2 - theDAVIDL2](https://github.com/theDAVIDL2)

**⭐ If you find this project useful, consider giving it a star!**

[🏠 Homepage](https://github.com/theDAVIDL2/L2-Setup) • [📥 Download](https://github.com/theDAVIDL2/L2-Setup/releases) • [📖 Documentation](docs/) • [🐛 Issues](https://github.com/theDAVIDL2/L2-Setup/issues)

</div>
