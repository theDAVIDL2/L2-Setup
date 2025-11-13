using System;
using System.Collections.Generic;

namespace WindowsSetup.App.Models
{
    public class SystemIdBackup
    {
        public DateTime BackupDate { get; set; } = DateTime.Now;
        public string BackupId { get; set; } = Guid.NewGuid().ToString();
        public string Description { get; set; } = "System Identity Backup";
        
        // Registry backups (uses shared RegistryBackupEntry from OptimizationBackup.cs)
        public List<RegistryBackupEntry> RegistryBackups { get; set; } = new();
        
        // MAC addresses (adapter name -> MAC)
        public Dictionary<string, string> MacAddresses { get; set; } = new();
        
        // Volume serials (drive letter -> serial)
        public Dictionary<string, string> VolumeSerials { get; set; } = new();
        
        // Hardware IDs
        public Dictionary<string, string> HardwareIds { get; set; } = new();
        
        // Network settings
        public NetworkBackup? NetworkSettings { get; set; }
    }

    public class NetworkBackup
    {
        public bool ProxyEnabled { get; set; }
        public string ProxyServer { get; set; } = "";
        public string ProxyBypass { get; set; } = "";
        public bool TorRunning { get; set; }
        public bool VpnConnected { get; set; }
        public string VpnConfig { get; set; } = "";
    }

    public class NetworkAdapterInfo
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string CurrentMac { get; set; } = "";
        public string OriginalMac { get; set; } = "";
        public bool IsEnabled { get; set; }
        public string AdapterId { get; set; } = "";
        public bool IsSpoofed { get; set; }
    }

    public class VolumeInfo
    {
        public string DriveLetter { get; set; } = "";
        public string Label { get; set; } = "";
        public string CurrentSerial { get; set; } = "";
        public string OriginalSerial { get; set; } = "";
        public bool IsSpoofed { get; set; }
        public long TotalSize { get; set; }
        public string FileSystem { get; set; } = "";
    }
}

