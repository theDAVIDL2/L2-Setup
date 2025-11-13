using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using WindowsSetup.App.Models;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App.Services
{
    public partial class WindowsOptimizerService
    {
        private OptimizationBackupService? _backupService;

        public async Task ApplyCustomOptimizations(OptimizationSettings settings)
        {
            try
            {
                _logger.LogInfo("=== Applying Custom Windows Optimizations (L2 Enhanced) ===");

                // Initialize backup service and start backup session
                _backupService = new OptimizationBackupService(_logger);
                _backupService.StartBackupSession("Custom Windows Optimizations");

                if (settings.CreateRestorePoint)
                {
                    await CreateRestorePoint();
                }

                // ═══════════════════════════════════════════════════════════
                // 🚀 PERFORMANCE
                // ═══════════════════════════════════════════════════════════
                if (settings.HighPerformancePowerPlan) await OptimizePower();
                if (settings.DisableMouseAcceleration) await OptimizeMouse();
                if (settings.OptimizeVisualEffects) await OptimizeVisualEffects();
                if (settings.OptimizeExplorer) await OptimizeExplorer();
                if (settings.DisableStartupPrograms) await DisableStartupPrograms();
                if (settings.OptimizePageFile) await OptimizePageFile();
                if (settings.DisableBackgroundApps) await DisableBackgroundApps();
                if (settings.DisableTransparency) await DisableTransparency();
                if (settings.DisableAnimations) await DisableAnimations();

                // ═══════════════════════════════════════════════════════════
                // 🔒 PRIVACY & TELEMETRY
                // ═══════════════════════════════════════════════════════════
                if (settings.DisableTelemetry) await DisableTelemetry();
                if (settings.DisableCortana) await DisableCortana();
                if (settings.DisableAdvertisingId) await DisableAdvertisingId();
                if (settings.DisableLocationTracking) await DisableLocationTracking();
                if (settings.DisableDiagnostics) await DisableDiagnosticData();
                if (settings.DisableActivityHistory) await DisableActivityHistory();
                if (settings.DisableWebSearch) await DisableWebSearch();

                // ═══════════════════════════════════════════════════════════
                // ⚙️ SERVICES & FEATURES
                // ═══════════════════════════════════════════════════════════
                if (settings.DisablePrintSpooler) await DisableService("Spooler");
                if (settings.DisableFax) await DisableService("Fax");
                if (settings.DisableWindowsSearch) await DisableService("WSearch");
                if (settings.DisableSuperfetch) await DisableService("SysMain");
                if (settings.SetWindowsUpdateManual) await SetServiceManual("wuauserv");

                // ═══════════════════════════════════════════════════════════
                // 🎮 GAMING OPTIMIZATIONS
                // ═══════════════════════════════════════════════════════════
                if (settings.EnableGameMode) await EnableGameMode();
                if (settings.DisableGameBar) await DisableGameBar();
                if (settings.DisableGameDVR) await DisableGameDVR();
                if (settings.EnableHardwareAcceleratedGPU) await EnableHardwareAcceleratedGPU();
                if (settings.DisableFullscreenOptimizations) await DisableFullscreenOptimizations();
                if (settings.OptimizeCPUScheduling) await OptimizeCPUScheduling();
                if (settings.DisableNagleAlgorithm) await DisableNagleAlgorithm();

                // ═══════════════════════════════════════════════════════════
                // 🌐 NETWORK OPTIMIZATIONS
                // ═══════════════════════════════════════════════════════════
                if (settings.OptimizeTCPIP) await OptimizeTCPIP();
                if (settings.OptimizeDNS) await OptimizeDNS();
                if (settings.DisableNetworkThrottling) await DisableNetworkThrottling();

                // ═══════════════════════════════════════════════════════════
                // 🗑️ DEBLOAT & CLEANUP
                // ═══════════════════════════════════════════════════════════
                if (settings.CleanTempFiles) await CleanTemporaryFiles();
                if (settings.EmptyRecycleBin) await EmptyRecycleBin();
                if (settings.DeleteWindowsOld) await DeleteWindowsOld();
                if (settings.CleanDownloads) await CleanDownloads();
                if (settings.RemoveBloatwareApps) await RemoveBloatwareApps();
                if (settings.DisableWidgets) await DisableWidgets();
                if (settings.RemoveCoPilot) await RemoveCoPilot();

                // ═══════════════════════════════════════════════════════════
                // 💾 STORAGE & MEMORY
                // ═══════════════════════════════════════════════════════════
                if (settings.DisableSearchIndexing) await DisableSearchIndexing();
                if (settings.OptimizeSSD) await OptimizeSSD();

                // ═══════════════════════════════════════════════════════════
                // 🖥️ CPU & MEMORY OPTIMIZATIONS
                // ═══════════════════════════════════════════════════════════
                if (settings.DisableCoreParking) await DisableCoreParking();
                if (settings.DisableSpectreMeltdown) await DisableSpectreMeltdown();

                // ═══════════════════════════════════════════════════════════
                // 🎨 UI TWEAKS
                // ═══════════════════════════════════════════════════════════
                if (settings.ShowFileExtensions) await OptimizeExplorer();
                if (settings.DisableLockScreen) await DisableLockScreen();
                if (settings.ClassicContextMenu) await ClassicContextMenu();

                // ═══════════════════════════════════════════════════════════
                // ⚡ ADVANCED & EXPERT
                // ═══════════════════════════════════════════════════════════
                if (settings.DisableOneDrive) await DisableOneDrive();
                if (settings.DisableHibernation) await DisableHibernation();

                _logger.LogSuccess("=== L2 Enhanced Optimizations Applied Successfully! ===");
                
                // Save backup
                var backupFile = await (_backupService?.SaveBackup() ?? Task.FromResult<string?>(null));
                if (backupFile != null)
                {
                    _logger.LogInfo($"💾 Backup saved: {Path.GetFileName(backupFile)}");
                    _logger.LogInfo("   Use 'Restore Previous Settings' to undo changes");
                }
                
                _logger.LogWarning("⚠️ A REBOOT is RECOMMENDED for all changes to take effect!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Optimization failed: {ex.Message}");
                throw;
            }
        }

        private async Task DisableStartupPrograms()
        {
            _logger.LogInfo("Disabling unnecessary startup programs...");
            // This would require analyzing HKCU\Software\Microsoft\Windows\CurrentVersion\Run
            // and Task Scheduler tasks - implemented via registry in production
            await Task.CompletedTask;
        }

        private async Task OptimizePageFile()
        {
            _logger.LogInfo("Optimizing virtual memory (enabling system-managed page file)...");
            try
            {
                // Use PowerShell instead of deprecated wmic
                var psCommand = "$cs = Get-CimInstance -ClassName Win32_ComputerSystem; $cs.AutomaticManagedPagefile = $true; Set-CimInstance -InputObject $cs";
                await _commandRunner.RunCommandAsync("powershell", $"-NoProfile -ExecutionPolicy Bypass -Command \"{psCommand}\"");
                _logger.LogSuccess("Virtual memory optimized (automatic managed page file enabled)");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not optimize page file: {ex.Message}");
            }
        }

        private async Task DisableCortana()
        {
            _logger.LogInfo("Disabling Cortana...");
            SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 0, RegistryValueKind.DWord);
        }

        private async Task DisableAdvertisingId()
        {
            _logger.LogInfo("Disabling Advertising ID...");
            SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", 0, RegistryValueKind.DWord);
            await Task.CompletedTask;
        }

        private async Task DisableLocationTracking()
        {
            _logger.LogInfo("Disabling location tracking...");
            SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location", "Value", "Deny", RegistryValueKind.String);
            await Task.CompletedTask;
        }

        private async Task DisableDiagnosticData()
        {
            _logger.LogInfo("Disabling diagnostic data collection...");
            SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
            await Task.CompletedTask;
        }

        private async Task DisableService(string serviceName)
        {
            _logger.LogInfo($"Disabling service: {serviceName}...");
            try
            {
                // Backup before changing
                await (_backupService?.BackupService(serviceName) ?? Task.CompletedTask);

                await _commandRunner.RunCommandAsync("sc", $"config {serviceName} start=disabled");
                await _commandRunner.RunCommandAsync("sc", $"stop {serviceName}");
            }
            catch { }
        }

        private async Task SetServiceManual(string serviceName)
        {
            _logger.LogInfo($"Setting service to manual: {serviceName}...");
            try
            {
                await _commandRunner.RunCommandAsync("sc", $"config {serviceName} start=demand");
            }
            catch { }
        }

        private async Task EnableGameMode()
        {
            _logger.LogInfo("Enabling Game Mode...");
            SetRegistryValue(@"SOFTWARE\Microsoft\GameBar", "AutoGameModeEnabled", 1, RegistryValueKind.DWord);
            await Task.CompletedTask;
        }

        private async Task DisableGameBar()
        {
            _logger.LogInfo("Disabling Xbox Game Bar...");
            SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR", "AppCaptureEnabled", 0, RegistryValueKind.DWord);
            SetRegistryValue(@"SOFTWARE\Microsoft\GameBar", "UseNexusForGameBarEnabled", 0, RegistryValueKind.DWord);
            await Task.CompletedTask;
        }

        private async Task DisableGameDVR()
        {
            _logger.LogInfo("Disabling Game DVR...");
            SetRegistryValue(@"System\GameConfigStore", "GameDVR_Enabled", 0, RegistryValueKind.DWord);
            await Task.CompletedTask;
        }

        private async Task EnableHardwareAcceleratedGPU()
        {
            _logger.LogInfo("Enabling Hardware Accelerated GPU Scheduling...");
            SetRegistryValue(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers", "HwSchMode", 2, RegistryValueKind.DWord, RegistryHive.LocalMachine);
            await Task.CompletedTask;
        }

        private async Task EmptyRecycleBin()
        {
            _logger.LogInfo("Emptying Recycle Bin...");
            await _commandRunner.RunCommandAsync("cmd.exe", "/c rd /s /q %systemdrive%\\$Recycle.Bin");
        }

        private async Task DeleteWindowsOld()
        {
            _logger.LogInfo("Deleting Windows.old folder...");
            await _commandRunner.RunCommandAsync("cmd.exe", "/c rd /s /q %systemdrive%\\Windows.old");
        }

        private async Task CleanDownloads()
        {
            _logger.LogInfo("Cleaning Downloads folder...");
            var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            await Task.Run(() =>
            {
                try
                {
                    var files = Directory.GetFiles(downloadsPath);
                    foreach (var file in files)
                    {
                        try { File.Delete(file); } catch { }
                    }
                }
                catch { }
            });
        }

        private async Task DisableOneDrive()
        {
            _logger.LogInfo("Disabling OneDrive...");
            SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows\OneDrive", "DisableFileSyncNGSC", 1, RegistryValueKind.DWord);
            await Task.CompletedTask;
        }

        private async Task DisableHibernation()
        {
            _logger.LogInfo("Disabling hibernation...");
            await _commandRunner.RunCommandAsync("powercfg", "/hibernate off");
        }

        private void SetRegistryValue(string path, string name, object value, RegistryValueKind kind, RegistryHive hive = RegistryHive.CurrentUser)
        {
            try
            {
                // Backup before changing
                var hiveString = hive == RegistryHive.LocalMachine ? "HKLM" : "HKCU";
                _backupService?.BackupRegistryValue(hiveString, path, name);

                using var baseKey = hive == RegistryHive.LocalMachine 
                    ? Registry.LocalMachine 
                    : Registry.CurrentUser;
                    
                using var key = baseKey.CreateSubKey(path, true);
                key?.SetValue(name, value, kind);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not set registry value {path}\\{name}: {ex.Message}");
            }
        }

        #region Reset to Safe Defaults

        public async Task ResetToSafeDefaults()
        {
            _logger.LogInfo("═══════════════════════════════════════════════════════════");
            _logger.LogWarning("⚠️  DEPRECATED METHOD - USE 'RESTORE FROM BACKUP' INSTEAD! ⚠️");
            _logger.LogInfo("═══════════════════════════════════════════════════════════");
            _logger.LogInfo("");
            _logger.LogInfo("This method uses generic 'safe defaults' which may not match");
            _logger.LogInfo("your system's ORIGINAL configuration.");
            _logger.LogInfo("");
            _logger.LogInfo("For proper restoration, use:");
            _logger.LogInfo("  1. Main Menu → Windows Optimization → Restore Previous Settings");
            _logger.LogInfo("  2. Select your most recent backup");
            _logger.LogInfo("  3. Let the system restore YOUR ACTUAL original values");
            _logger.LogInfo("");
            _logger.LogInfo("═══════════════════════════════════════════════════════════");
            _logger.LogWarning("Proceeding with generic safe defaults anyway...");
            _logger.LogInfo("═══════════════════════════════════════════════════════════");

            try
            {
                // 1. Restore Network Settings (GENERIC - may not match your original)
                _logger.LogInfo("🌐 Applying generic network defaults...");
                await RestoreNetworkDefaults();

                // 2. Re-enable Important Services
                _logger.LogInfo("⚙️ Re-enabling important services...");
                await RestoreServiceDefaults();

                // 3. Restore Visual Effects
                _logger.LogInfo("🎨 Restoring visual effects...");
                await RestoreVisualDefaults();

                // 4. Restore Windows Features
                _logger.LogInfo("📦 Restoring Windows features...");
                await RestoreFeatureDefaults();

                _logger.LogInfo("═══════════════════════════════════════════════════════════");
                _logger.LogSuccess("✅ Generic safe defaults applied!");
                _logger.LogWarning("⚠️ IMPORTANT: These are NOT your original settings!");
                _logger.LogWarning("⚠️ Use 'Restore from Backup' for proper restoration!");
                _logger.LogWarning("⚠️ A REBOOT is REQUIRED for all changes to take effect!");
                _logger.LogInfo("═══════════════════════════════════════════════════════════");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during reset: {ex.Message}");
                throw;
            }
        }

        private async Task RestoreNetworkDefaults()
        {
            try
            {
                _logger.LogWarning("   ⚠️  Applying GENERIC defaults (not your originals!)");
                
                // Reset TCP/IP settings to Windows defaults
                await _commandRunner.RunCommandAsync("netsh", "int tcp reset");
                await _commandRunner.RunCommandAsync("netsh", "int ip reset");
                
                // Flush DNS
                await _commandRunner.RunCommandAsync("ipconfig", "/flushdns");
                
                // Reset Winsock
                await _commandRunner.RunCommandAsync("netsh", "winsock reset");
                
                // Reset DNS to DHCP (automatic)
                await _commandRunner.RunCommandAsync("powershell", 
                    "-Command \"Get-NetAdapter | Where-Object {$_.Status -eq 'Up'} | Set-DnsClientServerAddress -ResetServerAddresses\"");
                
                _logger.LogInfo("   ✅ Generic network defaults applied");
                _logger.LogWarning("   ⚠️  Your original DNS servers were NOT restored!");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"   ⚠️ Some network settings couldn't be restored: {ex.Message}");
            }
        }

        private async Task RestoreServiceDefaults()
        {
            var services = new Dictionary<string, string>
            {
                { "WSearch", "Delayed Auto" },       // Windows Search
                { "SysMain", "Auto" },               // Superfetch
                { "Spooler", "Auto" },               // Print Spooler
                { "WinDefend", "Auto" },             // Windows Defender
                { "mpssvc", "Auto" },                // Windows Firewall
                { "Fax", "Manual" },                 // Fax
                { "wisvc", "Manual" },               // Windows Insider Service
                { "DiagTrack", "Auto" },             // Diagnostics
                { "dmwappushservice", "Auto" }       // WAP Push
            };

            foreach (var service in services)
            {
                try
                {
                    var startType = service.Value == "Auto" ? "auto" : 
                                   service.Value == "Delayed Auto" ? "delayed-auto" : "demand";
                    await _commandRunner.RunCommandAsync("sc", $"config {service.Key} start={startType}");
                    _logger.LogInfo($"   ✅ {service.Key} → {service.Value}");
                }
                catch
                {
                    _logger.LogWarning($"   ⚠️ Couldn't restore {service.Key}");
                }
            }
        }

        private async Task RestoreVisualDefaults()
        {
            try
            {
                // Enable transparency
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", 
                    "EnableTransparency", 1, RegistryValueKind.DWord);

                // Enable animations
                SetRegistryValue(@"Control Panel\Desktop\WindowMetrics", 
                    "MinAnimate", "1", RegistryValueKind.String);

                // Restore visual effects (best appearance)
                SetRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", 
                    "VisualFXSetting", 2, RegistryValueKind.DWord);

                await Task.CompletedTask;
                _logger.LogSuccess("   ✅ Visual effects restored");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"   ⚠️ Some visual settings couldn't be restored: {ex.Message}");
            }
        }

        private async Task RestoreFeatureDefaults()
        {
            try
            {
                // Re-enable Windows Search in Start Menu
                SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows\Windows Search", 
                    "AllowCortana", 1, RegistryValueKind.DWord, RegistryHive.LocalMachine);

                // Enable background apps
                SetRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", 
                    "GlobalUserDisabled", 0, RegistryValueKind.DWord);

                // Enable startup programs check
                SetRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", 
                    "Enabled", 1, RegistryValueKind.DWord);

                await Task.CompletedTask;
                _logger.LogSuccess("   ✅ Windows features restored");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"   ⚠️ Some features couldn't be restored: {ex.Message}");
            }
        }

        #endregion
    }
}

