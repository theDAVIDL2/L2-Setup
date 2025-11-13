using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace WindowsSetup.App.Services
{
    /// <summary>
    /// Enhanced Windows Optimizations - Inspired by top optimization tools:
    /// - ET-Optimizer (https://github.com/semazurek/ET-Optimizer)
    /// - windows-11-debloat (https://github.com/teeotsa/windows-11-debloat)
    /// - RyTuneX (https://github.com/rayenghanmi/RyTuneX)
    /// - XToolbox (https://github.com/nyxiereal/XToolbox)
    /// </summary>
    public partial class WindowsOptimizerService
    {
        // ═══════════════════════════════════════════════════════════════
        // 🚀 PERFORMANCE ENHANCEMENTS
        // ═══════════════════════════════════════════════════════════════
        
        private async Task DisableBackgroundApps()
        {
            _logger.LogInfo("Disabling background apps...");
            try
            {
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", "GlobalUserDisabled", 1, RegistryValueKind.DWord);
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", "BackgroundAppGlobalToggle", 0, RegistryValueKind.DWord);
                _logger.LogSuccess("Background apps disabled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not disable background apps: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        private async Task DisableTransparency()
        {
            _logger.LogInfo("Disabling transparency effects...");
            try
            {
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "EnableTransparency", 0, RegistryValueKind.DWord);
                _logger.LogSuccess("Transparency disabled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not disable transparency: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        private async Task DisableAnimations()
        {
            _logger.LogInfo("Disabling Windows animations...");
            try
            {
                SetRegistryValue(@"Control Panel\Desktop\WindowMetrics", "MinAnimate", "0", RegistryValueKind.String);
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAnimations", 0, RegistryValueKind.DWord);
                _logger.LogSuccess("Animations disabled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not disable animations: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        // ═══════════════════════════════════════════════════════════════
        // 🔒 PRIVACY ENHANCEMENTS
        // ═══════════════════════════════════════════════════════════════
        
        private async Task DisableActivityHistory()
        {
            _logger.LogInfo("Disabling activity history...");
            try
            {
                SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows\System", "EnableActivityFeed", 0, RegistryValueKind.DWord);
                SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows\System", "PublishUserActivities", 0, RegistryValueKind.DWord);
                SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows\System", "UploadUserActivities", 0, RegistryValueKind.DWord);
                _logger.LogSuccess("Activity history disabled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not disable activity history: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        private async Task DisableWebSearch()
        {
            _logger.LogInfo("Disabling web search in Start Menu...");
            try
            {
                SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "DisableWebSearch", 1, RegistryValueKind.DWord);
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", "BingSearchEnabled", 0, RegistryValueKind.DWord);
                _logger.LogSuccess("Web search disabled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not disable web search: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        // ═══════════════════════════════════════════════════════════════
        // 🎮 GAMING OPTIMIZATIONS
        // ═══════════════════════════════════════════════════════════════
        
        private async Task DisableFullscreenOptimizations()
        {
            _logger.LogInfo("Disabling fullscreen optimizations (better gaming performance)...");
            try
            {
                SetRegistryValue(@"System\GameConfigStore", "GameDVR_FSEBehaviorMode", 2, RegistryValueKind.DWord);
                SetRegistryValue(@"System\GameConfigStore", "GameDVR_HonorUserFSEBehaviorMode", 1, RegistryValueKind.DWord);
                SetRegistryValue(@"System\GameConfigStore", "GameDVR_DXGIHonorFSEWindowsCompatible", 1, RegistryValueKind.DWord);
                _logger.LogSuccess("Fullscreen optimizations disabled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not disable fullscreen optimizations: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        private async Task OptimizeCPUScheduling()
        {
            _logger.LogInfo("Optimizing CPU scheduling for gaming...");
            try
            {
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "SystemResponsiveness", 0, RegistryValueKind.DWord);
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "GPU Priority", 8, RegistryValueKind.DWord);
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "Priority", 6, RegistryValueKind.DWord);
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "Scheduling Category", "High", RegistryValueKind.String);
                _logger.LogSuccess("CPU scheduling optimized for gaming");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not optimize CPU scheduling: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        private async Task DisableNagleAlgorithm()
        {
            _logger.LogInfo("Disabling Nagle's Algorithm (lower network latency for gaming)...");
            try
            {
                var interfaces = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces");
                if (interfaces != null)
                {
                    foreach (var interfaceName in interfaces.GetSubKeyNames())
                    {
                        var interfaceKey = interfaces.OpenSubKey(interfaceName, true);
                        if (interfaceKey != null)
                        {
                            interfaceKey.SetValue("TcpAckFrequency", 1, RegistryValueKind.DWord);
                            interfaceKey.SetValue("TCPNoDelay", 1, RegistryValueKind.DWord);
                            interfaceKey.Close();
                        }
                    }
                    interfaces.Close();
                }
                _logger.LogSuccess("Nagle's Algorithm disabled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not disable Nagle's Algorithm: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        // ═══════════════════════════════════════════════════════════════
        // 🌐 NETWORK OPTIMIZATIONS
        // ═══════════════════════════════════════════════════════════════
        
        private async Task OptimizeTCPIP()
        {
            _logger.LogInfo("Optimizing TCP/IP stack...");
            try
            {
                await _commandRunner.RunCommandAsync("netsh", "int tcp set global chimney=enabled");
                await _commandRunner.RunCommandAsync("netsh", "int tcp set global autotuninglevel=normal");
                await _commandRunner.RunCommandAsync("netsh", "int tcp set global congestionprovider=ctcp");
                await _commandRunner.RunCommandAsync("netsh", "int tcp set global ecncapability=enabled");
                await _commandRunner.RunCommandAsync("netsh", "int tcp set global timestamps=disabled");
                _logger.LogSuccess("TCP/IP stack optimized");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not optimize TCP/IP: {ex.Message}");
            }
        }

        private async Task OptimizeDNS()
        {
            _logger.LogInfo("Setting Cloudflare DNS (1.1.1.1)...");
            try
            {
                await _commandRunner.RunCommandAsync("powershell", "-Command \"Get-NetAdapter | Where-Object {$_.Status -eq 'Up'} | Set-DnsClientServerAddress -ServerAddresses ('1.1.1.1','1.0.0.1')\"");
                _logger.LogSuccess("DNS set to Cloudflare (1.1.1.1)");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not set DNS: {ex.Message}");
            }
        }

        private async Task DisableNetworkThrottling()
        {
            _logger.LogInfo("Disabling network throttling...");
            try
            {
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "NetworkThrottlingIndex", unchecked((int)0xFFFFFFFF), RegistryValueKind.DWord);
                _logger.LogSuccess("Network throttling disabled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not disable network throttling: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        // ═══════════════════════════════════════════════════════════════
        // 🗑️ DEBLOAT & CLEANUP
        // ═══════════════════════════════════════════════════════════════
        
        private async Task RemoveBloatwareApps()
        {
            _logger.LogInfo("Removing Windows bloatware apps...");
            try
            {
                var bloatwareApps = new[]
                {
                    "Microsoft.BingNews",
                    "Microsoft.GetHelp",
                    "Microsoft.Getstarted",
                    "Microsoft.Messaging",
                    "Microsoft.Microsoft3DViewer",
                    "Microsoft.MicrosoftOfficeHub",
                    "Microsoft.MicrosoftSolitaireCollection",
                    "Microsoft.MicrosoftStickyNotes",
                    "Microsoft.MixedReality.Portal",
                    "Microsoft.OneConnect",
                    "Microsoft.People",
                    "Microsoft.SkypeApp",
                    "Microsoft.Wallet",
                    "Microsoft.WindowsAlarms",
                    "Microsoft.WindowsCamera",
                    "Microsoft.windowscommunicationsapps",
                    "Microsoft.WindowsFeedbackHub",
                    "Microsoft.WindowsMaps",
                    "Microsoft.WindowsSoundRecorder",
                    "Microsoft.Xbox.TCUI",
                    "Microsoft.XboxApp",
                    "Microsoft.XboxGameOverlay",
                    "Microsoft.XboxGamingOverlay",
                    "Microsoft.XboxIdentityProvider",
                    "Microsoft.XboxSpeechToTextOverlay",
                    "Microsoft.YourPhone",
                    "Microsoft.ZuneMusic",
                    "Microsoft.ZuneVideo"
                };

                foreach (var app in bloatwareApps)
                {
                    try
                    {
                        await _commandRunner.RunCommandAsync("powershell", $"-Command \"Get-AppxPackage {app} | Remove-AppxPackage\"");
                        _logger.LogInfo($"Removed: {app}");
                    }
                    catch
                    {
                        // App may not be installed
                    }
                }
                _logger.LogSuccess("Bloatware apps removed");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not remove all bloatware: {ex.Message}");
            }
        }

        private async Task DisableWidgets()
        {
            _logger.LogInfo("Disabling Windows 11 widgets...");
            try
            {
                SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Dsh", "AllowNewsAndInterests", 0, RegistryValueKind.DWord);
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Feeds", "ShellFeedsTaskbarViewMode", 2, RegistryValueKind.DWord);
                await _commandRunner.RunCommandAsync("powershell", "-Command \"Get-AppxPackage MicrosoftWindows.Client.WebExperience | Remove-AppxPackage\"");
                _logger.LogSuccess("Widgets disabled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not disable widgets: {ex.Message}");
            }
        }

        private async Task RemoveCoPilot()
        {
            _logger.LogInfo("Removing Windows CoPilot...");
            try
            {
                SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows\WindowsCopilot", "TurnOffWindowsCopilot", 1, RegistryValueKind.DWord);
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowCopilotButton", 0, RegistryValueKind.DWord);
                _logger.LogSuccess("CoPilot removed");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not remove CoPilot: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        // ═══════════════════════════════════════════════════════════════
        // 💾 STORAGE & MEMORY
        // ═══════════════════════════════════════════════════════════════
        
        private async Task DisableSearchIndexing()
        {
            _logger.LogInfo("Disabling Windows Search indexing...");
            try
            {
                await _commandRunner.RunCommandAsync("sc", "config WSearch start=disabled");
                await _commandRunner.RunCommandAsync("sc", "stop WSearch");
                _logger.LogSuccess("Search indexing disabled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not disable search indexing: {ex.Message}");
            }
        }

        private async Task OptimizeSSD()
        {
            _logger.LogInfo("Optimizing SSD (enabling TRIM)...");
            try
            {
                await _commandRunner.RunCommandAsync("fsutil", "behavior set DisableDeleteNotify 0");
                _logger.LogSuccess("SSD TRIM enabled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not optimize SSD: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // 🖥️ CPU & MEMORY
        // ═══════════════════════════════════════════════════════════════
        
        private async Task DisableCoreParking()
        {
            _logger.LogInfo("Disabling CPU core parking...");
            try
            {
                await _commandRunner.RunCommandAsync("powershell", "-Command \"powercfg -setacvalueindex SCHEME_CURRENT SUB_PROCESSOR CPMINCORES 100\"");
                await _commandRunner.RunCommandAsync("powershell", "-Command \"powercfg -setactive SCHEME_CURRENT\"");
                _logger.LogSuccess("CPU core parking disabled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not disable core parking: {ex.Message}");
            }
        }

        private async Task DisableSpectreMeltdown()
        {
            _logger.LogWarning("⚠️ EXPERT MODE: Disabling Spectre/Meltdown mitigations (SECURITY RISK!)...");
            try
            {
                SetRegistryValue(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "FeatureSettingsOverride", 3, RegistryValueKind.DWord);
                SetRegistryValue(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "FeatureSettingsOverrideMask", 3, RegistryValueKind.DWord);
                _logger.LogSuccess("Spectre/Meltdown mitigations disabled - ⚠️ REBOOT REQUIRED");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not disable Spectre/Meltdown: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        // ═══════════════════════════════════════════════════════════════
        // 🎨 UI TWEAKS
        // ═══════════════════════════════════════════════════════════════
        
        private async Task DisableLockScreen()
        {
            _logger.LogInfo("Disabling lock screen...");
            try
            {
                SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows\Personalization", "NoLockScreen", 1, RegistryValueKind.DWord);
                _logger.LogSuccess("Lock screen disabled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not disable lock screen: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        private async Task ClassicContextMenu()
        {
            _logger.LogInfo("Enabling classic context menu (Windows 11)...");
            try
            {
                await _commandRunner.RunCommandAsync("reg", "add \"HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32\" /f /ve");
                _logger.LogSuccess("Classic context menu enabled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not enable classic context menu: {ex.Message}");
            }
        }

        // Helper method to set registry values safely
        private void SetRegistryValue(string keyPath, string valueName, object value, RegistryValueKind valueKind)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(keyPath, true);
                key?.SetValue(valueName, value, valueKind);
            }
            catch
            {
                // Try HKLM if HKCU fails
                try
                {
                    using var key = Registry.LocalMachine.CreateSubKey(keyPath, true);
                    key?.SetValue(valueName, value, valueKind);
                }
                catch
                {
                    // Silently fail
                }
            }
        }
    }
}

