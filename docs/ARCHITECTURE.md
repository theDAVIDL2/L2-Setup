# Architecture Documentation

## Overview

Windows Post-Format Setup Tool is built using C# .NET 8 with WPF (Windows Presentation Foundation) following the MVVM (Model-View-ViewModel) pattern.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────┐
│                     MainWindow (View)                    │
│  ┌────────────┐ ┌────────────┐ ┌─────────────────────┐ │
│  │  Browser   │ │   Tools    │ │  Optimization &     │ │
│  │  Manager   │ │  Installer │ │  Activation Cards   │ │
│  └────────────┘ └────────────┘ └─────────────────────┘ │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────┴──────────────────────────────────┐
│                    Services Layer                        │
│  ┌───────────────┐  ┌──────────────┐  ┌──────────────┐ │
│  │  Browser      │  │   Tool       │  │   Windows    │ │
│  │  Backup       │  │   Installer  │  │   Optimizer  │ │
│  │  Service      │  │   Service    │  │   Service    │ │
│  └───────────────┘  └──────────────┘  └──────────────┘ │
│  ┌───────────────┐  ┌──────────────┐  ┌──────────────┐ │
│  │  Windows      │  │   Download   │  │   Command    │ │
│  │  Activation   │  │   Manager    │  │   Runner     │ │
│  │  Service      │  │              │  │              │ │
│  └───────────────┘  └──────────────┘  └──────────────┘ │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────┴──────────────────────────────────┐
│                    Utilities Layer                       │
│  ┌───────────┐  ┌────────────┐  ┌──────────────────┐   │
│  │  Logger   │  │   Admin    │  │  USB Drive       │   │
│  │           │  │   Helper   │  │  Detector        │   │
│  └───────────┘  └────────────┘  └──────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

## Core Components

### 1. Views (Presentation Layer)

#### MainWindow.xaml
- Main application window
- Contains all UI cards and controls
- Real-time activity log
- Progress indicators

**Responsibilities:**
- Display UI components
- Capture user interactions
- Show progress and status
- Display logs in real-time

### 2. Services (Business Logic Layer)

#### BrowserBackupService
```csharp
public class BrowserBackupService
{
    // Backup Brave profile to compressed archive
    Task<BackupResult> BackupBraveProfile(string destinationPath)
    
    // Restore profile from backup
    Task<RestoreResult> RestoreBraveProfile(string backupPath)
    
    // Set Brave as default browser
    Task SetBraveAsDefaultBrowser()
    
    // Check if Brave is running
    bool IsBraveRunning()
}
```

**Key Features:**
- Compression using SharpCompress
- Handles running browser detection
- Backs up existing profiles before restore
- Registry manipulation for default browser

#### ToolInstallerService
```csharp
public class ToolInstallerService
{
    // Install essential tools only
    Task InstallEssentialTools()
    
    // Install all tools
    Task InstallAllTools()
    
    // Install runtimes
    Task InstallRuntimes()
}
```

**Key Features:**
- Dual installation method (winget + direct download)
- Priority-based installation order
- Skip already installed tools
- Parallel downloads with DownloadManager
- Silent installation parameters

#### WindowsOptimizerService
```csharp
public class WindowsOptimizerService
{
    // Apply all optimizations
    Task ApplyAllOptimizations()
    
    // Run Chris Titus Tech script
    Task RunChrisTitusScript()
}
```

**Optimizations Applied:**
- Power plan (High Performance)
- Mouse acceleration disable
- Visual effects optimization
- Service disabling
- Telemetry disable
- Explorer configuration
- Temp files cleanup

#### WindowsActivationService
```csharp
public class WindowsActivationService
{
    // Activate Windows automatically
    Task ActivateWindowsAutomatic()
    
    // Check activation status
    Task<ActivationStatus> CheckWindowsActivationStatus()
}
```

**Key Features:**
- Uses Microsoft Activation Scripts
- Automatic menu navigation
- HWID activation method
- Status verification

#### DownloadManager
```csharp
public class DownloadManager
{
    // Download multiple files in parallel
    Task DownloadMultipleAsync(List<DownloadItem> items)
    
    // Download single file with progress
    Task DownloadWithProgressAsync(DownloadItem item, IProgress<DownloadProgress> progress)
}
```

**Key Features:**
- Multi-threaded downloads (configurable parallelism)
- Progress tracking
- Retry mechanism (exponential backoff)
- SHA256 hash verification
- Resume capability

#### CommandRunner
```csharp
public class CommandRunner
{
    // Run command with output capture
    Task<ProcessResult> RunCommandAsync(string command, string args, bool requireAdmin)
    
    // Run PowerShell script
    Task<ProcessResult> RunPowerShellAsync(string script, bool requireAdmin)
}
```

### 3. Models (Data Layer)

