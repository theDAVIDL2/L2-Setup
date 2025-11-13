using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WindowsSetup.App.Models
{
    public class CursorConfig
    {
        [JsonProperty("telemetry.macMachineId")]
        public string TelemetryMacMachineId { get; set; } = string.Empty;

        [JsonProperty("telemetry.machineId")]
        public string TelemetryMachineId { get; set; } = string.Empty;

        [JsonProperty("telemetry.sqmId")]
        public string TelemetrySqmId { get; set; } = string.Empty;

        [JsonProperty("telemetry.devDeviceId")]
        public string TelemetryDevDeviceId { get; set; } = string.Empty;

        [JsonProperty("storage.serviceMachineId")]
        public string StorageServiceMachineId { get; set; } = string.Empty;

        [JsonProperty("backupWorkspaces")]
        public BackupWorkspaces? BackupWorkspaces { get; set; }

        [JsonProperty("windowControlHeight")]
        public int? WindowControlHeight { get; set; }

        [JsonProperty("profileAssociations")]
        public ProfileAssociations? ProfileAssociations { get; set; }

        [JsonProperty("theme")]
        public string? Theme { get; set; }

        [JsonProperty("themeBackground")]
        public string? ThemeBackground { get; set; }

        [JsonProperty("windowSplash")]
        public object? WindowSplash { get; set; }

        [JsonProperty("windowsState")]
        public object? WindowsState { get; set; }

        [JsonProperty("windowSplashWorkspaceOverride")]
        public object? WindowSplashWorkspaceOverride { get; set; }

        // Store any additional properties not explicitly defined
        [JsonExtensionData]
        public Dictionary<string, object>? AdditionalData { get; set; }
    }

    public class BackupWorkspaces
    {
        [JsonProperty("workspaces")]
        public List<object>? Workspaces { get; set; }

        [JsonProperty("folders")]
        public List<FolderItem>? Folders { get; set; }

        [JsonProperty("emptyWindows")]
        public List<object>? EmptyWindows { get; set; }
    }

    public class FolderItem
    {
        [JsonProperty("folderUri")]
        public string? FolderUri { get; set; }
    }

    public class ProfileAssociations
    {
        [JsonProperty("workspaces")]
        public Dictionary<string, string>? Workspaces { get; set; }

        [JsonProperty("emptyWindows")]
        public Dictionary<string, string>? EmptyWindows { get; set; }
    }

    public class CursorInfo
    {
        public bool IsInstalled { get; set; }
        public bool IsRunning { get; set; }
        public string ConfigPath { get; set; } = string.Empty;
        public string StorageJsonPath { get; set; } = string.Empty;
        public long ConfigSize { get; set; }
        public int WorkspaceCount { get; set; }
        public DateTime? LastModified { get; set; }
    }
}

