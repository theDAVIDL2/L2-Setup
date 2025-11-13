using System;
using System.Collections.Generic;

namespace WindowsSetup.App.Models
{
    public class OptimizationBackup
    {
        public string BackupId { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Description { get; set; } = "Windows Optimization Backup";
        public List<RegistryBackupEntry> RegistryChanges { get; set; } = new();
        public List<ServiceBackupEntry> ServiceChanges { get; set; } = new();
        public List<PowerSettingBackupEntry> PowerSettings { get; set; } = new();
        public List<FileBackupEntry> FileChanges { get; set; } = new();
        public List<NetworkSettingBackupEntry> NetworkSettings { get; set; } = new();
        public List<DnsBackupEntry> DnsSettings { get; set; } = new();
        public List<TcpIpBackupEntry> TcpIpSettings { get; set; } = new();
    }

    public class RegistryBackupEntry
    {
        public string Hive { get; set; } = ""; // HKCU or HKLM
        public string Path { get; set; } = "";
        public string ValueName { get; set; } = "";
        public object? OriginalValue { get; set; }
        public string? OriginalValueKind { get; set; }
        public bool ValueExisted { get; set; }
        public bool KeyExisted { get; set; }
    }

    public class ServiceBackupEntry
    {
        public string ServiceName { get; set; } = "";
        public string OriginalStartMode { get; set; } = ""; // auto, manual, disabled
        public string OriginalStatus { get; set; } = ""; // running, stopped
    }

    public class PowerSettingBackupEntry
    {
        public string SettingType { get; set; } = ""; // scheme, timeout, etc.
        public string OriginalValue { get; set; } = "";
    }

    public class FileBackupEntry
    {
        public string FilePath { get; set; } = "";
        public string Action { get; set; } = ""; // deleted, modified
        public string? BackupPath { get; set; }
    }

    public class NetworkSettingBackupEntry
    {
        public string InterfaceName { get; set; } = "";
        public string SettingName { get; set; } = "";
        public object? OriginalValue { get; set; }
        public string? ValueKind { get; set; }
        public bool ValueExisted { get; set; }
    }

    public class DnsBackupEntry
    {
        public string InterfaceName { get; set; } = "";
        public List<string> OriginalDnsServers { get; set; } = new();
        public bool WasDhcp { get; set; }
    }

    public class TcpIpBackupEntry
    {
        public string SettingName { get; set; } = "";
        public string OriginalValue { get; set; } = "";
        public string CommandOutput { get; set; } = "";
    }
}

