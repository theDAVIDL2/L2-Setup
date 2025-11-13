using System;
using System.Windows;
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
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            SetAllCheckboxes(true);
        }

        private void DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            SetAllCheckboxes(false);
        }

        private void SelectRecommended_Click(object sender, RoutedEventArgs e)
        {
            // Performance
            OptPower.IsChecked = true;
            OptMouse.IsChecked = true;
            OptVisualEffects.IsChecked = true;
            OptExplorer.IsChecked = true;
            OptStartup.IsChecked = false;
            OptPageFile.IsChecked = false;

            // Privacy
            OptTelemetry.IsChecked = true;
            OptCortana.IsChecked = true;
            OptAdvertising.IsChecked = true;
            OptLocation.IsChecked = true;
            OptDiagnostics.IsChecked = true;

            // Services
            OptPrintSpooler.IsChecked = false;
            OptFax.IsChecked = true;
            OptWindowsSearch.IsChecked = false;
            OptSuperfetch.IsChecked = false;
            OptWindowsUpdate.IsChecked = false;

            // Gaming
            OptGameMode.IsChecked = true;
            OptGameBar.IsChecked = false;
            OptGameDVR.IsChecked = true;
            OptFullscreenOpt.IsChecked = true;

            // Cleanup
            OptTempFiles.IsChecked = true;
            OptRecycleBin.IsChecked = false;
            OptWindowsOld.IsChecked = false;
            OptDownloads.IsChecked = false;

            // Advanced
            OptCreateRestore.IsChecked = true;
            OptDisableOneDrive.IsChecked = false;
            OptDisableHibernation.IsChecked = false;
        }

        private void SetAllCheckboxes(bool value)
        {
            foreach (var child in FindVisualChildren<System.Windows.Controls.CheckBox>(this))
            {
                child.IsChecked = value;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            WasApplied = false;
            Close();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            // Collect settings
            Settings = new OptimizationSettings
            {
                // Performance
                HighPerformancePowerPlan = OptPower.IsChecked ?? false,
                DisableMouseAcceleration = OptMouse.IsChecked ?? false,
                OptimizeVisualEffects = OptVisualEffects.IsChecked ?? false,
                OptimizeExplorer = OptExplorer.IsChecked ?? false,
                DisableStartupPrograms = OptStartup.IsChecked ?? false,
                OptimizePageFile = OptPageFile.IsChecked ?? false,

                // Privacy
                DisableTelemetry = OptTelemetry.IsChecked ?? false,
                DisableCortana = OptCortana.IsChecked ?? false,
                DisableAdvertisingId = OptAdvertising.IsChecked ?? false,
                DisableLocationTracking = OptLocation.IsChecked ?? false,
                DisableDiagnostics = OptDiagnostics.IsChecked ?? false,

                // Services
                DisablePrintSpooler = OptPrintSpooler.IsChecked ?? false,
                DisableFax = OptFax.IsChecked ?? false,
                DisableWindowsSearch = OptWindowsSearch.IsChecked ?? false,
                DisableSuperfetch = OptSuperfetch.IsChecked ?? false,
                SetWindowsUpdateManual = OptWindowsUpdate.IsChecked ?? false,

                // Gaming
                EnableGameMode = OptGameMode.IsChecked ?? false,
                DisableGameBar = OptGameBar.IsChecked ?? false,
                DisableGameDVR = OptGameDVR.IsChecked ?? false,
                EnableHardwareAcceleratedGPU = OptFullscreenOpt.IsChecked ?? false,

                // Cleanup
                CleanTempFiles = OptTempFiles.IsChecked ?? false,
                EmptyRecycleBin = OptRecycleBin.IsChecked ?? false,
                DeleteWindowsOld = OptWindowsOld.IsChecked ?? false,
                CleanDownloads = OptDownloads.IsChecked ?? false,

                // Advanced
                CreateRestorePoint = OptCreateRestore.IsChecked ?? false,
                DisableOneDrive = OptDisableOneDrive.IsChecked ?? false,
                DisableHibernation = OptDisableHibernation.IsChecked ?? false
            };

            WasApplied = true;
            Close();
        }

        private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}

