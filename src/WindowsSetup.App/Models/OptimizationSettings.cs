namespace WindowsSetup.App.Models
{
    public class OptimizationSettings
    {
        // Performance
        public bool HighPerformancePowerPlan { get; set; }
        public bool DisableMouseAcceleration { get; set; }
        public bool OptimizeVisualEffects { get; set; }
        public bool OptimizeExplorer { get; set; }
        public bool DisableStartupPrograms { get; set; }
        public bool OptimizePageFile { get; set; }

        // Privacy
        public bool DisableTelemetry { get; set; }
        public bool DisableCortana { get; set; }
        public bool DisableAdvertisingId { get; set; }
        public bool DisableLocationTracking { get; set; }
        public bool DisableDiagnostics { get; set; }

        // Services
        public bool DisablePrintSpooler { get; set; }
        public bool DisableFax { get; set; }
        public bool DisableWindowsSearch { get; set; }
        public bool DisableSuperfetch { get; set; }
        public bool SetWindowsUpdateManual { get; set; }

        // Gaming
        public bool EnableGameMode { get; set; }
        public bool DisableGameBar { get; set; }
        public bool DisableGameDVR { get; set; }
        public bool EnableHardwareAcceleratedGPU { get; set; }

        // Cleanup
        public bool CleanTempFiles { get; set; }
        public bool EmptyRecycleBin { get; set; }
        public bool DeleteWindowsOld { get; set; }
        public bool CleanDownloads { get; set; }

        // Advanced
        public bool CreateRestorePoint { get; set; }
        public bool DisableOneDrive { get; set; }
        public bool DisableHibernation { get; set; }
    }
}

