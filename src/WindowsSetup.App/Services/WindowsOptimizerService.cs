using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App.Services
{
    public class WindowsOptimizerService
    {
        private readonly Logger _logger;
        private readonly CommandRunner _commandRunner;

        public WindowsOptimizerService(Logger logger)
        {
            _logger = logger;
            _commandRunner = new CommandRunner();
        }

        public async Task ApplyAllOptimizations()
        {
            try
            {
                _logger.LogInfo("=== Starting Windows Optimizations ===");

                // Create restore point first
                _logger.LogInfo("Creating system restore point...");
                await CreateRestorePoint();

                await OptimizePower();
                await OptimizeMouse();
                await OptimizeVisualEffects();
                await DisableUnnecessaryServices();
                await OptimizeExplorer();
                await DisableTelemetry();
                await CleanTemporaryFiles();

                _logger.LogSuccess("=== All optimizations applied successfully! ===");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Optimization failed: {ex.Message}");
                throw;
            }
        }

        private async Task CreateRestorePoint()
        {
            try
            {
                var result = await _commandRunner.RunPowerShellAsync(
                    "Checkpoint-Computer -Description 'Before WindowsSetup Optimizations' -RestorePointType 'MODIFY_SETTINGS'",
                    requireAdmin: true);

                if (result.Success)
                {
                    _logger.LogSuccess("Restore point created successfully");
                }
                else
                {
                    _logger.LogWarning("Could not create restore point, continuing anyway...");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Restore point creation failed: {ex.Message}");
            }
        }

        private async Task OptimizePower()
        {
            _logger.LogInfo("Optimizing power settings...");

            try
            {
                // Duplicate High Performance scheme
                await _commandRunner.RunCommandAsync("powercfg", 
                    "-duplicatescheme e9a42b02-d5df-448d-aa00-03baaac53275");

                // Set to High Performance
                await _commandRunner.RunCommandAsync("powercfg", "-setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");

                // Disable sleep and monitor timeout when plugged in
                await _commandRunner.RunCommandAsync("powercfg", "/change monitor-timeout-ac 0");
                await _commandRunner.RunCommandAsync("powercfg", "/change standby-timeout-ac 0");
                await _commandRunner.RunCommandAsync("powercfg", "/change hibernate-timeout-ac 0");

                _logger.LogSuccess("Power settings optimized");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Power optimization failed: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        private async Task OptimizeMouse()
        {
            _logger.LogInfo("Disabling mouse acceleration...");

            try
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Mouse", "MouseSpeed", "0", RegistryValueKind.String);
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Mouse", "MouseThreshold1", "0", RegistryValueKind.String);
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Mouse", "MouseThreshold2", "0", RegistryValueKind.String);

                _logger.LogSuccess("Mouse acceleration disabled (restart required for full effect)");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Mouse optimization failed: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        private async Task OptimizeVisualEffects()
        {
            _logger.LogInfo("Optimizing visual effects for performance...");

            try
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects",
                    "VisualFXSetting", 2, RegistryValueKind.DWord);

                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop",
                    "UserPreferencesMask", new byte[] { 0x90, 0x12, 0x03, 0x80, 0x10, 0x00, 0x00, 0x00 },
                    RegistryValueKind.Binary);

                _logger.LogSuccess("Visual effects optimized");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Visual effects optimization failed: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        private async Task DisableUnnecessaryServices()
        {
            _logger.LogInfo("Disabling unnecessary services...");

            var services = new[]
            {
                "SysMain", // Superfetch
                "DmWappushService", // WAP Push
                "diagtrack", // Diagnostics Tracking
                "MapsBroker", // Downloaded Maps Manager
                "RetailDemo" // Retail Demo Service
            };

            foreach (var service in services)
            {
                try
                {
                    await _commandRunner.RunCommandAsync("sc", $"config \"{service}\" start=disabled");
                    _logger.LogInfo($"Disabled service: {service}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Could not disable {service}: {ex.Message}");
                }
            }

            _logger.LogSuccess("Unnecessary services disabled");
        }

        private async Task OptimizeExplorer()
        {
            _logger.LogInfo("Optimizing File Explorer...");

            try
            {
                // Show file extensions
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                    "HideFileExt", 0, RegistryValueKind.DWord);

                // Show hidden files
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                    "Hidden", 1, RegistryValueKind.DWord);

                // Open to This PC instead of Quick Access
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                    "LaunchTo", 1, RegistryValueKind.DWord);

                _logger.LogSuccess("File Explorer optimized");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Explorer optimization failed: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        private async Task DisableTelemetry()
        {
            _logger.LogInfo("Disabling telemetry and data collection...");

            try
            {
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DataCollection",
                    "AllowTelemetry", 0, RegistryValueKind.DWord);

                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection",
                    "AllowTelemetry", 0, RegistryValueKind.DWord);

                _logger.LogSuccess("Telemetry disabled");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Telemetry disabling failed: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        private async Task CleanTemporaryFiles()
        {
            _logger.LogInfo("Cleaning temporary files...");

            try
            {
                var tempPaths = new[]
                {
                    Environment.GetEnvironmentVariable("TEMP"),
                    Path.Combine(Environment.GetEnvironmentVariable("SystemRoot") ?? "C:\\Windows", "Temp")
                };

                foreach (var tempPath in tempPaths)
                {
                    if (string.IsNullOrEmpty(tempPath) || !Directory.Exists(tempPath))
                        continue;

                    try
                    {
                        var files = Directory.GetFiles(tempPath, "*", SearchOption.AllDirectories);
                        var deletedCount = 0;

                        foreach (var file in files)
                        {
                            try
                            {
                                File.Delete(file);
                                deletedCount++;
                            }
                            catch
                            {
                                // Skip files in use
                            }
                        }

                        _logger.LogInfo($"Cleaned {deletedCount} files from {tempPath}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Could not clean {tempPath}: {ex.Message}");
                    }
                }

                _logger.LogSuccess("Temporary files cleaned");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Cleanup failed: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        public async Task RunChrisTitusScript()
        {
            try
            {
                _logger.LogInfo("Running Chris Titus Tech optimization script...");
                _logger.LogWarning("This will open a new window with advanced options");

                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"irm christitus.com/win | iex\"",
                    UseShellExecute = true,
                    Verb = "runas"
                };

                Process.Start(psi);

                _logger.LogInfo("Script launched. Follow the on-screen instructions.");
                
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run CTT script: {ex.Message}");
                throw;
            }
        }
    }
}

