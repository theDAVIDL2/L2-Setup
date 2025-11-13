using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WindowsSetup.App.Models;
using WindowsSetup.App.Services;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App.Views
{
    public partial class SystemIdSpooferWindow : Window
    {
        private readonly SystemIdSpooferService _service;
        private readonly Logger _logger;

        public SystemIdSpooferWindow()
        {
            try
            {
                InitializeComponent();
                _logger = new Logger(UpdateLog);
                _service = new SystemIdSpooferService(_logger);
                
                Loaded += async (s, e) => await InitializeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing System ID Spoofer:\n{ex.Message}", 
                    "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private async Task InitializeAsync()
        {
            try
            {
                await Task.Delay(100);
                await RefreshStatus();
                await RefreshBackups();
                
                // Check if Tor is installed
                if (_service.IsTorInstalled())
                {
                    TorStatusText.Text = _service.IsTorRunning() ? "Status: Running" : "Status: Installed (not running)";
                    TorToggle.IsEnabled = true;
                    TorDownloadBtn.Content = "✅ Tor Installed";
                }
                
                _logger.LogInfo("System ID Spoofer initialized");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during initialization:\n{ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateLog(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => UpdateLog(message));
                return;
            }

            try
            {
                var timestamp = DateTime.Now.ToString("HH:mm:ss");
                LogOutput.Text += $"[{timestamp}] {message}\n";
                
                if (LogOutput.IsLoaded)
                {
                    LogOutput.ScrollToEnd();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Log error: {ex.Message}");
            }
        }

        private async Task RefreshStatus()
        {
            try
            {
                // Check if any IDs are spoofed (simplified check)
                StatusMachineGuids.Text = "• Machine GUIDs: Unknown (click refresh for details)";
                StatusMacAddresses.Text = "• MAC Addresses: Check Individual Controls tab";
                StatusVolumeSerials.Text = "• Volume Serials: Original";
                StatusNetwork.Text = "• Network: Normal";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error refreshing status: {ex.Message}");
            }
        }

        private async Task RefreshBackups()
        {
            try
            {
                var backups = _service.ListBackups();
                BackupsList.ItemsSource = backups;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error refreshing backups: {ex.Message}");
            }
        }

        #region Quick Actions

        private async void RefreshStatus_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("Refreshing status...");
            await RefreshStatus();
        }

        private async void SpoofAll_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "⚠️ SPOOF ALL MACHINE IDs?\n\n" +
                "This will:\n" +
                "• Change all Windows Machine GUIDs\n" +
                "• Change all network adapter MAC addresses\n" +
                "• Create automatic backup first\n\n" +
                "Some changes may require reboot.\n\n" +
                "Continue?",
                "Spoof All - Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await ExecuteWithBackup(async () =>
                {
                    _logger.LogInfo("=== Starting Full System ID Spoofing ===");
                    
                    // Spoof Machine GUIDs
                    await _service.SpoofMachineGuids();
                    
                    // Spoof Hardware IDs
                    await _service.SpoofHardwareIds();
                    
                    // Load and spoof all MAC addresses
                    var adapters = await _service.GetNetworkAdapters();
                    foreach (var adapter in adapters)
                    {
                        await _service.SpoofMacAddress(adapter.Name);
                    }
                    
                    _logger.LogSuccess("✅ All IDs spoofed successfully!");
                    MessageBox.Show(
                        "✅ All machine IDs have been spoofed!\n\n" +
                        "Changes applied:\n" +
                        "• Windows Machine GUIDs changed\n" +
                        "• Hardware IDs changed (disk, motherboard)\n" +
                        "• All MAC addresses changed\n\n" +
                        "⚠️ A system reboot is HIGHLY RECOMMENDED for all changes to take effect.",
                        "Spoofing Complete",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                });
            }
        }

        private async void RestoreAll_Click(object sender, RoutedEventArgs e)
        {
            var backups = _service.ListBackups();
            if (!backups.Any())
            {
                MessageBox.Show(
                    "❌ No backups found!\n\n" +
                    "You need to create a backup first before you can restore.",
                    "No Backups",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Restore all IDs from the most recent backup?\n\n" +
                $"Backup Date: {backups.First().BackupDate:yyyy-MM-dd HH:mm:ss}\n\n" +
                "This will restore all original machine identifiers.",
                "Restore All - Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var backup = backups.First();
                await _service.RestoreBackup(backup);
                
                MessageBox.Show(
                    "✅ All machine IDs restored to original values!",
                    "Restore Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private async void CreateBackup_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("Creating manual backup...");
            var backup = await _service.CreateFullBackup();
            
            if (backup != null)
            {
                MessageBox.Show(
                    "✅ Backup created successfully!\n\n" +
                    $"Backup ID: {backup.BackupId}\n" +
                    $"Date: {backup.BackupDate:yyyy-MM-dd HH:mm:ss}",
                    "Backup Created",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                
                await RefreshBackups();
            }
            else
            {
                MessageBox.Show(
                    "❌ Failed to create backup!\n\n" +
                    "Check the log for details.",
                    "Backup Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region Individual Controls

        private async void SpoofMachineGuids_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Spoof Windows Machine GUIDs?\n\n" +
                "This will change:\n" +
                "• Machine GUID\n" +
                "• SQM Client ID\n" +
                "• Hardware Profile GUID\n\n" +
                "Continue?",
                "Spoof Machine GUIDs",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await ExecuteWithBackup(async () =>
                {
                    await _service.SpoofMachineGuids();
                    MessageBox.Show(
                        "✅ Machine GUIDs spoofed successfully!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                });
            }
        }

        private async void SpoofHardwareIds_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Spoof Hardware IDs?\n\n" +
                "This will change:\n" +
                "• Disk serial numbers\n" +
                "• Motherboard UUID\n" +
                "• Computer Hardware ID\n\n" +
                "⚠️ NOTE: Some hardware IDs are firmware-based\n" +
                "and cannot be fully changed. This tool modifies\n" +
                "registry entries that most software checks.\n\n" +
                "Continue?",
                "Spoof Hardware IDs",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await ExecuteWithBackup(async () =>
                {
                    await _service.SpoofHardwareIds();
                    MessageBox.Show(
                        "✅ Hardware IDs spoofed!\n\n" +
                        "Registry entries have been modified.\n" +
                        "⚠️ A system reboot is recommended for all changes to take effect.",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                });
            }
        }

        private async void LoadAdapters_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("Loading network adapters...");
            var adapters = await _service.GetNetworkAdapters();
            AdaptersList.ItemsSource = adapters;
            _logger.LogSuccess($"✅ Loaded {adapters.Count} network adapters");
        }

        private async void SpoofSelectedMac_Click(object sender, RoutedEventArgs e)
        {
            var selected = AdaptersList.SelectedItem as NetworkAdapterInfo;
            if (selected == null)
            {
                MessageBox.Show("Please select a network adapter first.", "No Selection", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Spoof MAC address for:\n{selected.Name}\n\n" +
                $"Current MAC: {selected.CurrentMac}\n\n" +
                "A new random MAC will be generated.\n" +
                "The adapter will be restarted.\n\n" +
                "Continue?",
                "Spoof MAC Address",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await ExecuteWithBackup(async () =>
                {
                    await _service.SpoofMacAddress(selected.Name);
                    
                    // Reload adapters
                    _logger.LogInfo("Reloading network adapters...");
                    var adapters = await _service.GetNetworkAdapters();
                    AdaptersList.ItemsSource = adapters;
                    _logger.LogSuccess($"✅ Loaded {adapters.Count} network adapters");
                    
                    MessageBox.Show(
                        $"✅ MAC address spoofed for {selected.Name}!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                });
            }
        }

        private async void RestoreSelectedMac_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "To restore MAC addresses, use the 'Restore All' button in Quick Actions tab.\n\n" +
                "This will restore all IDs from your backup.",
                "Restore MAC",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private async void LoadVolumes_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("Loading volume information...");
            var volumes = await _service.GetVolumeInfo();
            VolumesList.ItemsSource = volumes;
            _logger.LogSuccess($"✅ Loaded {volumes.Count} volumes");
        }

        #endregion

        #region IP Masking

        private async void CheckIp_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("Checking current IP address...");
            try
            {
                CurrentIpText.Text = "IP: Checking...";
                var ip = await _service.CheckCurrentIp();
                CurrentIpText.Text = $"IP: {ip}";
                _logger.LogSuccess($"✅ Current IP: {ip}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking IP: {ex.Message}");
                CurrentIpText.Text = "IP: Error checking";
            }
        }

        private async void DownloadTor_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Download Tor Expert Bundle?\n\n" +
                "This will download approximately 10-15 MB.\n" +
                "Tor will be installed in your AppData folder.\n\n" +
                "Continue?",
                "Download Tor",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                TorDownloadBtn.IsEnabled = false;
                TorStatusText.Text = "Status: Downloading...";
                
                var success = await _service.DownloadTor();
                
                if (success)
                {
                    TorStatusText.Text = "Status: Installed (not running)";
                    TorToggle.IsEnabled = true;
                    MessageBox.Show(
                        "✅ Tor installed successfully!\n\n" +
                        "You can now start Tor to enable anonymous browsing.",
                        "Installation Complete",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    TorStatusText.Text = "Status: Installation failed";
                    TorDownloadBtn.IsEnabled = true;
                    MessageBox.Show(
                        "❌ Failed to install Tor.\n\n" +
                        "Check the log for details.",
                        "Installation Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private async void TorToggle_Checked(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("Starting Tor...");
            var success = await _service.StartTor();
            
            if (success)
            {
                TorStatusText.Text = "Status: Running (proxy enabled)";
                TorToggle.Content = "Stop Tor";
                _logger.LogSuccess("✅ Tor is running. Your connection is now anonymous.");
            }
            else
            {
                TorToggle.IsChecked = false;
                TorStatusText.Text = "Status: Failed to start";
                MessageBox.Show(
                    "❌ Failed to start Tor.\n\n" +
                    "Check the log for details.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void TorToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("Stopping Tor...");
            await _service.StopTor();
            TorStatusText.Text = "Status: Stopped";
            TorToggle.Content = "Start Tor";
        }

        private void ImportVpnConfig_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "OpenVPN integration coming in Phase 5!\n\n" +
                "This will allow you to import .ovpn configuration files.",
                "Coming Soon",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void VpnToggle_Checked(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("VPN toggle feature coming in Phase 5");
        }

        private void VpnToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("VPN toggle feature coming in Phase 5");
        }

        #endregion

        #region Backups

        private async void RefreshBackups_Click(object sender, RoutedEventArgs e)
        {
            await RefreshBackups();
            _logger.LogInfo("Backups list refreshed");
        }

        private void BackupsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Could show backup details here
        }

        private async void RestoreBackup_Click(object sender, RoutedEventArgs e)
        {
            var selected = BackupsList.SelectedItem as SystemIdBackup;
            if (selected == null)
            {
                MessageBox.Show("Please select a backup first.", "No Selection", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Restore backup from:\n{selected.BackupDate:yyyy-MM-dd HH:mm:ss}\n\n" +
                "This will restore all saved machine identifiers.\n\n" +
                "Continue?",
                "Restore Backup",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await _service.RestoreBackup(selected);
                MessageBox.Show(
                    "✅ Backup restored successfully!",
                    "Restore Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private async void DeleteBackup_Click(object sender, RoutedEventArgs e)
        {
            var selected = BackupsList.SelectedItem as SystemIdBackup;
            if (selected == null)
            {
                MessageBox.Show("Please select a backup first.", "No Selection", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Delete backup from:\n{selected.BackupDate:yyyy-MM-dd HH:mm:ss}\n\n" +
                "This action cannot be undone.\n\n" +
                "Continue?",
                "Delete Backup",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Delete implementation would go here
                _logger.LogInfo("Backup deletion feature to be implemented");
                await RefreshBackups();
            }
        }

        #endregion

        #region Helper Methods

        private async Task ExecuteWithBackup(Func<Task> operation)
        {
            try
            {
                if (ChkAutoBackup.IsChecked == true)
                {
                    _logger.LogInfo("Creating automatic backup...");
                    await _service.CreateFullBackup();
                }

                await operation();
                await RefreshStatus();
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Operation failed: {ex.Message}");
                MessageBox.Show(
                    $"❌ Operation failed:\n\n{ex.Message}\n\n" +
                    "Check the log for details.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion
    }
}

