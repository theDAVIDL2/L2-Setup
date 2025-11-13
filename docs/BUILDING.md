# Building Windows Post-Format Setup Tool

This guide explains how to build the project from source code.

## Prerequisites

### Required Software

1. **Operating System**
   - Windows 10 (version 1809 or later) or Windows 11
   - 64-bit system required

2. **Development Tools**
   - [Visual Studio 2022](https://visualstudio.microsoft.com/) (Community, Professional, or Enterprise)
     - Workloads required:
       - .NET desktop development
       - Desktop development with C++
   - OR [Visual Studio Code](https://code.visualstudio.com/) with C# extension
   - [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
   - [Git](https://git-scm.com/downloads)

3. **Optional (for creating installer)**
   - [Inno Setup 6.x](https://jrsoftware.org/isdl.php)

### Recommended Tools

- [GitHub Desktop](https://desktop.github.com/) - For easier Git management
- [Windows Terminal](https://aka.ms/terminal) - Better command line experience

## Getting the Source Code

### Clone the Repository

```bash
# Using HTTPS
git clone https://github.com/yourusername/windows-post-format-setup.git

# Using SSH
git clone git@github.com:yourusername/windows-post-format-setup.git

# Navigate to project directory
cd windows-post-format-setup
```

### Fork the Repository (for contributions)

1. Go to https://github.com/yourusername/windows-post-format-setup
2. Click "Fork" button
3. Clone your fork:
```bash
git clone https://github.com/YOUR-USERNAME/windows-post-format-setup.git
cd windows-post-format-setup
git remote add upstream https://github.com/ORIGINAL-OWNER/windows-post-format-setup.git
```

## Building the Project

### Option 1: Using Visual Studio

1. Open `windows-post-format-setup.sln` in Visual Studio 2022
2. Select "Release" or "Debug" configuration
3. Press `Ctrl + Shift + B` to build
4. Press `F5` to run with debugging or `Ctrl + F5` without debugging

### Option 2: Using .NET CLI

#### Debug Build

```bash
# Navigate to project directory
cd src/WindowsSetup.App

# Restore NuGet packages
dotnet restore

# Build
dotnet build

# Run
dotnet run
```

#### Release Build

```bash
# Build in Release mode
dotnet build --configuration Release

# Run release build
dotnet run --configuration Release
```

#### Publish (Self-Contained)

```bash
# Publish for Windows x64
dotnet publish -c Release -r win-x64 --self-contained false -o ../../publish/

# The executable will be in: publish/WindowsSetup.exe
```

### Option 3: Using Visual Studio Code

1. Open project folder in VS Code
2. Install recommended extensions (C# extension)
3. Press `Ctrl + Shift + P` and select "Tasks: Run Build Task"
4. Or use integrated terminal and run .NET CLI commands

## Building the Installer

### Prerequisites

- Inno Setup 6.x installed
- Project built in Release mode

### Steps

1. Build the project in Release mode:
```bash
dotnet publish src/WindowsSetup.App/WindowsSetup.App.csproj -c Release -r win-x64 --self-contained false
```

2. Compile the Inno Setup script:
```bash
# Using command line (if Inno Setup is in PATH)
iscc setup.iss

# Or open setup.iss in Inno Setup and click "Compile"
```

3. The installer will be created in `output/WindowsPostFormatSetup_v1.0.0.exe`

## Project Structure

```
windows-post-format-setup/
├── src/
│   └── WindowsSetup.App/
│       ├── WindowsSetup.App.csproj    # Project file
│       ├── App.xaml                    # Application definition
│       ├── MainWindow.xaml             # Main window UI
│       ├── Services/                   # Business logic
│       ├── Models/                     # Data models
│       └── Utils/                      # Utilities
├── assets/                             # Icons and images
├── docs/                               # Documentation
├── setup.iss                           # Inno Setup script
└── .github/workflows/                  # CI/CD configuration
```

## Build Configurations

### Debug Configuration

- Optimizations: Disabled
- Debugging symbols: Included
- Purpose: Development and testing

```bash
dotnet build --configuration Debug
```

### Release Configuration

- Optimizations: Enabled
- Debugging symbols: Stripped (unless specified)
- Purpose: Production deployment

```bash
dotnet build --configuration Release
```

## Common Build Issues

### Issue: NuGet Package Restore Failed

**Solution:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages again
dotnet restore
```

### Issue: .NET SDK Not Found

**Solution:**
1. Verify .NET 8 SDK is installed:
```bash
dotnet --list-sdks
```

2. If not listed, download from: https://dotnet.microsoft.com/download/dotnet/8.0

### Issue: MaterialDesignThemes Not Found

**Solution:**
```bash
# Restore NuGet packages
dotnet restore src/WindowsSetup.App/WindowsSetup.App.csproj
```

### Issue: Administrator Privileges Required

**Solution:**
- Run Visual Studio or terminal as Administrator
- Or adjust UAC settings temporarily

### Issue: SharpCompress Error

**Solution:**
```bash
# Install specific version
dotnet add src/WindowsSetup.App package SharpCompress --version 0.36.0
```

## Testing Builds

### Manual Testing

1. **Test on Clean VM:**
   - Create Windows 10/11 VM
   - Install the built application
   - Test all features
   - Check for errors

2. **Test Without Admin:**
   - Run without administrator privileges
   - Verify admin prompt appears

3. **Test Network Scenarios:**
   - Test with slow internet
   - Test offline (already cached installers)
   - Test download failures

### Automated Testing (Future)

```bash
# Run unit tests (when implemented)
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## Debugging

### Visual Studio Debugging

1. Set breakpoints by clicking left margin
2. Press F5 to start debugging
3. Use Debug menu for step-by-step execution

### VS Code Debugging

1. Install C# extension
2. Add launch.json configuration:
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (console)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/src/WindowsSetup.App/bin/Debug/net8.0-windows/WindowsSetup.exe",
      "args": [],
      "cwd": "${workspaceFolder}/src/WindowsSetup.App",
      "stopAtEntry": false,
      "console": "internalConsole"
    }
  ]
}
```
3. Press F5 to debug

### Logging

All operations are logged to:
- UI Log panel (real-time)
- Export log feature (save to file)

## Performance Profiling

### Using Visual Studio Profiler

1. Debug → Performance Profiler
2. Select "CPU Usage" or "Memory Usage"
3. Start profiling
4. Run the operations
5. Stop and analyze results

## Clean Build

To ensure a completely clean build:

```bash
# Remove all build artifacts
dotnet clean

# Remove bin and obj folders
Remove-Item -Recurse -Force src/WindowsSetup.App/bin,src/WindowsSetup.App/obj

# Restore and rebuild
dotnet restore
dotnet build --configuration Release
```

## Build Scripts

### Build All (PowerShell)

Create `build.ps1`:
```powershell
# Clean
dotnet clean

# Restore
dotnet restore src/WindowsSetup.App/WindowsSetup.App.csproj

# Build
dotnet build src/WindowsSetup.App/WindowsSetup.App.csproj --configuration Release

# Publish
dotnet publish src/WindowsSetup.App/WindowsSetup.App.csproj -c Release -r win-x64 --self-contained false -o publish/

# Create installer
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" setup.iss

Write-Host "Build complete! Installer is in output/ folder"
```

Run with:
```powershell
.\build.ps1
```

## CI/CD

The project uses GitHub Actions for automated builds.

See `.github/workflows/build.yml` for configuration.

### Local CI Testing

Install [act](https://github.com/nektos/act):
```bash
# Test workflow locally
act -j build
```

## Version Management

Update version in:
1. `src/WindowsSetup.App/WindowsSetup.App.csproj` - `<Version>` tag
2. `setup.iss` - `AppVersion` directive
3. `docs/CHANGELOG.md` - Add new version entry

## Troubleshooting

### Can't Build: MSBuild Errors

Check:
- Visual Studio installation is complete
- .NET 8 SDK is installed
- Project file is not corrupted

### Can't Run: Missing DLLs

Solution:
```bash
# Publish with all dependencies
dotnet publish -c Release -r win-x64 --self-contained true
```

### Installer Creation Fails

Check:
- Inno Setup is installed
- Path to ISCC.exe is correct
- All files exist in publish directory

## Getting Help

- Check [ARCHITECTURE.md](ARCHITECTURE.md) for code structure
- See [CONTRIBUTING.md](CONTRIBUTING.md) for development guidelines
- Open an issue for build problems
- Join discussions for questions

---

**Happy Building! 🚀**

