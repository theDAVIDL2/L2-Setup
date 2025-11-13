using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsSetup.App.Models;

namespace WindowsSetup.App.Views
{
    public partial class OptimizationWindow : Window
    {
        public OptimizationSettings Settings { get; private set; }
        public bool WasApplied { get; private set; }

        public OptimizationWindow()
        {
            InitializeComponent();
            Settings = new OptimizationSettings();
            WasApplied = false;
            
            // Update counter initially
            UpdateSelectedCount();
            
            // Add checkbox change handlers
            foreach (var checkbox in FindVisualChildren<CheckBox>(this).Where(cb => cb.Name != "ChkCreateRestorePoint"))
            {
                checkbox.Checked += OnCheckboxChanged;
                checkbox.Unchecked += OnCheckboxChanged;
            }
        }

        private void OnCheckboxChanged(object sender, RoutedEventArgs e)
        {
            UpdateSelectedCount();
        }

        private void UpdateSelectedCount()
        {
            int count = FindVisualChildren<CheckBox>(this)
                .Where(cb => cb.Name != "ChkCreateRestorePoint" && cb.IsChecked == true)
                .Count();
            
            if (SelectionCount != null)
            {
                SelectionCount.Text = $"{count} optimization{(count != 1 ? "s" : "")} selected";
            }
        }

        #region Preset Buttons

        private void RecommendedSafe_Click(object sender, RoutedEventArgs e)
        {
            // Select all safe optimizations
            OptPowerPlan.IsChecked = true;
            OptMouseAcceleration.IsChecked = true;
            
            OptTelemetry.IsChecked = true;
            OptCortana.IsChecked = true;
            OptAdvertisingId.IsChecked = true;
            OptLocationTracking.IsChecked = true;
            OptDiagnostics.IsChecked = true;
            
            OptGameMode.IsChecked = true;
            OptGameBar.IsChecked = true;
            OptGameDVR.IsChecked = true;
            
            OptShowFileExtensions.IsChecked = true;
            OptShowHiddenFiles.IsChecked = true;
            
            OptCleanTemp.IsChecked = true;
            
            UpdateSelectedCount();
        }

        private void DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var checkbox in FindVisualChildren<CheckBox>(this).Where(cb => cb.Name != "ChkCreateRestorePoint"))
            {
                checkbox.IsChecked = false;
            }
            UpdateSelectedCount();
        }

        #endregion

        #region Apply & Cancel

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            // Map UI checkboxes to Settings model (ONLY SAFE OPTIONS)
            Settings.HighPerformancePowerPlan = OptPowerPlan.IsChecked ?? false;
            Settings.DisableMouseAcceleration = OptMouseAcceleration.IsChecked ?? false;
            
            Settings.DisableTelemetry = OptTelemetry.IsChecked ?? false;
            Settings.DisableCortana = OptCortana.IsChecked ?? false;
            Settings.DisableAdvertisingId = OptAdvertisingId.IsChecked ?? false;
            Settings.DisableLocationTracking = OptLocationTracking.IsChecked ?? false;
            Settings.DisableDiagnostics = OptDiagnostics.IsChecked ?? false;
            
            Settings.EnableGameMode = OptGameMode.IsChecked ?? false;
            Settings.DisableGameBar = OptGameBar.IsChecked ?? false;
            Settings.DisableGameDVR = OptGameDVR.IsChecked ?? false;
            
            Settings.ShowFileExtensions = OptShowFileExtensions.IsChecked ?? false;
            Settings.ShowHiddenFiles = OptShowHiddenFiles.IsChecked ?? false;
            
            Settings.CleanTempFiles = OptCleanTemp.IsChecked ?? false;
            
            Settings.CreateRestorePoint = ChkCreateRestorePoint.IsChecked ?? true;

            // Set all risky options to FALSE (ensure they're never applied)
            Settings.OptimizeVisualEffects = false;
            Settings.OptimizeExplorer = false;
            Settings.DisableStartupPrograms = false;
            Settings.OptimizePageFile = false;
            Settings.DisableBackgroundApps = false;
            Settings.DisableTransparency = false;
            Settings.DisableAnimations = false;
            Settings.DisableActivityHistory = false;
            Settings.DisableWebSearch = false;
            Settings.DisableBiometrics = false;
            Settings.DisableCameraAccess = false;
            Settings.DisablePrintSpooler = false;
            Settings.DisableFax = false;
            Settings.DisableWindowsSearch = false;
            Settings.DisableSuperfetch = false;
            Settings.SetWindowsUpdateManual = false;
            Settings.DisableWindowsDefender = false;
            Settings.DisableFirewall = false;
            Settings.DisableUAC = false;
            Settings.DisableSmartScreen = false;
            Settings.EnableHardwareAcceleratedGPU = false;
            Settings.DisableFullscreenOptimizations = false;
            Settings.OptimizeCPUScheduling = false;
            Settings.DisableNagleAlgorithm = false;
            Settings.OptimizeTCPIP = false;
            Settings.DisableLargeSendOffload = false;
            Settings.OptimizeDNS = false;
            Settings.DisableNetworkThrottling = false;
            Settings.OptimizeNetworkAdapter = false;
            Settings.EmptyRecycleBin = false;
            Settings.DeleteWindowsOld = false;
            Settings.CleanDownloads = false;
            Settings.RemoveBloatwareApps = false;
            Settings.DisableWidgets = false;
            Settings.DisableNewsInterests = false;
            Settings.RemoveCoPilot = false;
            Settings.OptimizeSSD = false;
            Settings.DisablePrefetch = false;
            Settings.OptimizeMemoryManagement = false;
            Settings.DisableHibernation = false;
            Settings.DisableFastStartup = false;
            Settings.DisableOneDrive = false;
            Settings.DisableLockScreen = false;
            Settings.DisableActionCenter = false;

            WasApplied = true;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            WasApplied = false;
            DialogResult = false;
            Close();
        }

        #endregion

        #region Helper Methods

        private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                if (child is T t)
                {
                    yield return t;
                }

                foreach (var childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }

        #endregion
    }
}
