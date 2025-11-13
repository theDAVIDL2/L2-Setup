using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;
using WindowsSetup.App.Models;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App.Services
{
    public class BrowserBackupService
    {
        private readonly Logger _logger;
        private readonly string _bravePath;

        public BrowserBackupService(Logger logger)
        {
            _logger = logger;
            _bravePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BraveSoftware", "Brave-Browser", "User Data");
        }

        public async Task<BackupResult> BackupBraveProfile(string destinationPath)
        {
            try
            {
                // Check if Brave is running
                if (IsBraveRunning())
                {
                    _logger.LogWarning("Brave is running. Please close Brave before backing up.");
                    return new BackupResult
                    {
                        Success = false,
                        ErrorMessage = "Brave is currently running. Please close it and try again."
                    };
                }

                // Check if profile exists
                if (!Directory.Exists(_bravePath))
                {
                    _logger.LogError("Brave profile directory not found!");
                    return new BackupResult
                    {
                        Success = false,
                        ErrorMessage = "Brave profile not found. Is Brave installed?"
                    };
                }

                _logger.LogInfo($"Starting backup from: {_bravePath}");
                _logger.LogInfo($"Destination: {destinationPath}");

                // Create ZIP archive
                await Task.Run(() =>
                {
                    using var archive = ZipArchive.Create();
                    AddDirectoryToArchive(archive, _bravePath, "User Data");
                    
                    using var writer = WriterFactory.Open(new FileStream(destinationPath, FileMode.Create), 
                        ArchiveType.Zip, new WriterOptions(CompressionType.Deflate) { LeaveStreamOpen = false });
                    archive.SaveTo(writer);
                });

                // Get file info
                var fileInfo = new FileInfo(destinationPath);
                var sizeInMB = fileInfo.Length / (1024.0 * 1024.0);

                // Count extensions
                var extensionsPath = Path.Combine(_bravePath, "Default", "Extensions");
                var extensionsCount = 0;
                if (Directory.Exists(extensionsPath))
                {
                    extensionsCount = Directory.GetDirectories(extensionsPath).Length;
                }

                _logger.LogSuccess($"Backup completed! Size: {sizeInMB:F2} MB, Extensions: {extensionsCount}");

                return new BackupResult
                {
                    Success = true,
                    BackupPath = destinationPath,
                    SizeInMB = sizeInMB,
                    ExtensionsCount = extensionsCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backup failed: {ex.Message}");
                return new BackupResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<RestoreResult> RestoreBraveProfile(string backupPath)
        {
            try
            {
                // Check if Brave is running
                if (IsBraveRunning())
                {
                    _logger.LogWarning("Attempting to close Brave...");
                    await CloseBrave();
                    await Task.Delay(2000); // Wait for Brave to close
                }

                // Backup existing profile
                var braveDir = Path.GetDirectoryName(_bravePath);
                if (Directory.Exists(_bravePath))
                {
                    var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
                    var oldProfilePath = Path.Combine(braveDir!, $"User Data_OLD_{timestamp}");
                    _logger.LogInfo($"Backing up existing profile to: {oldProfilePath}");
                    Directory.Move(_bravePath, oldProfilePath);
                }

                // Extract backup
                _logger.LogInfo($"Restoring backup from: {backupPath}");
                
                await Task.Run(() =>
                {
                    using var archive = ZipArchive.Open(backupPath);
                    foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                    {
                        var extractPath = Path.Combine(braveDir!, entry.Key);
                        var directory = Path.GetDirectoryName(extractPath);
                        
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        
                        entry.WriteToFile(extractPath, new ExtractionOptions { ExtractFullPath = true, Overwrite = true });
                    }
                });

                _logger.LogSuccess("Profile restored successfully!");

                return new RestoreResult { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Restore failed: {ex.Message}");
                return new RestoreResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task SetBraveAsDefaultBrowser()
        {
            try
            {
                _logger.LogInfo("Setting Brave as default browser...");

                var braveProgId = "BraveHTML";

                // Set HTTP
                Registry.SetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice",
                    "ProgId", braveProgId);

                // Set HTTPS
                Registry.SetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\https\UserChoice",
                    "ProgId", braveProgId);

                // Set HTM
                Registry.SetValue(
                    @"HKEY_CURRENT_USER\Software\Classes\.htm\OpenWithProgids",
                    braveProgId, new byte[0], RegistryValueKind.None);

                // Set HTML
                Registry.SetValue(
                    @"HKEY_CURRENT_USER\Software\Classes\.html\OpenWithProgids",
                    braveProgId, new byte[0], RegistryValueKind.None);

                _logger.LogSuccess("Brave set as default browser!");
                
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to set Brave as default: {ex.Message}");
                throw;
            }
        }

        public bool IsBraveRunning()
        {
            return Process.GetProcessesByName("brave").Any();
        }

        public async Task CloseBrave()
        {
            var processes = Process.GetProcessesByName("brave");
            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                    await process.WaitForExitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to close Brave process: {ex.Message}");
                }
            }
        }

        private void AddDirectoryToArchive(ZipArchive archive, string sourcePath, string entryPath)
        {
            foreach (var file in Directory.GetFiles(sourcePath))
            {
                var relativePath = Path.Combine(entryPath, Path.GetFileName(file));
                archive.AddEntry(relativePath, file);
            }

            foreach (var directory in Directory.GetDirectories(sourcePath))
            {
                var relativePath = Path.Combine(entryPath, Path.GetFileName(directory));
                AddDirectoryToArchive(archive, directory, relativePath);
            }
        }
    }
}