#### ToolDefinition
```csharp
public class ToolDefinition
{
    string Name
    string Method          // "winget" or "direct"
    string WingetId
    string DirectUrl
    string SilentArgs
    int Priority
    bool Essential
    string PostInstallCommand
}
```

#### BackupResult / RestoreResult
```csharp
public class BackupResult
{
    bool Success
    string BackupPath
    double SizeInMB
    int ExtensionsCount
    string ErrorMessage
}
```

#### DownloadProgress
```csharp
public class DownloadProgress
{
    string FileName
    long BytesDownloaded
    long TotalBytes
    int Percentage
    double SpeedMBps
}
```

### 4. Utilities

#### AdminHelper
- Check if running as administrator
- Restart with admin privileges

#### Logger
- Centralized logging
- Log levels (Info, Success, Warning, Error)
- Real-time UI updates

#### USBDriveDetector
- Detect USB drives
- Search for backups on USB

## Data Flow

### Backup Flow
```
User Click → MainWindow → BrowserBackupService
                              ↓
                    Check if Brave running
                              ↓
                    Compress profile folder
                              ↓
                    Save to selected location
                              ↓
                    Return BackupResult
                              ↓
                    Update UI with result
```

### Installation Flow
```
User Click → MainWindow → ToolInstallerService
                              ↓
                    Get tool definitions
                              ↓
            ┌─────────────────┴────────────────┐
            │                                   │
     Check winget              Download via      
            │                  DownloadManager
            │                         │
            ├─────────────────────────┘
            │
     Install via winget or direct
            │
     Run post-install commands
            │
     Update progress UI
```

### Optimization Flow
```
User Click → MainWindow → WindowsOptimizerService
                              ↓
                    Create restore point
                              ↓
            ┌───────────────────────────────┐
            │ Parallel Optimization Tasks   │
            │ - Power                       │
            │ - Mouse                       │
            │ - Visual Effects              │
            │ - Services                    │
            │ - Telemetry                   │
            │ - Explorer                    │
            │ - Cleanup                     │
            └───────────────────────────────┘
                              ↓
                    Log results
                              ↓
                    Update UI
```

## Configuration

### AppSettings.json
```json
{
  "DownloadSettings": {
    "MaxParallelDownloads": 4,
    "RetryAttempts": 3,
    "DownloadTimeout": 300
  },
  "BrowserSettings": {
    "DefaultBackupPath": "...",
    "AutoDetectUSB": true,
    "CompressBackups": true
  }
}
```

## Threading Model

- **UI Thread:** WPF MainWindow, user interactions
- **Background Threads:** 
  - Download tasks (SemaphoreSlim for parallelism)
  - Installation processes
  - File compression/decompression
- **Async/Await:** All I/O operations

## Security Considerations

1. **Admin Privileges:** Required for system modifications
2. **Registry Access:** Careful handling with error management
3. **Process Execution:** Validation of commands before execution
4. **File Operations:** Exception handling for permission issues
5. **Download Verification:** SHA256 hash checking

## Error Handling

- Try-catch blocks at service level
- Detailed error logging
- User-friendly error messages
- Graceful degradation (non-essential features fail safely)

## Performance Optimizations

1. **Parallel Downloads:** Up to 4 simultaneous downloads
2. **Caching:** Downloaded installers cached locally
3. **Skip Installed:** Check before reinstalling tools
4. **Async Operations:** Non-blocking UI
5. **Compression:** Fast ZIP for backups

## Extensibility

### Adding New Tools
1. Add `ToolDefinition` to `GetAllToolDefinitions()`
2. Set appropriate priority and installation method
3. Define silent installation arguments

### Adding New Services
1. Create service class in `Services/`
2. Inject logger in constructor
3. Implement async methods
4. Wire up to UI in MainWindow

### Adding New Optimizations
1. Add method to `WindowsOptimizerService`
2. Call from `ApplyAllOptimizations()`
3. Include registry paths or commands
4. Add logging

## Testing Strategy

- Manual testing on clean Windows installs
- VM testing for system modifications
- Backup/restore testing with actual profiles
- Installation testing with various internet speeds
- Error scenario testing (no admin, no internet, etc.)

## Dependencies

- **.NET 8:** Core framework
- **MaterialDesignThemes:** UI components
- **SharpCompress:** Archive operations
- **Newtonsoft.Json:** Configuration parsing

## Build and Deployment

1. **Development Build:** `dotnet build`
2. **Release Build:** `dotnet publish -c Release`
3. **Installer Creation:** Inno Setup script
4. **CI/CD:** GitHub Actions for automated builds
5. **Distribution:** GitHub Releases

## Future Improvements

- Unit tests for services
- Integration tests
- Configuration UI for advanced users
- Plugin system for custom tools
- Cloud backup integration
- Multi-language support

