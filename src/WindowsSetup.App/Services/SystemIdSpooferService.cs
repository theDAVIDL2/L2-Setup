using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;
using WindowsSetup.App.Models;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App.Services
{
    public class SystemIdSpooferService
    {
        private readonly Logger _logger;
        private readonly string _backupDirectory;
        private readonly CommandRunner _commandRunner;
        private readonly string _torDirectory;
        private Process? _torProcess;

        public SystemIdSpooferService(Logger logger)
        {
            _logger = logger;
            _backupDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "L2Setup", "SystemIdBackups");
            Directory.CreateDirectory(_backupDirectory);
            _commandRunner = new CommandRunner();
            _torDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "L2Setup", "Tor");
        }

        #region Backup and Restore

        public async Task<SystemIdBackup?> CreateFullBackup()
        {
            try
            {
                _logger.LogInfo("Creating full system identity backup...");
                
                var backup = new SystemIdBackup
                {
                    Description = "Full System Identity Backup"
                };

                // Backup registry entries
                await BackupRegistryEntries(backup);
                
                // Backup MAC addresses
                await BackupMacAddresses(backup);
                
                // Backup volume serials
                await BackupVolumeSerials(backup);
                
                // Backup network settings
                await BackupNetworkSettings(backup);

                // Save backup to file
                var backupPath = Path.Combine(_backupDirectory, $"SystemIdBackup_{backup.BackupId}.json");
                var json = JsonConvert.SerializeObject(backup, Formatting.Indented);
                await File.WriteAllTextAsync(backupPath, json);
                
                _logger.LogSuccess($"✅ Backup created: {Path.GetFileName(backupPath)}");
                return backup;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error creating backup: {ex.Message}");
                return null;
            }
        }

        private async Task BackupRegistryEntries(SystemIdBackup backup)
        {
            var registryPaths = new Dictionary<RegistryHive, Dictionary<string, string[]>>
            {
                {
                    RegistryHive.LocalMachine, new Dictionary<string, string[]>
                    {
                        { @"SOFTWARE\Microsoft\Cryptography", new[] { "MachineGuid" } },
                        { @"SOFTWARE\Microsoft\SQMClient", new[] { "MachineId" } },
                        { @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", new[] { "ProductId" } },
                        { @"SYSTEM\CurrentControlSet\Control\IDConfigDB\Hardware Profiles\0001", new[] { "HwProfileGuid" } }
                    }
                }
            };

            foreach (var hive in registryPaths)
            {
                foreach (var keyPath in hive.Value)
                {
                    try
                    {
                        using var key = GetRegistryKey(hive.Key, keyPath.Key, false);
                        if (key != null)
                        {
                            foreach (var valueName in keyPath.Value)
                            {
                                var value = key.GetValue(valueName);
                                if (value != null)
                                {
                                    backup.RegistryBackups.Add(new RegistryBackupEntry
                                    {
                                        Hive = hive.Key.ToString(),
                                        Path = keyPath.Key,
                                        ValueName = valueName,
                                        OriginalValue = value,
                                        OriginalValueKind = key.GetValueKind(valueName).ToString(),
                                        ValueExisted = true,
                                        KeyExisted = true
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"⚠️ Could not backup {keyPath.Key}: {ex.Message}");
                    }
                }
            }

            await Task.CompletedTask;
        }

        private async Task BackupMacAddresses(SystemIdBackup backup)
        {
            var adapters = await GetNetworkAdapters();
            foreach (var adapter in adapters)
            {
                backup.MacAddresses[adapter.Name] = adapter.CurrentMac;
            }
        }

        private async Task BackupVolumeSerials(SystemIdBackup backup)
        {
            var volumes = await GetVolumeInfo();
            foreach (var volume in volumes)
            {
                backup.VolumeSerials[volume.DriveLetter] = volume.CurrentSerial;
            }
        }

        private async Task BackupNetworkSettings(SystemIdBackup backup)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings");
                if (key != null)
                {
                    backup.NetworkSettings = new NetworkBackup
                    {
                        ProxyEnabled = key.GetValue("ProxyEnable") as int? == 1,
                        ProxyServer = key.GetValue("ProxyServer") as string ?? "",
                        ProxyBypass = key.GetValue("ProxyOverride") as string ?? ""
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"⚠️ Could not backup network settings: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        public async Task<bool> RestoreBackup(SystemIdBackup backup)
        {
            try
            {
                _logger.LogInfo($"Restoring backup from {backup.BackupDate:yyyy-MM-dd HH:mm:ss}...");

                // Restore registry entries
                await RestoreRegistryEntries(backup);
                
                // Restore MAC addresses
                await RestoreMacAddresses(backup);
                
                // Restore network settings
                await RestoreNetworkSettings(backup);

                _logger.LogSuccess("✅ Backup restored successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error restoring backup: {ex.Message}");
                return false;
            }
        }

        private async Task RestoreRegistryEntries(SystemIdBackup backup)
        {
            foreach (var entry in backup.RegistryBackups)
            {
                try
                {
                    var hive = Enum.Parse<RegistryHive>(entry.Hive);
                    using var key = GetRegistryKey(hive, entry.Path, true);
                    if (key != null && entry.OriginalValue != null)
                    {
                        var valueKind = Enum.Parse<RegistryValueKind>(entry.OriginalValueKind ?? "String");
                        key.SetValue(entry.ValueName, entry.OriginalValue, valueKind);
                        _logger.LogInfo($"   Restored: {entry.Path}\\{entry.ValueName}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"⚠️ Could not restore {entry.Path}: {ex.Message}");
                }
            }

            await Task.CompletedTask;
        }

        private async Task RestoreMacAddresses(SystemIdBackup backup)
        {
            foreach (var mac in backup.MacAddresses)
            {
                try
                {
                    await SetMacAddress(mac.Key, mac.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"⚠️ Could not restore MAC for {mac.Key}: {ex.Message}");
                }
            }
        }

        private async Task RestoreNetworkSettings(SystemIdBackup backup)
        {
            if (backup.NetworkSettings != null)
            {
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);
                    if (key != null)
                    {
                        key.SetValue("ProxyEnable", backup.NetworkSettings.ProxyEnabled ? 1 : 0);
                        key.SetValue("ProxyServer", backup.NetworkSettings.ProxyServer);
                        key.SetValue("ProxyOverride", backup.NetworkSettings.ProxyBypass);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"⚠️ Could not restore network settings: {ex.Message}");
                }
            }

            await Task.CompletedTask;
        }

        public List<SystemIdBackup> ListBackups()
        {
            var backups = new List<SystemIdBackup>();
            
            try
            {
                var files = Directory.GetFiles(_backupDirectory, "SystemIdBackup_*.json");
                foreach (var file in files.OrderByDescending(f => File.GetCreationTime(f)))
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var backup = JsonConvert.DeserializeObject<SystemIdBackup>(json);
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
                _logger.LogError($"❌ Error listing backups: {ex.Message}");
            }

            return backups;
        }

        #endregion

        #region Machine GUID Spoofing

        public async Task<bool> SpoofMachineGuids()
        {
            try
            {
                _logger.LogInfo("🔧 Spoofing Windows Machine GUIDs...");

                var guids = new Dictionary<string, string>
                {
                    { @"SOFTWARE\Microsoft\Cryptography", "MachineGuid" },
                    { @"SOFTWARE\Microsoft\SQMClient", "MachineId" },
                    { @"SYSTEM\CurrentControlSet\Control\IDConfigDB\Hardware Profiles\0001", "HwProfileGuid" }
                };

                foreach (var entry in guids)
                {
                    try
                    {
                        using var key = Registry.LocalMachine.OpenSubKey(entry.Key, true);
                        if (key != null)
                        {
                            var newGuid = entry.Key.Contains("SQM") 
                                ? $"{{{Guid.NewGuid().ToString().ToUpper()}}}"
                                : Guid.NewGuid().ToString();
                            
                            key.SetValue(entry.Value, newGuid);
                            _logger.LogInfo($"   ✅ {entry.Key}\\{entry.Value} = {newGuid}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"   ⚠️ Could not spoof {entry.Key}: {ex.Message}");
                    }
                }

                _logger.LogSuccess("✅ Machine GUIDs spoofed successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error spoofing Machine GUIDs: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Hardware ID Spoofing

        public async Task<bool> SpoofHardwareIds()
        {
            try
            {
                _logger.LogInfo("🔧 Spoofing Hardware IDs...");
                _logger.LogWarning("⚠️ Some hardware IDs are read-only or firmware-based");

                bool anySuccess = false;

                // Spoof disk serial numbers (registry-based)
                anySuccess |= await SpoofDiskSerials();

                // Spoof motherboard UUID (registry-based where possible)
                anySuccess |= await SpoofMotherboardUuid();

                // Spoof processor ID (limited, mostly read-only)
                anySuccess |= await SpoofProcessorId();

                if (anySuccess)
                {
                    _logger.LogSuccess("✅ Hardware IDs spoofed (some changes may require reboot)");
                }
                else
                {
                    _logger.LogWarning("⚠️ Hardware ID spoofing had limited success");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error spoofing Hardware IDs: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> SpoofDiskSerials()
        {
            try
            {
                _logger.LogInfo("   Spoofing disk serial numbers...");
                
                // Note: Disk serials are mostly hardware-based
                // We can modify registry entries that some software checks
                var diskKey = @"SYSTEM\CurrentControlSet\Services\Disk\Enum";
                
                using var key = Registry.LocalMachine.OpenSubKey(diskKey, true);
                if (key != null)
                {
                    var count = key.GetValue("Count") as int?;
                    if (count.HasValue)
                    {
                        for (int i = 0; i < count.Value; i++)
                        {
                            var valueName = i.ToString();
                            var currentValue = key.GetValue(valueName) as string;
                            
                            if (!string.IsNullOrEmpty(currentValue))
                            {
                                // Generate a new disk identifier
                                var newSerial = GenerateRandomHexString(16);
                                var modifiedValue = ModifyDiskEnumString(currentValue, newSerial);
                                
                                key.SetValue(valueName, modifiedValue);
                                _logger.LogInfo($"      Modified disk {i}: {newSerial}");
                            }
                        }
                        
                        _logger.LogSuccess("   ✅ Disk serials modified");
                        return true;
                    }
                }
                
                _logger.LogWarning("   ⚠️ Could not access disk serial registry keys");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"   ⚠️ Disk serial spoofing failed: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> SpoofMotherboardUuid()
        {
            try
            {
                _logger.LogInfo("   Spoofing motherboard UUID...");
                
                // Try to modify SMBIOS UUID in registry (where possible)
                // Note: Real SMBIOS is in firmware, but some software checks registry
                var newUuid = Guid.NewGuid().ToString();
                
                var paths = new[]
                {
                    @"SYSTEM\CurrentControlSet\Control\SystemInformation",
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate"
                };
                
                bool anySuccess = false;
                foreach (var path in paths)
                {
                    try
                    {
                        using var key = Registry.LocalMachine.OpenSubKey(path, true);
                        if (key != null)
                        {
                            key.SetValue("ComputerHardwareId", newUuid);
                            _logger.LogInfo($"      Modified UUID in: {path}");
                            anySuccess = true;
                        }
                    }
                    catch
                    {
                        // Skip keys we can't modify
                    }
                }
                
                if (anySuccess)
                {
                    _logger.LogSuccess($"   ✅ Motherboard UUID: {newUuid}");
                    return true;
                }
                else
                {
                    _logger.LogWarning("   ⚠️ Motherboard UUID spoofing not available");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"   ⚠️ Motherboard UUID spoofing failed: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> SpoofProcessorId()
        {
            try
            {
                _logger.LogInfo("   Checking processor ID...");
                _logger.LogWarning("   ⚠️ Processor ID is hardware-based and cannot be changed");
                
                // Processor IDs are burned into the CPU and cannot be modified
                // We can only note this limitation
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"   ⚠️ Processor ID check failed: {ex.Message}");
                return false;
            }
        }

        private string ModifyDiskEnumString(string original, string newSerial)
        {
            // Disk enum strings are in format: SCSI\Disk&...\Serial
            // We'll try to replace any serial-like portion
            var parts = original.Split('\\');
            if (parts.Length >= 2)
            {
                // Replace the last part (usually contains serial)
                parts[parts.Length - 1] = newSerial;
                return string.Join("\\", parts);
            }
            return original;
        }

        private string GenerateRandomHexString(int length)
        {
            var random = new Random();
            var bytes = new byte[length / 2];
            random.NextBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        #endregion

        #region MAC Address Spoofing

        public async Task<List<NetworkAdapterInfo>> GetNetworkAdapters()
        {
            var adapters = new List<NetworkAdapterInfo>();

            try
            {
                var query = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionStatus = 2");
                using var searcher = new ManagementObjectSearcher(query);
                
                foreach (ManagementObject adapter in searcher.Get())
                {
                    try
                    {
                        var name = adapter["NetConnectionID"]?.ToString();
                        var mac = adapter["MACAddress"]?.ToString();
                        var guid = adapter["GUID"]?.ToString();

                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(mac))
                        {
                            adapters.Add(new NetworkAdapterInfo
                            {
                                Name = name,
                                Description = adapter["Description"]?.ToString() ?? "",
                                CurrentMac = mac,
                                OriginalMac = mac,
                                IsEnabled = true,
                                AdapterId = guid ?? "",
                                IsSpoofed = false
                            });
                        }
                    }
                    catch
                    {
                        // Skip adapters we can't read
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error enumerating network adapters: {ex.Message}");
            }

            return adapters;
        }

        public async Task<bool> SpoofMacAddress(string adapterName, string? customMac = null)
        {
            try
            {
                var mac = customMac ?? GenerateRandomMac();
                return await SetMacAddress(adapterName, mac);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error spoofing MAC for {adapterName}: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> SetMacAddress(string adapterName, string macAddress)
        {
            try
            {
                // Find adapter registry key
                var adapters = await GetNetworkAdapters();
                var adapter = adapters.FirstOrDefault(a => a.Name == adapterName);
                if (adapter == null)
                {
                    _logger.LogWarning($"⚠️ Adapter not found: {adapterName}");
                    return false;
                }

                // Set MAC in registry
                var networkCardsKey = @"SYSTEM\CurrentControlSet\Control\Class\{4D36E972-E325-11CE-BFC1-08002BE10318}";
                using var baseKey = Registry.LocalMachine.OpenSubKey(networkCardsKey, false);
                
                if (baseKey != null)
                {
                    foreach (var subKeyName in baseKey.GetSubKeyNames())
                    {
                        using var subKey = baseKey.OpenSubKey(subKeyName, true);
                        if (subKey != null)
                        {
                            var driverDesc = subKey.GetValue("DriverDesc") as string;
                            if (driverDesc == adapter.Description)
                            {
                                // Remove dashes from MAC
                                var macNoSep = macAddress.Replace("-", "").Replace(":", "");
                                subKey.SetValue("NetworkAddress", macNoSep, RegistryValueKind.String);
                                _logger.LogInfo($"   Set NetworkAddress to {macNoSep}");
                                
                                // Restart adapter
                                await RestartNetworkAdapter(adapterName);
                                
                                _logger.LogSuccess($"✅ MAC address changed for {adapterName}");
                                return true;
                            }
                        }
                    }
                }

                _logger.LogWarning($"⚠️ Could not find registry key for {adapterName}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error setting MAC address: {ex.Message}");
                return false;
            }
        }

        private async Task RestartNetworkAdapter(string adapterName)
        {
            try
            {
                _logger.LogInfo($"   Restarting adapter: {adapterName}");
                
                // Disable
                var disableResult = await _commandRunner.RunCommandAsync("netsh", $"interface set interface \"{adapterName}\" disable");
                await Task.Delay(1000);
                
                // Enable
                var enableResult = await _commandRunner.RunCommandAsync("netsh", $"interface set interface \"{adapterName}\" enable");
                await Task.Delay(2000);
                
                _logger.LogInfo($"   Adapter restarted");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"⚠️ Could not restart adapter: {ex.Message}");
            }
        }

        private string GenerateRandomMac()
        {
            var random = new Random();
            var bytes = new byte[6];
            random.NextBytes(bytes);
            
            // Set locally administered bit (bit 1 of first byte)
            bytes[0] = (byte)((bytes[0] | 0x02) & 0xFE);
            
            return string.Join("-", bytes.Select(b => b.ToString("X2")));
        }

        #endregion

        #region Volume Serial Spoofing

        public async Task<List<VolumeInfo>> GetVolumeInfo()
        {
            var volumes = new List<VolumeInfo>();

            try
            {
                var drives = DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Fixed);
                
                foreach (var drive in drives)
                {
                    try
                    {
                        var serial = GetVolumeSerial(drive.Name);
                        volumes.Add(new VolumeInfo
                        {
                            DriveLetter = drive.Name.TrimEnd('\\'),
                            Label = drive.VolumeLabel,
                            CurrentSerial = serial,
                            OriginalSerial = serial,
                            TotalSize = drive.TotalSize,
                            FileSystem = drive.DriveFormat,
                            IsSpoofed = false
                        });
                    }
                    catch
                    {
                        // Skip drives we can't read
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error getting volume info: {ex.Message}");
            }

            return volumes;
        }

        private string GetVolumeSerial(string driveLetter)
        {
            try
            {
                var query = $"SELECT VolumeSerialNumber FROM Win32_LogicalDisk WHERE DeviceID = '{driveLetter.TrimEnd('\\')}'";
                using var searcher = new ManagementObjectSearcher(query);
                
                foreach (ManagementObject disk in searcher.Get())
                {
                    return disk["VolumeSerialNumber"]?.ToString() ?? "Unknown";
                }
            }
            catch
            {
                return "Unknown";
            }

            return "Unknown";
        }

        public async Task<bool> SpoofVolumeSerial(string driveLetter)
        {
            try
            {
                _logger.LogWarning("⚠️ Volume serial spoofing is experimental and may cause issues!");
                _logger.LogInfo($"Spoofing volume serial for {driveLetter}...");
                
                // Generate random serial (8 hex digits)
                var random = new Random();
                var newSerial = random.Next(0x10000000, 0x7FFFFFFF).ToString("X8");
                
                // Note: This requires external tool or low-level disk access
                _logger.LogWarning("⚠️ Volume serial spoofing requires VolumeID.exe or direct disk access");
                _logger.LogInfo($"   New serial would be: {newSerial}");
                
                return false; // Not implemented for safety
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error spoofing volume serial: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Tor Integration

        public bool IsTorInstalled()
        {
            var torExePath = Path.Combine(_torDirectory, "Tor", "tor.exe");
            return File.Exists(torExePath);
        }

        public bool IsTorRunning()
        {
            return _torProcess != null && !_torProcess.HasExited;
        }

        public async Task<bool> DownloadTor()
        {
            try
            {
                _logger.LogInfo("🧅 Downloading Tor Expert Bundle...");
                
                // Tor download URL (Windows Expert Bundle)
                var torUrl = "https://dist.torproject.org/torbrowser/13.0.1/tor-expert-bundle-windows-x86_64-13.0.1.tar.gz";
                var torArchive = Path.Combine(_torDirectory, "tor-expert-bundle.tar.gz");
                
                Directory.CreateDirectory(_torDirectory);
                
                // Download Tor
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(10);
                    _logger.LogInfo("   Downloading from torproject.org...");
                    
                    var response = await client.GetAsync(torUrl);
                    response.EnsureSuccessStatusCode();
                    
                    var totalBytes = response.Content.Headers.ContentLength ?? 0;
                    using (var fileStream = File.Create(torArchive))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }
                    
                    _logger.LogSuccess($"   ✅ Downloaded ({totalBytes / 1024 / 1024} MB)");
                }
                
                // Extract Tor
                _logger.LogInfo("   Extracting Tor...");
                await Task.Run(() =>
                {
                    using var stream = File.OpenRead(torArchive);
                    using var reader = SharpCompress.Readers.ReaderFactory.Open(stream);
                    
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            var entryPath = Path.Combine(_torDirectory, reader.Entry.Key);
                            var entryDir = Path.GetDirectoryName(entryPath);
                            
                            if (!string.IsNullOrEmpty(entryDir))
                            {
                                Directory.CreateDirectory(entryDir);
                            }
                            
                            using var entryStream = reader.OpenEntryStream();
                            using var outputStream = File.Create(entryPath);
                            entryStream.CopyTo(outputStream);
                        }
                    }
                });
                
                // Clean up archive
                File.Delete(torArchive);
                
                // Create torrc config
                await CreateTorConfig();
                
                _logger.LogSuccess("✅ Tor installed successfully!");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error downloading Tor: {ex.Message}");
                return false;
            }
        }

        private async Task CreateTorConfig()
        {
            try
            {
                var torrcPath = Path.Combine(_torDirectory, "Tor", "torrc");
                var config = @"# Tor configuration for L2 Setup
SocksPort 9050
ControlPort 9051
DataDirectory ./Data
Log notice stdout
";
                await File.WriteAllTextAsync(torrcPath, config);
                _logger.LogInfo("   Tor configuration created");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"   ⚠️ Could not create torrc: {ex.Message}");
            }
        }

        public async Task<bool> StartTor()
        {
            try
            {
                if (IsTorRunning())
                {
                    _logger.LogWarning("Tor is already running");
                    return true;
                }

                if (!IsTorInstalled())
                {
                    _logger.LogError("❌ Tor is not installed. Please download it first.");
                    return false;
                }

                _logger.LogInfo("🧅 Starting Tor...");
                
                var torExePath = Path.Combine(_torDirectory, "Tor", "tor.exe");
                var torrcPath = Path.Combine(_torDirectory, "Tor", "torrc");
                
                _torProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = torExePath,
                        Arguments = $"-f \"{torrcPath}\"",
                        WorkingDirectory = Path.Combine(_torDirectory, "Tor"),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                _torProcess.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        _logger.LogInfo($"   TOR: {e.Data}");
                    }
                };

                _torProcess.Start();
                _torProcess.BeginOutputReadLine();
                
                // Wait a bit for Tor to start
                await Task.Delay(3000);
                
                if (!_torProcess.HasExited)
                {
                    // Enable Tor proxy
                    await EnableTorProxy();
                    
                    _logger.LogSuccess("✅ Tor started successfully!");
                    _logger.LogInfo("   SOCKS5 proxy running on 127.0.0.1:9050");
                    return true;
                }
                else
                {
                    _logger.LogError("❌ Tor process exited unexpectedly");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error starting Tor: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> StopTor()
        {
            try
            {
                if (!IsTorRunning())
                {
                    _logger.LogWarning("Tor is not running");
                    return true;
                }

                _logger.LogInfo("Stopping Tor...");
                
                // Disable proxy first
                await DisableTorProxy();
                
                _torProcess?.Kill();
                _torProcess?.WaitForExit(5000);
                _torProcess?.Dispose();
                _torProcess = null;
                
                _logger.LogSuccess("✅ Tor stopped");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error stopping Tor: {ex.Message}");
                return false;
            }
        }

        private async Task EnableTorProxy()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);
                if (key != null)
                {
                    key.SetValue("ProxyEnable", 1);
                    key.SetValue("ProxyServer", "socks=127.0.0.1:9050");
                    _logger.LogInfo("   ✅ System proxy configured for Tor");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"   ⚠️ Could not configure proxy: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        private async Task DisableTorProxy()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);
                if (key != null)
                {
                    key.SetValue("ProxyEnable", 0);
                    key.SetValue("ProxyServer", "");
                    _logger.LogInfo("   ✅ System proxy disabled");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"   ⚠️ Could not disable proxy: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        public async Task<string> CheckCurrentIp()
        {
            try
            {
                using var client = new System.Net.Http.HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                
                var response = await client.GetStringAsync("https://api.ipify.org");
                return response.Trim();
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error checking IP: {ex.Message}");
                return "Unknown";
            }
        }

        #endregion

        #region Helper Methods

        private RegistryKey? GetRegistryKey(RegistryHive hive, string path, bool writable)
        {
            try
            {
                var baseKey = hive switch
                {
                    RegistryHive.LocalMachine => Registry.LocalMachine,
                    RegistryHive.CurrentUser => Registry.CurrentUser,
                    _ => null
                };

                return baseKey?.OpenSubKey(path, writable);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}

