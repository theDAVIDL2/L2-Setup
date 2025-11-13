using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WindowsSetup.App.Utils
{
    public class USBDriveDetector
    {
        public List<DriveInfo> GetUSBDrives()
        {
            return DriveInfo.GetDrives()
                .Where(d => d.DriveType == DriveType.Removable && d.IsReady)
                .ToList();
        }

        public async Task<List<string>> FindBackupsOnUSBAsync()
        {
            var backups = new List<string>();
            var usbDrives = GetUSBDrives();

            foreach (var drive in usbDrives)
            {
                try
                {
                    var searchPath = Path.Combine(drive.Name, "BraveBackups");
                    if (Directory.Exists(searchPath))
                    {
                        var backupFiles = Directory.GetFiles(searchPath, "Brave_Backup_*.zip");
                        backups.AddRange(backupFiles);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error scanning {drive.Name}: {ex.Message}");
                }
            }

            return backups;
        }
    }
}

