namespace WindowsSetup.App.Models
{
    public class OptimizationSettings
    {
        // ═══════════════════════════════════════════════════════════════
        // 🚀 PERFORMANCE OPTIMIZATIONS
        // ═══════════════════════════════════════════════════════════════
        
        public bool HighPerformancePowerPlan { get; set; } = true;
        public bool DisableMouseAcceleration { get; set; } = true;
        public bool OptimizeVisualEffects { get; set; } = true;
        public bool OptimizeExplorer { get; set; } = true;
        public bool DisableStartupPrograms { get; set; } = false;
        public bool OptimizePageFile { get; set; } = false;
        public bool DisableBackgroundApps { get; set; } = true; // NEW
        public bool DisableTransparency { get; set; } = true; // NEW
        public bool DisableAnimations { get; set; } = true; // NEW
        
        // ═══════════════════════════════════════════════════════════════
        // 🔒 PRIVACY & TELEMETRY
        // ═══════════════════════════════════════════════════════════════
        
        public bool DisableTelemetry { get; set; } = true;
        public bool DisableCortana { get; set; } = true;
        public bool DisableAdvertisingId { get; set; } = true;
        public bool DisableLocationTracking { get; set; } = true;
        public bool DisableDiagnostics { get; set; } = true;
        public bool DisableActivityHistory { get; set; } = true; // NEW
        public bool DisableWebSearch { get; set; } = true; // NEW
        public bool DisableBiometrics { get; set; } = false; // NEW
        public bool DisableCameraAccess { get; set; } = false; // NEW
        
        // ═══════════════════════════════════════════════════════════════
        // ⚙️ SERVICES & FEATURES
        // ═══════════════════════════════════════════════════════════════
        
        public bool DisablePrintSpooler { get; set; } = false;
        public bool DisableFax { get; set; } = false;
        public bool DisableWindowsSearch { get; set; } = false;
        public bool DisableSuperfetch { get; set; } = false;
        public bool SetWindowsUpdateManual { get; set; } = false;
        public bool DisableWindowsDefender { get; set; } = false; // EXPERT MODE
        public bool DisableFirewall { get; set; } = false; // EXPERT MODE
        public bool DisableUAC { get; set; } = false; // EXPERT MODE
        public bool DisableSmartScreen { get; set; } = false; // NEW
        
        // ═══════════════════════════════════════════════════════════════
        // 🎮 GAMING OPTIMIZATIONS
        // ═══════════════════════════════════════════════════════════════
        
        public bool EnableGameMode { get; set; } = true;
        public bool DisableGameBar { get; set; } = true;
        public bool DisableGameDVR { get; set; } = true;
        public bool EnableHardwareAcceleratedGPU { get; set; } = true;
        public bool DisableFullscreenOptimizations { get; set; } = true; // NEW
        public bool OptimizeCPUScheduling { get; set; } = true; // NEW
        public bool DisableNagleAlgorithm { get; set; } = true; // NEW - Network gaming
        
        // ═══════════════════════════════════════════════════════════════
        // 🌐 NETWORK OPTIMIZATIONS
        // ═══════════════════════════════════════════════════════════════
        
        public bool OptimizeTCPIP { get; set; } = true; // NEW
        public bool DisableLargeSendOffload { get; set; } = false; // NEW
        public bool OptimizeDNS { get; set; } = true; // NEW - Cloudflare 1.1.1.1
        public bool DisableNetworkThrottling { get; set; } = true; // NEW
        public bool OptimizeNetworkAdapter { get; set; } = true; // NEW
        
        // ═══════════════════════════════════════════════════════════════
        // 🗑️ DEBLOAT & CLEANUP
        // ═══════════════════════════════════════════════════════════════
        
        public bool CleanTempFiles { get; set; } = true;
        public bool EmptyRecycleBin { get; set; } = false;
        public bool DeleteWindowsOld { get; set; } = false;
        public bool CleanDownloads { get; set; } = false;
        public bool RemoveBloatwareApps { get; set; } = true; // NEW
        public bool DisableWidgets { get; set; } = true; // NEW
        public bool DisableNewsInterests { get; set; } = true; // NEW
        public bool RemoveCoPilot { get; set; } = true; // NEW
        public bool DisableChatIcon { get; set; } = true; // NEW
        
        // ═══════════════════════════════════════════════════════════════
        // 💾 STORAGE & MEMORY
        // ═══════════════════════════════════════════════════════════════
        
        public bool DisableSearchIndexing { get; set; } = false; // NEW
        public bool OptimizeSSD { get; set; } = true; // NEW - TRIM
        public bool DisableSystemRestore { get; set; } = false; // NEW - Frees space
        public bool CompactOS { get; set; } = false; // NEW - Compress Windows
        public bool DisablePrefetch { get; set; } = false; // NEW
        
        // ═══════════════════════════════════════════════════════════════
        // 🖥️ CPU & MEMORY OPTIMIZATIONS
        // ═══════════════════════════════════════════════════════════════
        
        public bool DisableCoreParking { get; set; } = true; // NEW
        public bool OptimizeProcessorScheduling { get; set; } = true; // NEW
        public bool DisableSpectreMeltdown { get; set; } = false; // EXPERT - Security risk!
        public bool OptimizeMemoryManagement { get; set; } = true; // NEW
        public bool DisableWriteCacheBufferFlushing { get; set; } = false; // NEW
        
        // ═══════════════════════════════════════════════════════════════
        // 🎨 VISUAL & UI TWEAKS
        // ═══════════════════════════════════════════════════════════════
        
        public bool ShowFileExtensions { get; set; } = true;
        public bool ShowHiddenFiles { get; set; } = true;
        public bool DisableLockScreen { get; set; } = false; // NEW
        public bool DisableActionCenter { get; set; } = false; // NEW
        public bool ClassicContextMenu { get; set; } = true; // NEW - Win11
        public bool TaskbarToLeft { get; set; } = false; // NEW - Win11
        public bool DisableSnapAssist { get; set; } = false; // NEW
        
        // ═══════════════════════════════════════════════════════════════
        // ⚡ ADVANCED & EXPERT
        // ═══════════════════════════════════════════════════════════════
        
        public bool CreateRestorePoint { get; set; } = true;
        public bool DisableOneDrive { get; set; } = false;
        public bool DisableHibernation { get; set; } = false;
        public bool RunChrisTitusTechScript { get; set; } = false; // External script
        public bool DisableFastStartup { get; set; } = false; // NEW
        public bool DisableRemoteAssistance { get; set; } = true; // NEW
        public bool DisableErrorReporting { get; set; } = true; // NEW
    }
}
