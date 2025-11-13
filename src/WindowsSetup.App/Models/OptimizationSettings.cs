namespace WindowsSetup.App.Models
{
    public class OptimizationSettings
    {
        // ═══════════════════════════════════════════════════════════════
        // 🚀 PERFORMANCE OPTIMIZATIONS (SAFE - Enabled by default)
        // ═══════════════════════════════════════════════════════════════
        
        public bool HighPerformancePowerPlan { get; set; } = true; // ✅ SAFE
        public bool DisableMouseAcceleration { get; set; } = true; // ✅ SAFE
        public bool OptimizeVisualEffects { get; set; } = false; // ⚠️ DISABLED - Can affect UI
        public bool OptimizeExplorer { get; set; } = false; // ⚠️ DISABLED - Unstable
        public bool DisableStartupPrograms { get; set; } = false; // ⚠️ DISABLED
        public bool OptimizePageFile { get; set; } = false; // ⚠️ DISABLED
        public bool DisableBackgroundApps { get; set; } = false; // ⚠️ DISABLED
        public bool DisableTransparency { get; set; } = false; // ⚠️ DISABLED
        public bool DisableAnimations { get; set; } = false; // ⚠️ DISABLED
        
        // ═══════════════════════════════════════════════════════════════
        // 🔒 PRIVACY & TELEMETRY (SAFE - Enabled by default)
        // ═══════════════════════════════════════════════════════════════
        
        public bool DisableTelemetry { get; set; } = true; // ✅ SAFE
        public bool DisableCortana { get; set; } = true; // ✅ SAFE
        public bool DisableAdvertisingId { get; set; } = true; // ✅ SAFE
        public bool DisableLocationTracking { get; set; } = true; // ✅ SAFE
        public bool DisableDiagnostics { get; set; } = true; // ✅ SAFE
        public bool DisableActivityHistory { get; set; } = false; // ⚠️ DISABLED
        public bool DisableWebSearch { get; set; } = false; // ⚠️ DISABLED
        public bool DisableBiometrics { get; set; } = false; // ⚠️ DISABLED
        public bool DisableCameraAccess { get; set; } = false; // ⚠️ DISABLED
        
        // ═══════════════════════════════════════════════════════════════
        // ⚙️ SERVICES & FEATURES (ALL DISABLED - Potentially unstable)
        // ═══════════════════════════════════════════════════════════════
        
        public bool DisablePrintSpooler { get; set; } = false; // ⚠️ DISABLED
        public bool DisableFax { get; set; } = false; // ⚠️ DISABLED
        public bool DisableWindowsSearch { get; set; } = false; // ⚠️ DISABLED
        public bool DisableSuperfetch { get; set; } = false; // ⚠️ DISABLED
        public bool SetWindowsUpdateManual { get; set; } = false; // ⚠️ DISABLED
        public bool DisableWindowsDefender { get; set; } = false; // ⚠️ DISABLED - DANGEROUS
        public bool DisableFirewall { get; set; } = false; // ⚠️ DISABLED - DANGEROUS
        public bool DisableUAC { get; set; } = false; // ⚠️ DISABLED - DANGEROUS
        public bool DisableSmartScreen { get; set; } = false; // ⚠️ DISABLED
        
        // ═══════════════════════════════════════════════════════════════
        // 🎮 GAMING OPTIMIZATIONS (Mostly disabled due to stability)
        // ═══════════════════════════════════════════════════════════════
        
        public bool EnableGameMode { get; set; } = true; // ✅ SAFE
        public bool DisableGameBar { get; set; } = true; // ✅ SAFE
        public bool DisableGameDVR { get; set; } = true; // ✅ SAFE
        public bool EnableHardwareAcceleratedGPU { get; set; } = false; // ⚠️ DISABLED - Can cause issues
        public bool DisableFullscreenOptimizations { get; set; } = false; // ⚠️ DISABLED
        public bool OptimizeCPUScheduling { get; set; } = false; // ⚠️ DISABLED
        public bool DisableNagleAlgorithm { get; set; } = false; // ⚠️ DISABLED
        
        // ═══════════════════════════════════════════════════════════════
        // 🌐 NETWORK OPTIMIZATIONS (ALL DISABLED - Can break connectivity)
        // ═══════════════════════════════════════════════════════════════
        
        public bool OptimizeTCPIP { get; set; } = false; // ⚠️ DISABLED
        public bool DisableLargeSendOffload { get; set; } = false; // ⚠️ DISABLED
        public bool OptimizeDNS { get; set; } = false; // ⚠️ DISABLED
        public bool DisableNetworkThrottling { get; set; } = false; // ⚠️ DISABLED
        public bool OptimizeNetworkAdapter { get; set; } = false; // ⚠️ DISABLED
        
        // ═══════════════════════════════════════════════════════════════
        // 🗑️ DEBLOAT & CLEANUP (Mostly disabled - Can remove needed apps)
        // ═══════════════════════════════════════════════════════════════
        
        public bool CleanTempFiles { get; set; } = true; // ✅ SAFE
        public bool EmptyRecycleBin { get; set; } = false; // User choice
        public bool DeleteWindowsOld { get; set; } = false; // User choice
        public bool CleanDownloads { get; set; } = false; // User choice
        public bool RemoveBloatwareApps { get; set; } = false; // ⚠️ DISABLED - Can remove needed apps
        public bool DisableWidgets { get; set; } = false; // ⚠️ DISABLED
        public bool DisableNewsInterests { get; set; } = false; // ⚠️ DISABLED
        public bool RemoveCoPilot { get; set; } = false; // ⚠️ DISABLED
        public bool DisableChatIcon { get; set; } = false; // ⚠️ DISABLED
        
        // ═══════════════════════════════════════════════════════════════
        // 💾 STORAGE & MEMORY (ALL DISABLED - Can cause system issues)
        // ═══════════════════════════════════════════════════════════════
        
        public bool DisableSearchIndexing { get; set; } = false; // ⚠️ DISABLED
        public bool OptimizeSSD { get; set; } = false; // ⚠️ DISABLED
        public bool DisableSystemRestore { get; set; } = false; // ⚠️ DISABLED
        public bool CompactOS { get; set; } = false; // ⚠️ DISABLED
        public bool DisablePrefetch { get; set; } = false; // ⚠️ DISABLED
        
        // ═══════════════════════════════════════════════════════════════
        // 🖥️ CPU & MEMORY OPTIMIZATIONS (ALL DISABLED - Advanced tweaks)
        // ═══════════════════════════════════════════════════════════════
        
        public bool DisableCoreParking { get; set; } = false; // ⚠️ DISABLED
        public bool OptimizeProcessorScheduling { get; set; } = false; // ⚠️ DISABLED
        public bool DisableSpectreMeltdown { get; set; } = false; // ⚠️ DISABLED - SECURITY RISK!
        public bool OptimizeMemoryManagement { get; set; } = false; // ⚠️ DISABLED
        public bool DisableWriteCacheBufferFlushing { get; set; } = false; // ⚠️ DISABLED
        
        // ═══════════════════════════════════════════════════════════════
        // 🎨 VISUAL & UI TWEAKS (Safe UI changes)
        // ═══════════════════════════════════════════════════════════════
        
        public bool ShowFileExtensions { get; set; } = true; // ✅ SAFE
        public bool ShowHiddenFiles { get; set; } = true; // ✅ SAFE
        public bool DisableLockScreen { get; set; } = false; // ⚠️ DISABLED
        public bool DisableActionCenter { get; set; } = false; // ⚠️ DISABLED
        public bool ClassicContextMenu { get; set; } = false; // ⚠️ DISABLED
        public bool TaskbarToLeft { get; set; } = false; // ⚠️ DISABLED
        public bool DisableSnapAssist { get; set; } = false; // ⚠️ DISABLED
        
        // ═══════════════════════════════════════════════════════════════
        // ⚡ ADVANCED & EXPERT (Mostly disabled)
        // ═══════════════════════════════════════════════════════════════
        
        public bool CreateRestorePoint { get; set; } = true; // ✅ SAFE - Always create restore point
        public bool DisableOneDrive { get; set; } = false; // ⚠️ DISABLED
        public bool DisableHibernation { get; set; } = false; // ⚠️ DISABLED
        public bool RunChrisTitusTechScript { get; set; } = false; // ⚠️ DISABLED - External script
        public bool DisableFastStartup { get; set; } = false; // ⚠️ DISABLED
        public bool DisableRemoteAssistance { get; set; } = false; // ⚠️ DISABLED
        public bool DisableErrorReporting { get; set; } = false; // ⚠️ DISABLED
    }
}
