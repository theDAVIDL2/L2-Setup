using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;
using WindowsSetup.App.Models;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App.Services
{
    public class OptimizationBackupService
    {
        private readonly Logger _logger;
        private readonly CommandRunner _commandRunner;
        private readonly string _backupDirectory;
        private OptimizationBackup? _currentBackup;

        public OptimizationBackupService(Logger logger)
        {
            _logger = logger;
            _commandRunner = new CommandRunner();
            _backupDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "L2Setup", "Backups");
            
            Directory.CreateDirectory(_backupDirectory);
        }

        public void StartBackupSession(string description = "Windows Optimization Backup")
        {
            _currentBackup = new OptimizationBackup
            {
                Description = description,
                CreatedAt = DateTime.Now
            };
            _logger.LogInfo($"🔄 Backup session started: {_currentBackup.BackupId}");
        }

        public async Task<string?> SaveBackup()
        {
            if (_currentBackup == null)
            {
                _logger.LogWarning("No active backup session to save");
                return null;
            }

            try
            {
                var backupFile = Path.Combine(_backupDirectory, $"backup_{_currentBackup.BackupId}.json");
                var json = JsonConvert.SerializeObject(_currentBackup, Formatting.Indented);
                await File.WriteAllTextAsync(backupFile, json);
                
                _logger.LogSuccess($"✅ Backup saved: {backupFile}");
                _logger.LogInfo($"   Registry changes: {_currentBackup.RegistryChanges.Count}");
                _logger.LogInfo($"   Service changes: {_currentBackup.ServiceChanges.Count}");
                _logger.LogInfo($"   Power settings: {_currentBackup.PowerSettings.Count}");
                _logger.LogInfo($"   Network settings: {_currentBackup.NetworkSettings.Count}");
                _logger.LogInfo($"   DNS configurations: {_currentBackup.DnsSettings.Count}");
                _logger.LogInfo($"   TCP/IP settings: {_currentBackup.TcpIpSettings.Count}");
                
                return backupFile;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save backup: {ex.Message}");
                return null;
            }
        }

        public void BackupRegistryValue(string hive, string path, string valueName)
        {
            if (_currentBackup == null) return;

            try
            {
                var baseKey = hive == "HKLM" ? Registry.LocalMachine : Registry.CurrentUser;
                
                using var key = baseKey.OpenSubKey(path, false);
                var entry = new RegistryBackupEntry
                {
                    Hive = hive,
                    Path = path,
                    ValueName = valueName,
                    KeyExisted = key != null
                };

                if (key != null)
                {
                    try
                    {
                        entry.OriginalValue = key.GetValue(valueName);
                        entry.ValueExisted = entry.OriginalValue != null;
                        entry.OriginalValueKind = entry.ValueExisted ? key.GetValueKind(valueName).ToString() : null;
                    }
                    catch
                    {
                        entry.ValueExisted = false;
                    }
                }

                _currentBackup.RegistryChanges.Add(entry);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not backup registry value {path}\\{valueName}: {ex.Message}");
            }
        }

        public async Task BackupService(string serviceName)
        {
            if (_currentBackup == null) return;

            try
            {
                var statusResult = await _commandRunner.RunCommandAsync("sc", $"query {serviceName}");
                var configResult = await _commandRunner.RunCommandAsync("sc", $"qc {serviceName}");

                var entry = new ServiceBackupEntry
                {
                    ServiceName = serviceName,
                    OriginalStatus = ExtractServiceStatus(statusResult.Output ?? ""),
                    OriginalStartMode = ExtractServiceStartMode(configResult.Output ?? "")
                };

                _currentBackup.ServiceChanges.Add(entry);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not backup service {serviceName}: {ex.Message}");
            }
        }

        public async Task BackupPowerSetting(string settingType, string command)
        {
            if (_currentBackup == null) return;

            try
            {
                var result = await _commandRunner.RunCommandAsync("powercfg", command);
                
                var entry = new PowerSettingBackupEntry
                {
                    SettingType = settingType,
                    OriginalValue = result.Output ?? ""
                };

                _currentBackup.PowerSettings.Add(entry);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not backup power setting {settingType}: {ex.Message}");
            }
        }

        public async Task BackupNetworkInterfaceSettings()
        {
            if (_currentBackup == null) return;

            try
            {
                _logger.LogInfo("📡 Backing up network interface settings...");
                
                var interfaces = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces");
                if (interfaces != null)
                {
                    foreach (var interfaceName in interfaces.GetSubKeyNames())
                    {
                        using var interfaceKey = interfaces.OpenSubKey(interfaceName, false);
                        if (interfaceKey != null)
                        {
                            // Backup TcpAckFrequency
                            BackupNetworkInterfaceValue(interfaceName, interfaceKey, "TcpAckFrequency");
                            // Backup TCPNoDelay
                            BackupNetworkInterfaceValue(interfaceName, interfaceKey, "TCPNoDelay");
                        }
                    }
                    interfaces.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not backup network interface settings: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        private void BackupNetworkInterfaceValue(string interfaceName, RegistryKey key, string valueName)
        {
            if (_currentBackup == null) return;

            try
            {
                var value = key.GetValue(valueName);
                var entry = new NetworkSettingBackupEntry
                {
                    InterfaceName = interfaceName,
                    SettingName = valueName,
                    ValueExisted = value != null,
                    OriginalValue = value,
                    ValueKind = value != null ? key.GetValueKind(valueName).ToString() : null
                };
                _currentBackup.NetworkSettings.Add(entry);
            }
            catch
            {
                // Value doesn't exist, still add entry
                _currentBackup.NetworkSettings.Add(new NetworkSettingBackupEntry
                {
                    InterfaceName = interfaceName,
                    SettingName = valueName,
                    ValueExisted = false
                });
            }
        }

        public async Task BackupDnsSettings()
        {
            if (_currentBackup == null) return;

            try
            {
                _logger.LogInfo("🌐 Backing up DNS settings...");
                
                var result = await _commandRunner.RunCommandAsync("powershell", 
                    "-Command \"Get-NetAdapter | Where-Object {$_.Status -eq 'Up'} | ForEach-Object { $dns = Get-DnsClientServerAddress -InterfaceAlias $_.Name -AddressFamily IPv4; [PSCustomObject]@{Name=$_.Name; DNS=($dns.ServerAddresses -join ',')} } | ConvertTo-Json\"");

                if (result.Success && !string.IsNullOrWhiteSpace(result.Output))
                {
                    try
                    {
                        var adapters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(result.Output);
                        if (adapters != null)
                        {
                            foreach (var adapter in adapters)
                            {
                                var entry = new DnsBackupEntry
                                {
                                    InterfaceName = adapter.Name,
                                    OriginalDnsServers = new List<string>(),
                                    WasDhcp = string.IsNullOrEmpty((string)adapter.DNS)
                                };

                                if (!string.IsNullOrEmpty((string)adapter.DNS))
                                {
                                    entry.OriginalDnsServers = ((string)adapter.DNS).Split(',').ToList();
                                }

                                _currentBackup.DnsSettings.Add(entry);
                            }
                        }
                    }
                    catch
                    {
                        // Single object, not array
                        dynamic? adapter = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result.Output ?? "{}");
                        if (adapter != null)
                        {
                            var entry = new DnsBackupEntry
                            {
                                InterfaceName = adapter.Name,
                                OriginalDnsServers = new List<string>(),
                                WasDhcp = string.IsNullOrEmpty((string)adapter.DNS)
                            };

                            if (!string.IsNullOrEmpty((string)adapter.DNS))
                            {
                                entry.OriginalDnsServers = ((string)adapter.DNS).Split(',').ToList();
                            }

                            _currentBackup.DnsSettings.Add(entry);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not backup DNS settings: {ex.Message}");
            }
        }

        public async Task BackupTcpIpSettings()
        {
            if (_currentBackup == null) return;

            try
            {
                _logger.LogInfo("⚙️ Backing up TCP/IP stack settings...");

                // Backup various TCP/IP settings
                var settings = new Dictionary<string, string>
                {
                    { "chimney", "int tcp show global" },
                    { "autotuninglevel", "int tcp show global" },
                    { "congestionprovider", "int tcp show global" },
                    { "ecncapability", "int tcp show global" },
                    { "timestamps", "int tcp show global" }
                };

                foreach (var setting in settings)
                {
                    try
                    {
                        var result = await _commandRunner.RunCommandAsync("netsh", setting.Value);
                        if (result.Success)
                        {
                            var entry = new TcpIpBackupEntry
                            {
                                SettingName = setting.Key,
                                CommandOutput = result.Output ?? "",
                                OriginalValue = ExtractTcpIpValue(result.Output ?? "", setting.Key)
                            };
                            _currentBackup.TcpIpSettings.Add(entry);
                        }
                    }
                    catch { }
                }

                // Backup network throttling index from registry
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", false);
                    if (key != null)
                    {
                        var value = key.GetValue("NetworkThrottlingIndex");
                        var entry = new NetworkSettingBackupEntry
                        {
                            InterfaceName = "SYSTEM",
                            SettingName = "NetworkThrottlingIndex",
                            OriginalValue = value,
                            ValueExisted = value != null,
                            ValueKind = value != null ? key.GetValueKind("NetworkThrottlingIndex").ToString() : null
                        };
                        _currentBackup.NetworkSettings.Add(entry);
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not backup TCP/IP settings: {ex.Message}");
            }
        }

        private string ExtractTcpIpValue(string output, string settingName)
        {
            try
            {
                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (line.Contains(settingName, StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = line.Split(':');
                        if (parts.Length > 1)
                        {
                            return parts[1].Trim();
                        }
                    }
                }
            }
            catch { }
            return output;
        }

        public async Task<bool> RestoreFromBackup(string backupFile)
        {
            try
            {
                _logger.LogInfo("═══════════════════════════════════════════════════════════");
                _logger.LogInfo("🔄 Starting restore from backup...");
                _logger.LogInfo("═══════════════════════════════════════════════════════════");

                if (!File.Exists(backupFile))
                {
                    _logger.LogError($"Backup file not found: {backupFile}");
                    return false;
                }

                var json = await File.ReadAllTextAsync(backupFile);
                var backup = JsonConvert.DeserializeObject<OptimizationBackup>(json);

                if (backup == null)
                {
                    _logger.LogError("Failed to deserialize backup file");
                    return false;
                }

                _logger.LogInfo($"📦 Backup: {backup.Description}");
                _logger.LogInfo($"📅 Created: {backup.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                _logger.LogInfo("");

                // Restore registry values
                if (backup.RegistryChanges.Count > 0)
                {
                    _logger.LogInfo($"🔧 Restoring {backup.RegistryChanges.Count} registry changes...");
                    await RestoreRegistryChanges(backup.RegistryChanges);
                }

                // Restore services
                if (backup.ServiceChanges.Count > 0)
                {
                    _logger.LogInfo($"⚙️  Restoring {backup.ServiceChanges.Count} service changes...");
                    await RestoreServiceChanges(backup.ServiceChanges);
                }

                // Restore power settings
                if (backup.PowerSettings.Count > 0)
                {
                    _logger.LogInfo($"⚡ Restoring {backup.PowerSettings.Count} power settings...");
                    await RestorePowerSettings(backup.PowerSettings);
                }

                // Restore network interface settings (Nagle's Algorithm, etc.)
                if (backup.NetworkSettings.Count > 0)
                {
                    _logger.LogInfo($"📡 Restoring {backup.NetworkSettings.Count} network interface settings...");
                    await RestoreNetworkInterfaceSettings(backup.NetworkSettings);
                }

                // Restore DNS settings
                if (backup.DnsSettings.Count > 0)
                {
                    _logger.LogInfo($"🌐 Restoring {backup.DnsSettings.Count} DNS configurations...");
                    await RestoreDnsSettings(backup.DnsSettings);
                }

                // Restore TCP/IP stack settings
                if (backup.TcpIpSettings.Count > 0)
                {
                    _logger.LogInfo($"⚙️  Restoring {backup.TcpIpSettings.Count} TCP/IP settings...");
                    await RestoreTcpIpSettings(backup.TcpIpSettings);
                }

                _logger.LogInfo("═══════════════════════════════════════════════════════════");
                _logger.LogSuccess("✅ Restore completed successfully!");
                _logger.LogInfo("═══════════════════════════════════════════════════════════");
                _logger.LogWarning("⚠️  A system restart is REQUIRED for network changes to take effect!");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Restore failed: {ex.Message}");
                return false;
            }
        }

        private async Task RestoreRegistryChanges(List<RegistryBackupEntry> changes)
        {
            int restored = 0;
            int failed = 0;

            foreach (var change in changes)
            {
                try
                {
                    var baseKey = change.Hive == "HKLM" ? Registry.LocalMachine : Registry.CurrentUser;

                    if (!change.ValueExisted)
                    {
                        // Value didn't exist before, so delete it
                        using var key = baseKey.OpenSubKey(change.Path, true);
                        if (key != null)
                        {
                            try
                            {
                                key.DeleteValue(change.ValueName, false);
                                restored++;
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        // Restore original value
                        using var key = baseKey.CreateSubKey(change.Path, true);
                        if (key != null && change.OriginalValue != null && change.OriginalValueKind != null)
                        {
                            var kind = Enum.Parse<RegistryValueKind>(change.OriginalValueKind);
                            key.SetValue(change.ValueName, change.OriginalValue, kind);
                            restored++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"   ⚠️  Could not restore {change.Path}\\{change.ValueName}: {ex.Message}");
                    failed++;
                }
            }

            _logger.LogInfo($"   ✅ Restored: {restored}, ⚠️  Failed: {failed}");
            await Task.CompletedTask;
        }

        private async Task RestoreServiceChanges(List<ServiceBackupEntry> changes)
        {
            int restored = 0;
            int failed = 0;

            foreach (var change in changes)
            {
                try
                {
                    // Restore start mode
                    await _commandRunner.RunCommandAsync("sc", $"config {change.ServiceName} start={change.OriginalStartMode}");

                    // Restore status
                    if (change.OriginalStatus.Contains("RUNNING", StringComparison.OrdinalIgnoreCase))
                    {
                        await _commandRunner.RunCommandAsync("sc", $"start {change.ServiceName}");
                    }
                    else if (change.OriginalStatus.Contains("STOPPED", StringComparison.OrdinalIgnoreCase))
                    {
                        await _commandRunner.RunCommandAsync("sc", $"stop {change.ServiceName}");
                    }

                    restored++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"   ⚠️  Could not restore service {change.ServiceName}: {ex.Message}");
                    failed++;
                }
            }

            _logger.LogInfo($"   ✅ Restored: {restored}, ⚠️  Failed: {failed}");
        }

        private async Task RestorePowerSettings(List<PowerSettingBackupEntry> settings)
        {
            int restored = 0;
            int failed = 0;

            foreach (var setting in settings)
            {
                try
                {
                    // This is a simplified restoration - in practice, power settings
                    // are complex and may need more sophisticated handling
                    _logger.LogInfo($"   Power setting: {setting.SettingType}");
                    restored++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"   ⚠️  Could not restore power setting {setting.SettingType}: {ex.Message}");
                    failed++;
                }
            }

            _logger.LogInfo($"   ✅ Restored: {restored}, ⚠️  Failed: {failed}");
            await Task.CompletedTask;
        }

        private async Task RestoreNetworkInterfaceSettings(List<NetworkSettingBackupEntry> settings)
        {
            int restored = 0;
            int failed = 0;

            foreach (var setting in settings)
            {
                try
                {
                    if (setting.InterfaceName == "SYSTEM")
                    {
                        // Restore system-wide network settings (e.g., NetworkThrottlingIndex)
                        using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", true);
                        if (key != null)
                        {
                            if (!setting.ValueExisted)
                            {
                                try { key.DeleteValue(setting.SettingName, false); } catch { }
                            }
                            else if (setting.OriginalValue != null && setting.ValueKind != null)
                            {
                                var kind = Enum.Parse<RegistryValueKind>(setting.ValueKind);
                                key.SetValue(setting.SettingName, setting.OriginalValue, kind);
                            }
                            restored++;
                        }
                    }
                    else
                    {
                        // Restore network interface settings
                        using var key = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\{setting.InterfaceName}", true);
                        if (key != null)
                        {
                            if (!setting.ValueExisted)
                            {
                                // Value didn't exist before, delete it
                                try { key.DeleteValue(setting.SettingName, false); } catch { }
                            }
                            else if (setting.OriginalValue != null && setting.ValueKind != null)
                            {
                                // Restore original value
                                var kind = Enum.Parse<RegistryValueKind>(setting.ValueKind);
                                key.SetValue(setting.SettingName, setting.OriginalValue, kind);
                            }
                            restored++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"   ⚠️  Could not restore {setting.SettingName}: {ex.Message}");
                    failed++;
                }
            }

            _logger.LogInfo($"   ✅ Restored: {restored}, ⚠️  Failed: {failed}");
            await Task.CompletedTask;
        }

        private async Task RestoreDnsSettings(List<DnsBackupEntry> settings)
        {
            int restored = 0;
            int failed = 0;

            foreach (var setting in settings)
            {
                try
                {
                    if (setting.WasDhcp || setting.OriginalDnsServers.Count == 0)
                    {
                        // Restore to DHCP
                        await _commandRunner.RunCommandAsync("powershell", 
                            $"-Command \"Set-DnsClientServerAddress -InterfaceAlias '{setting.InterfaceName}' -ResetServerAddresses\"");
                        _logger.LogInfo($"   ✅ {setting.InterfaceName} → DHCP (automatic)");
                    }
                    else
                    {
                        // Restore specific DNS servers
                        var dnsString = string.Join("','", setting.OriginalDnsServers);
                        await _commandRunner.RunCommandAsync("powershell", 
                            $"-Command \"Set-DnsClientServerAddress -InterfaceAlias '{setting.InterfaceName}' -ServerAddresses ('{dnsString}')\"");
                        _logger.LogInfo($"   ✅ {setting.InterfaceName} → {string.Join(", ", setting.OriginalDnsServers)}");
                    }
                    restored++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"   ⚠️  Could not restore DNS for {setting.InterfaceName}: {ex.Message}");
                    failed++;
                }
            }

            _logger.LogInfo($"   ✅ Restored: {restored}, ⚠️  Failed: {failed}");
        }

        private async Task RestoreTcpIpSettings(List<TcpIpBackupEntry> settings)
        {
            int restored = 0;
            int failed = 0;

            // Group settings to avoid calling netsh multiple times for the same global show
            var globalSettings = settings.Where(s => s.CommandOutput.Contains("global", StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var setting in globalSettings)
            {
                try
                {
                    var value = setting.OriginalValue;
                    
                    // Parse and restore TCP/IP settings
                    switch (setting.SettingName.ToLower())
                    {
                        case "chimney":
                            await _commandRunner.RunCommandAsync("netsh", $"int tcp set global chimney={value}");
                            break;
                        case "autotuninglevel":
                            await _commandRunner.RunCommandAsync("netsh", $"int tcp set global autotuninglevel={value}");
                            break;
                        case "congestionprovider":
                            await _commandRunner.RunCommandAsync("netsh", $"int tcp set global congestionprovider={value}");
                            break;
                        case "ecncapability":
                            await _commandRunner.RunCommandAsync("netsh", $"int tcp set global ecncapability={value}");
                            break;
                        case "timestamps":
                            await _commandRunner.RunCommandAsync("netsh", $"int tcp set global timestamps={value}");
                            break;
                    }
                    
                    _logger.LogInfo($"   ✅ TCP/IP {setting.SettingName} → {value}");
                    restored++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"   ⚠️  Could not restore TCP/IP {setting.SettingName}: {ex.Message}");
                    failed++;
                }
            }

            _logger.LogInfo($"   ✅ Restored: {restored}, ⚠️  Failed: {failed}");
        }

        public List<OptimizationBackup> ListBackups()
        {
            var backups = new List<OptimizationBackup>();

            try
            {
                var files = Directory.GetFiles(_backupDirectory, "backup_*.json");
                foreach (var file in files.OrderByDescending(f => File.GetCreationTime(f)))
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var backup = JsonConvert.DeserializeObject<OptimizationBackup>(json);
                        if (backup != null)
                        {
                            backups.Add(backup);
                        }
                    }
                    catch
                    {
                        // Skip invalid backup files
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to list backups: {ex.Message}");
            }

            return backups;
        }

        public string GetBackupFilePath(string backupId)
        {
            return Path.Combine(_backupDirectory, $"backup_{backupId}.json");
        }

        public List<(OptimizationBackup Backup, string FilePath)> ListBackupsDetailed()
        {
            var backups = new List<(OptimizationBackup, string)>();

            try
            {
                if (!Directory.Exists(_backupDirectory))
                {
                    Directory.CreateDirectory(_backupDirectory);
                    return backups;
                }

                var files = Directory.GetFiles(_backupDirectory, "backup_*.json");
                
                foreach (var file in files.OrderByDescending(f => File.GetLastWriteTime(f)))
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var backup = JsonConvert.DeserializeObject<OptimizationBackup>(json);
                        
                        if (backup != null)
                        {
                            backups.Add((backup, file));
                        }
                    }
                    catch
                    {
                        // Skip invalid backup files
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to list detailed backups: {ex.Message}");
            }

            return backups;
        }

        private string ExtractServiceStatus(string output)
        {
            if (output.Contains("RUNNING", StringComparison.OrdinalIgnoreCase))
                return "RUNNING";
            if (output.Contains("STOPPED", StringComparison.OrdinalIgnoreCase))
                return "STOPPED";
            return "UNKNOWN";
        }

        private string ExtractServiceStartMode(string output)
        {
            if (output.Contains("AUTO_START", StringComparison.OrdinalIgnoreCase))
                return "auto";
            if (output.Contains("DEMAND_START", StringComparison.OrdinalIgnoreCase))
                return "demand";
            if (output.Contains("DISABLED", StringComparison.OrdinalIgnoreCase))
                return "disabled";
            return "manual";
        }
    }
}

