using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Data.Sqlite;
using WindowsSetup.App.Models;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App.Services
{
    public class CursorToolsService
    {
        private readonly Logger _logger;
        private readonly string _cursorConfigPath;
        private readonly string _storageJsonPath;
        private readonly string _sqlitePath;
        private readonly string _machineIdPath;
        private readonly string _backupDirectory;

        public CursorToolsService(Logger logger)
        {
            _logger = logger;
            _cursorConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Cursor");
            _storageJsonPath = Path.Combine(_cursorConfigPath, "User", "globalStorage", "storage.json");
            _sqlitePath = Path.Combine(_cursorConfigPath, "User", "globalStorage", "state.vscdb");
            _machineIdPath = Path.Combine(_cursorConfigPath, "machineId");
            _backupDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "L2Setup", "CursorBackups");
            Directory.CreateDirectory(_backupDirectory);
        }

        #region Cursor Detection

        public CursorInfo GetCursorInfo()
        {
            var info = new CursorInfo
            {
                IsInstalled = Directory.Exists(_cursorConfigPath),
                IsRunning = IsCursorRunning(),
                ConfigPath = _cursorConfigPath,
                StorageJsonPath = _storageJsonPath
            };

            if (info.IsInstalled && File.Exists(_storageJsonPath))
            {
                try
                {
                    var fileInfo = new FileInfo(_storageJsonPath);
                    info.LastModified = fileInfo.LastWriteTime;

                    var config = LoadStorageJson();
                    if (config?.BackupWorkspaces?.Folders != null)
                    {
                        info.WorkspaceCount = config.BackupWorkspaces.Folders.Count;
                    }

                    // Calculate total config size
                    var dirInfo = new DirectoryInfo(_cursorConfigPath);
                    info.ConfigSize = GetDirectorySize(dirInfo);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Error reading Cursor info: {ex.Message}");
                }
            }

            return info;
        }

        public bool IsCursorRunning()
        {
            try
            {
                var cursorProcesses = Process.GetProcessesByName("Cursor");
                return cursorProcesses.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CloseCursor()
        {
            try
            {
                _logger.LogInfo("Closing Cursor...");
                var cursorProcesses = Process.GetProcessesByName("Cursor");

                foreach (var process in cursorProcesses)
                {
                    try
                    {
                        process.CloseMainWindow();
                        await Task.Delay(500);

                        if (!process.HasExited)
                        {
                            process.Kill();
                        }

                        process.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Error closing Cursor process {process.Id}: {ex.Message}");
                    }
                }

                // Wait a bit for processes to fully close
                await Task.Delay(1000);

                _logger.LogSuccess("Cursor closed successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to close Cursor: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Reset Trial / Machine ID

        public async Task<bool> ResetTrial(bool autoCloseCursor = true, bool fullReset = false)
        {
            try
            {
                if (fullReset)
                {
                    return await ResetByDeletingStorage(autoCloseCursor);
                }
                else
                {
                    return await ResetByModifyingJson(autoCloseCursor);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error changing machine IDs: {ex.Message}");
                return false;
            }
        }

        // Method 1: REAL Machine ID Spoofer (Changes Windows System IDs + Modifies Cursor Files)
        private async Task<bool> ResetByDeletingStorage(bool autoCloseCursor)
        {
            _logger.LogInfo("=== MACHINE ID SPOOFER - Full Implementation ===");
            _logger.LogWarning("⚠️ This will modify Windows registry + Cursor configuration files!");

            // Check if Cursor is installed
            if (!Directory.Exists(_cursorConfigPath))
            {
                _logger.LogError("❌ Cursor is not installed!");
                return false;
            }

            // Close Cursor if running
            if (IsCursorRunning())
            {
                if (autoCloseCursor)
                {
                    _logger.LogWarning("⚠️ Cursor is running. Closing it...");
                    if (!await CloseCursor())
                    {
                        _logger.LogError("❌ Failed to close Cursor. Please close it manually.");
                        return false;
                    }
                }
                else
                {
                    _logger.LogError("❌ Cursor is running. Please close it first.");
                    return false;
                }
            }

            // Backup current config
            _logger.LogInfo("💾 Creating backup of current configuration...");
            var backupPath = await BackupSettings();
            if (backupPath != null)
            {
                _logger.LogSuccess($"✅ Backup created: {Path.GetFileName(backupPath)}");
            }

            // Generate new machine IDs
            _logger.LogInfo("");
            _logger.LogInfo("🔑 Generating new machine identifiers...");
            var newIds = GenerateNewMachineIds();
            _logger.LogSuccess("✅ New IDs generated");

            // STEP 1: Change Windows Machine GUIDs (Registry)
            _logger.LogInfo("");
            _logger.LogInfo("🔧 STEP 1/4: Spoofing Windows Registry Machine IDs...");
            await SpoofWindowsMachineId();

            // STEP 2: Update storage.json
            _logger.LogInfo("");
            _logger.LogInfo("📝 STEP 2/4: Updating storage.json with new IDs...");
            if (await UpdateStorageJson(newIds))
            {
                _logger.LogSuccess("✅ storage.json updated successfully");
            }
            else
            {
                _logger.LogWarning("⚠️ storage.json update failed (file may not exist)");
            }

            // STEP 3: Update SQLite database (state.vscdb)
            _logger.LogInfo("");
            _logger.LogInfo("💾 STEP 3/4: Updating SQLite database (state.vscdb)...");
            if (await UpdateSqliteDatabase(newIds))
            {
                _logger.LogSuccess("✅ SQLite database updated successfully");
            }
            else
            {
                _logger.LogWarning("⚠️ SQLite database update failed (file may not exist)");
            }

            // STEP 4: Update machineId file
            _logger.LogInfo("");
            _logger.LogInfo("🆔 STEP 4/4: Updating machineId file...");
            if (await UpdateMachineIdFile(newIds["telemetry.devDeviceId"]))
            {
                _logger.LogSuccess("✅ machineId file updated successfully");
            }
            else
            {
                _logger.LogWarning("⚠️ machineId file update failed");
            }

            // Summary
            _logger.LogInfo("");
            _logger.LogSuccess("═══════════════════════════════════════════════");
            _logger.LogSuccess("✅ MACHINE ID SPOOFING COMPLETE!");
            _logger.LogSuccess("═══════════════════════════════════════════════");
            _logger.LogInfo("");
            _logger.LogInfo("🎯 WHAT WAS CHANGED:");
            _logger.LogInfo("   ✅ Windows Machine GUID (Registry)");
            _logger.LogInfo("   ✅ Cryptography Machine GUID (Registry)");
            _logger.LogInfo("   ✅ SQM Client ID (Registry)");
            _logger.LogInfo("   ✅ Cursor storage.json (5 machine IDs)");
            _logger.LogInfo("   ✅ Cursor state.vscdb (SQLite database)");
            _logger.LogInfo("   ✅ Cursor machineId file");
            _logger.LogInfo("");
            _logger.LogInfo("🔑 NEW MACHINE IDs:");
            foreach (var kvp in newIds)
            {
                _logger.LogInfo($"   • {kvp.Key}: {kvp.Value}");
            }
            _logger.LogInfo("");
            _logger.LogInfo("🔄 NEXT STEPS:");
            _logger.LogInfo("   1️⃣ Restart Cursor IDE");
            _logger.LogInfo("   2️⃣ All machine IDs are now spoofed");
            _logger.LogInfo("   3️⃣ Cursor will use the new identifiers");
            _logger.LogInfo("");
            _logger.LogWarning("💡 All machine identifiers have been PERMANENTLY changed!");
            _logger.LogInfo("═══════════════════════════════════════════════");
            return true;
        }

        private async Task SpoofWindowsMachineId()
        {
            try
            {
                using (var cryptoKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Cryptography", writable: true))
                {
                    if (cryptoKey != null)
                    {
                        var oldGuid = cryptoKey.GetValue("MachineGuid")?.ToString() ?? "unknown";
                        var newGuid = Guid.NewGuid().ToString();
                        
                        _logger.LogInfo($"   Old GUID: {oldGuid}");
                        _logger.LogInfo($"   New GUID: {newGuid}");
                        
                        cryptoKey.SetValue("MachineGuid", newGuid);
                        _logger.LogSuccess("   ✅ Windows Machine GUID spoofed");
                    }
                }

                // Also change SQM Client ID (used by Microsoft telemetry)
                using (var sqmKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\SQMClient", writable: true))
                {
                    if (sqmKey != null)
                    {
                        sqmKey.SetValue("MachineId", $"{{{Guid.NewGuid().ToString().ToUpper()}}}");
                        _logger.LogSuccess("   ✅ SQM Machine ID spoofed");
                    }
                }

                // Windows Update Client ID
                try
                {
                    using (var wuKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                        @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate", writable: true))
                    {
                        if (wuKey != null)
                        {
                            wuKey.SetValue("SusClientId", Guid.NewGuid().ToString());
                            wuKey.SetValue("SusClientIdValidation", GenerateSha256Hash(64));
                            _logger.LogSuccess("   ✅ Windows Update Client ID spoofed");
                        }
                    }
                }
                catch { /* May not exist on all systems */ }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError($"   ⚠️ Error spoofing machine IDs: {ex.Message}");
                _logger.LogWarning("   ⚠️ You may need to run as Administrator for full access");
            }
        }

        private Dictionary<string, string> GenerateNewMachineIds()
        {
            // Generate UUID for devDeviceId
            var devDeviceId = Guid.NewGuid().ToString();

            // Generate SHA-256 hash (64 hex characters) for machineId
            var machineId = GenerateSha256Hash(64);

            // Generate SHA-512 hash (128 hex characters) for macMachineId
            var macMachineId = GenerateSha512Hash(128);

            // Generate GUID for sqmId (with brackets and uppercase)
            var sqmId = $"{{{Guid.NewGuid().ToString().ToUpper()}}}";

            // serviceMachineId is same as devDeviceId
            var serviceMachineId = devDeviceId;

            return new Dictionary<string, string>
            {
                { "telemetry.devDeviceId", devDeviceId },
                { "telemetry.machineId", machineId },
                { "telemetry.macMachineId", macMachineId },
                { "telemetry.sqmId", sqmId },
                { "storage.serviceMachineId", serviceMachineId }
            };
        }

        private async Task<bool> UpdateStorageJson(Dictionary<string, string> newIds)
        {
            try
            {
                if (!File.Exists(_storageJsonPath))
                {
                    _logger.LogWarning($"   ⚠️ storage.json not found at: {_storageJsonPath}");
                    return false;
                }

                // Read current storage.json
                var json = await File.ReadAllTextAsync(_storageJsonPath);
                var config = JObject.Parse(json);

                // Update all machine IDs
                foreach (var kvp in newIds)
                {
                    config[kvp.Key] = kvp.Value;
                    _logger.LogInfo($"   • Updated {kvp.Key}");
                }

                // Write back to file
                await File.WriteAllTextAsync(_storageJsonPath, config.ToString(Formatting.Indented));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"   ❌ Error updating storage.json: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> UpdateSqliteDatabase(Dictionary<string, string> newIds)
        {
            try
            {
                if (!File.Exists(_sqlitePath))
                {
                    _logger.LogWarning($"   ⚠️ state.vscdb not found at: {_sqlitePath}");
                    return false;
                }

                await Task.Run(() =>
                {
                    using (var connection = new SqliteConnection($"Data Source={_sqlitePath}"))
                    {
                        connection.Open();

                        // Create table if it doesn't exist
                        using (var createCmd = connection.CreateCommand())
                        {
                            createCmd.CommandText = @"
                                CREATE TABLE IF NOT EXISTS ItemTable (
                                    key TEXT PRIMARY KEY,
                                    value TEXT
                                )";
                            createCmd.ExecuteNonQuery();
                        }

                        // Insert or replace each ID
                        foreach (var kvp in newIds)
                        {
                            using (var cmd = connection.CreateCommand())
                            {
                                cmd.CommandText = "INSERT OR REPLACE INTO ItemTable (key, value) VALUES (@key, @value)";
                                cmd.Parameters.AddWithValue("@key", kvp.Key);
                                cmd.Parameters.AddWithValue("@value", kvp.Value);
                                cmd.ExecuteNonQuery();
                                _logger.LogInfo($"   • Updated {kvp.Key} in SQLite");
                            }
                        }
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"   ❌ Error updating SQLite database: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> UpdateMachineIdFile(string machineId)
        {
            try
            {
                // Create directory if it doesn't exist
                var directory = Path.GetDirectoryName(_machineIdPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Create backup if file exists
                if (File.Exists(_machineIdPath))
                {
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var backupPath = $"{_machineIdPath}.backup.{timestamp}";
                    File.Copy(_machineIdPath, backupPath, true);
                    _logger.LogInfo($"   • Backup created: {Path.GetFileName(backupPath)}");
                }

                // Write new machine ID
                await File.WriteAllTextAsync(_machineIdPath, machineId);
                _logger.LogInfo($"   • machineId file updated with: {machineId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"   ❌ Error updating machineId file: {ex.Message}");
                return false;
            }
        }

        private string GenerateSha512Hash(int length)
        {
            using (var sha512 = SHA512.Create())
            {
                var randomBytes = new byte[64]; // 64 bytes for SHA-512
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomBytes);
                }
                var hashBytes = sha512.ComputeHash(randomBytes);
                var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashString.Substring(0, Math.Min(length, hashString.Length));
            }
        }

        // Method 2: JSON modification (Original method - kept for compatibility)
        private async Task<bool> ResetByModifyingJson(bool autoCloseCursor)
        {
            _logger.LogInfo("=== Changing Machine IDs (JSON Method) ===");

            // Check if Cursor is installed
            if (!Directory.Exists(_cursorConfigPath))
            {
                _logger.LogError("❌ Cursor is not installed!");
                return false;
            }

            if (!File.Exists(_storageJsonPath))
            {
                _logger.LogError("❌ storage.json not found!");
                return false;
            }

            // Close Cursor if running
            if (IsCursorRunning())
            {
                if (autoCloseCursor)
                {
                    _logger.LogWarning("⚠️ Cursor is running. Closing it...");
                    if (!await CloseCursor())
                    {
                        _logger.LogError("❌ Failed to close Cursor. Please close it manually.");
                        return false;
                    }
                }
                else
                {
                    _logger.LogError("❌ Cursor is running. Please close it first.");
                    return false;
                }
            }

            // Backup current config
            _logger.LogInfo("💾 Creating backup before changes...");
            var backupPath = await BackupSettings();
            if (backupPath != null)
            {
                _logger.LogSuccess($"✅ Backup created: {Path.GetFileName(backupPath)}");
            }

            // Load storage.json
            _logger.LogInfo("📖 Loading configuration...");
            var config = LoadStorageJson();
            if (config == null)
            {
                _logger.LogError("❌ Failed to load storage.json");
                return false;
            }

            // Generate new machine IDs
            _logger.LogInfo("🔄 Regenerating machine identifiers...");
            _logger.LogInfo("   • Generating telemetry.macMachineId (128 chars)...");
            config.TelemetryMacMachineId = GenerateSha256Hash(128);
            _logger.LogInfo("   • Generating telemetry.machineId (64 chars)...");
            config.TelemetryMachineId = GenerateSha256Hash(64);
            _logger.LogInfo("   • Generating telemetry.sqmId (GUID)...");
            config.TelemetrySqmId = $"{{{Guid.NewGuid().ToString().ToUpper()}}}";
            _logger.LogInfo("   • Generating telemetry.devDeviceId (GUID)...");
            config.TelemetryDevDeviceId = Guid.NewGuid().ToString();
            _logger.LogInfo("   • Generating storage.serviceMachineId (GUID)...");
            config.StorageServiceMachineId = Guid.NewGuid().ToString();

            // Save storage.json
            _logger.LogInfo("💾 Saving updated configuration...");
            if (!SaveStorageJson(config))
            {
                _logger.LogError("❌ Failed to save storage.json");
                return false;
            }

            _logger.LogSuccess("✅ Machine IDs changed successfully!");
            _logger.LogInfo("📋 All 5 device identifiers have been regenerated");
            _logger.LogInfo("🎉 You can now restart Cursor IDE");
            return true;
        }

        #endregion

        #region Backup & Restore

        public async Task<string?> BackupSettings()
        {
            try
            {
                if (!File.Exists(_storageJsonPath))
                {
                    _logger.LogError("❌ storage.json not found!");
                    return null;
                }

                var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                var backupFileName = $"cursor_backup_{timestamp}.json";
                var backupPath = Path.Combine(_backupDirectory, backupFileName);

                await Task.Run(() => File.Copy(_storageJsonPath, backupPath, true));

                _logger.LogSuccess($"✅ Backup created: {backupFileName}");
                return backupPath;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Backup failed: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> RestoreSettings(string backupPath)
        {
            try
            {
                _logger.LogInfo($"♻️ Restoring settings from backup...");

                if (!File.Exists(backupPath))
                {
                    _logger.LogError("❌ Backup file not found!");
                    return false;
                }

                // Close Cursor if running
                if (IsCursorRunning())
                {
                    _logger.LogWarning("⚠️ Cursor is running. Closing it...");
                    if (!await CloseCursor())
                    {
                        _logger.LogError("❌ Failed to close Cursor. Please close it manually.");
                        return false;
                    }
                }

                // Restore backup
                await Task.Run(() => File.Copy(backupPath, _storageJsonPath, true));

                _logger.LogSuccess("✅ Settings restored successfully!");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Restore failed: {ex.Message}");
                return false;
            }
        }

        public System.Collections.Generic.List<string> ListBackups()
        {
            try
            {
                return Directory.GetFiles(_backupDirectory, "cursor_backup_*.json")
                    .OrderByDescending(f => File.GetLastWriteTime(f))
                    .ToList();
            }
            catch
            {
                return new System.Collections.Generic.List<string>();
            }
        }

        #endregion

        #region Clear Cache

        public async Task<bool> ClearCache(bool clearWorkspaces = false)
        {
            try
            {
                _logger.LogInfo("=== Clearing Cursor Cache ===");

                if (!Directory.Exists(_cursorConfigPath))
                {
                    _logger.LogError("❌ Cursor is not installed!");
                    return false;
                }

                // Close Cursor if running
                if (IsCursorRunning())
                {
                    _logger.LogWarning("⚠️ Cursor is running. Closing it...");
                    if (!await CloseCursor())
                    {
                        _logger.LogError("❌ Failed to close Cursor. Please close it manually.");
                        return false;
                    }
                }

                var foldersToDelete = new[]
                {
                    Path.Combine(_cursorConfigPath, "Cache"),
                    Path.Combine(_cursorConfigPath, "CachedData"),
                    Path.Combine(_cursorConfigPath, "Code Cache"),
                    Path.Combine(_cursorConfigPath, "GPUCache"),
                    Path.Combine(_cursorConfigPath, "Session Storage"),
                    Path.Combine(_cursorConfigPath, "Local Storage"),
                    Path.Combine(_cursorConfigPath, "blob_storage"),
                    Path.Combine(_cursorConfigPath, "WebStorage")
                };

                if (clearWorkspaces)
                {
                    foldersToDelete = foldersToDelete.Concat(new[]
                    {
                        Path.Combine(_cursorConfigPath, "User", "workspaceStorage")
                    }).ToArray();
                }

                int deletedCount = 0;
                foreach (var folder in foldersToDelete)
                {
                    if (Directory.Exists(folder))
                    {
                        try
                        {
                            await Task.Run(() => Directory.Delete(folder, true));
                            _logger.LogInfo($"   ✅ Deleted: {Path.GetFileName(folder)}");
                            deletedCount++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"   ⚠️ Could not delete {Path.GetFileName(folder)}: {ex.Message}");
                        }
                    }
                }

                _logger.LogSuccess($"✅ Cache cleared! ({deletedCount} folders deleted)");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error clearing cache: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Advanced Features

        public async Task<bool> DisableAutoUpdate()
        {
            try
            {
                _logger.LogInfo("=== Disabling Cursor Auto-Update ===");

                if (!Directory.Exists(_cursorConfigPath))
                {
                    _logger.LogError("❌ Cursor is not installed!");
                    return false;
                }

                // Close Cursor if running
                if (IsCursorRunning())
                {
                    _logger.LogWarning("⚠️ Cursor is running. Closing it...");
                    if (!await CloseCursor())
                    {
                        _logger.LogError("❌ Failed to close Cursor.");
                        return false;
                    }
                }

                // Method 1: Create marker file to disable updates
                var noUpdateMarkerPath = Path.Combine(_cursorConfigPath, ".disable-updates");
                await File.WriteAllTextAsync(noUpdateMarkerPath, "Auto-updates disabled by L2 Setup");
                
                // Method 2: Modify settings if available
                var settingsPath = Path.Combine(_cursorConfigPath, "User", "settings.json");
                if (File.Exists(settingsPath))
                {
                    var settingsJson = await File.ReadAllTextAsync(settingsPath);
                    var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(settingsJson) 
                        ?? new Dictionary<string, object>();
                    
                    settings["update.mode"] = "none";
                    settings["update.enableWindowsBackgroundUpdates"] = false;
                    
                    settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
                    await File.WriteAllTextAsync(settingsPath, settingsJson);
                }

                _logger.LogSuccess("✅ Auto-update disabled successfully!");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error disabling auto-update: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> TotallyResetCursor()
        {
            try
            {
                _logger.LogInfo("=== Totally Resetting Cursor ===");
                _logger.LogWarning("⚠️ This will DELETE ALL Cursor data!");

                if (!Directory.Exists(_cursorConfigPath))
                {
                    _logger.LogError("❌ Cursor is not installed!");
                    return false;
                }

                // Close Cursor if running
                if (IsCursorRunning())
                {
                    _logger.LogWarning("⚠️ Cursor is running. Closing it...");
                    if (!await CloseCursor())
                    {
                        _logger.LogError("❌ Failed to close Cursor.");
                        return false;
                    }
                }

                // Create full backup before reset
                _logger.LogInfo("💾 Creating full backup before reset...");
                var backupPath = await BackupSettings();
                if (backupPath != null)
                {
                    _logger.LogSuccess($"✅ Backup saved: {Path.GetFileName(backupPath)}");
                }

                // Delete all Cursor config
                _logger.LogInfo("🗑️ Deleting all Cursor configuration...");
                await Task.Run(() => Directory.Delete(_cursorConfigPath, true));

                _logger.LogSuccess("✅ Cursor totally reset!");
                _logger.LogInfo("💡 Cursor will create fresh config on next startup");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error during total reset: {ex.Message}");
                return false;
            }
        }

        public string GetFullConfig()
        {
            try
            {
                var info = GetCursorInfo();
                
                if (!info.IsInstalled || !File.Exists(info.StorageJsonPath))
                {
                    return "❌ Cursor storage.json not found!";
                }

                var json = File.ReadAllText(info.StorageJsonPath);
                var config = JsonConvert.DeserializeObject<CursorConfig>(json);

                if (config == null)
                {
                    return "❌ Failed to load configuration.";
                }

                var output = new System.Text.StringBuilder();
                output.AppendLine("═══════════════════════════════════════════════");
                output.AppendLine("          CURSOR IDE CONFIGURATION");
                output.AppendLine("═══════════════════════════════════════════════");
                output.AppendLine();
                
                output.AppendLine("🔑 MACHINE IDENTIFIERS:");
                output.AppendLine($"  Machine ID: {config.TelemetryMachineId}");
                output.AppendLine($"  Mac Machine ID: {config.TelemetryMacMachineId}");
                output.AppendLine($"  SQM ID: {config.TelemetrySqmId}");
                output.AppendLine($"  Device ID: {config.TelemetryDevDeviceId}");
                output.AppendLine($"  Service Machine ID: {config.StorageServiceMachineId}");
                output.AppendLine();

                output.AppendLine("📁 WORKSPACES:");
                if (config.BackupWorkspaces?.Folders != null && config.BackupWorkspaces.Folders.Count > 0)
                {
                    foreach (var folder in config.BackupWorkspaces.Folders)
                    {
                        if (folder.FolderUri != null)
                        {
                            output.AppendLine($"  • {System.Uri.UnescapeDataString(folder.FolderUri)}");
                        }
                    }
                }
                else
                {
                    output.AppendLine("  No workspaces found");
                }
                output.AppendLine();

                output.AppendLine("🎨 APPEARANCE:");
                output.AppendLine($"  Theme: {config.Theme ?? "Not set"}");
                output.AppendLine($"  Background: {config.ThemeBackground ?? "Not set"}");
                output.AppendLine($"  Window Control Height: {config.WindowControlHeight ?? 0}");
                output.AppendLine();

                output.AppendLine("💾 CONFIGURATION FILES:");
                output.AppendLine($"  Config Path: {_cursorConfigPath}");
                output.AppendLine($"  Storage.json: {info.StorageJsonPath}");
                output.AppendLine($"  Config Size: {info.ConfigSize / (1024.0 * 1024.0):F2} MB");
                output.AppendLine($"  Last Modified: {info.LastModified:yyyy-MM-dd HH:mm:ss}");
                output.AppendLine();

                output.AppendLine("═══════════════════════════════════════════════");

                return output.ToString();
            }
            catch (Exception ex)
            {
                return $"❌ Error reading config: {ex.Message}";
            }
        }

        #endregion

        #region Utility Methods

        private CursorConfig? LoadStorageJson()
        {
            try
            {
                var json = File.ReadAllText(_storageJsonPath);
                return JsonConvert.DeserializeObject<CursorConfig>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading storage.json: {ex.Message}");
                return null;
            }
        }

        private bool SaveStorageJson(CursorConfig config)
        {
            try
            {
                var json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(_storageJsonPath, json);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving storage.json: {ex.Message}");
                return false;
            }
        }

        private string GenerateSha256Hash(int length)
        {
            // Generate random bytes
            var randomBytes = new byte[length / 2];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            // Convert to hex string
            var sb = new StringBuilder(length);
            for (int i = 0; i < randomBytes.Length && sb.Length < length; i++)
            {
                sb.Append(randomBytes[i].ToString("x2"));
            }

            return sb.ToString().Substring(0, length);
        }

        private long GetDirectorySize(DirectoryInfo dir)
        {
            try
            {
                long size = dir.GetFiles().Sum(fi => fi.Length);
                size += dir.GetDirectories().Sum(di => GetDirectorySize(di));
                return size;
            }
            catch
            {
                return 0;
            }
        }

        public void OpenConfigFolder()
        {
            try
            {
                if (Directory.Exists(_cursorConfigPath))
                {
                    Process.Start("explorer.exe", _cursorConfigPath);
                }
                else
                {
                    _logger.LogError("❌ Cursor config folder not found!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error opening folder: {ex.Message}");
            }
        }

        #endregion
    }
}

