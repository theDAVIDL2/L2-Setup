using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using WindowsSetup.App.Models;
using WindowsSetup.App.Services;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App.Views
{
    public partial class CursorToolsWindow : Window
    {
        private readonly CursorToolsService _service;
        private readonly Logger _logger;

        public CursorToolsWindow()
        {
            try
            {
                InitializeComponent();
                _logger = new Logger(UpdateLog);
                _service = new CursorToolsService(_logger);
                
                Loaded += async (s, e) => await InitializeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing Cursor Tools:\n{ex.Message}\n\nDetails: {ex.StackTrace}", 
                    "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private async Task InitializeAsync()
        {
            try
            {
                await Task.Delay(100); // Small delay to ensure UI is fully loaded
                await RefreshStatus();
                await RefreshBackups();
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
                
                // Auto-scroll to bottom - safely
                if (LogOutput.IsLoaded)
                {
                    var scrollViewer = FindVisualParent<ScrollViewer>(LogOutput);
                    scrollViewer?.ScrollToEnd();
                }
            }
            catch (Exception ex)
            {
                // Fallback if UI access fails
                System.Diagnostics.Debug.WriteLine($"Log error: {ex.Message}");
            }
        }

        private async Task RefreshStatus()
        {
            try
            {
                var info = await Task.Run(() => _service.GetCursorInfo());

                Dispatcher.Invoke(() =>
                {
                    StatusInstalled.Text = info.IsInstalled 
                        ? "✅ Cursor is installed" 
                        : "❌ Cursor is not installed";
                    
                    StatusRunning.Text = info.IsRunning 
                        ? "🟢 Cursor is running" 
                        : "⚪ Cursor is not running";
                    
                    StatusWorkspaces.Text = $"📁 Workspaces: {info.WorkspaceCount}";
                    
                    var sizeMB = info.ConfigSize / (1024.0 * 1024.0);
                    StatusSize.Text = $"💾 Config size: {sizeMB:F2} MB";
                    
                    StatusLastModified.Text = info.LastModified.HasValue 
                        ? $"🕐 Last modified: {info.LastModified.Value:yyyy-MM-dd HH:mm}" 
                        : "🕐 Last modified: Never";

                    HeaderStatus.Text = info.IsInstalled 
                        ? $"✅ Cursor installed at: {info.ConfigPath}" 
                        : "❌ Cursor not found";
                });
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
                var backups = await Task.Run(() => _service.ListBackups());
                var backupItems = backups.Select(path =>
                {
                    var fileInfo = new FileInfo(path);
                    return new
                    {
                        FileName = Path.GetFileName(path),
                        FullPath = path,
                        Info = $"{fileInfo.LastWriteTime:yyyy-MM-dd HH:mm} • {fileInfo.Length / 1024.0:F1} KB"
                    };
                }).ToList();

                Dispatcher.Invoke(() =>
                {
                    BackupsList.ItemsSource = backupItems;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error refreshing backups: {ex.Message}");
            }
        }

        #region Event Handlers

        private async void RefreshStatus_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("Refreshing status...");
            await RefreshStatus();
            _logger.LogSuccess("Status refreshed");
        }

        private async void ResetTrial_Click(object sender, RoutedEventArgs e)
        {
            // Ask user which method they prefer
            var methodChoice = MessageBox.Show(
                "Choose MACHINE ID SPOOFING method:\n\n" +
                "🎯 YES = REAL SPOOFER (Recommended)\n" +
                "   • Changes Windows registry Machine GUIDs\n" +
                "   • Spoofs Cryptography IDs\n" +
                "   • Changes SQM Client ID\n" +
                "   • Deletes Cursor storage (regenerates with new IDs)\n" +
                "   • PERMANENT system changes!\n\n" +
                "📝 NO = JSON Method (Lighter)\n" +
                "   • Only modifies Cursor's storage.json\n" +
                "   • Changes 5 Cursor IDs\n" +
                "   • Doesn't touch Windows system\n" +
                "   • Keeps workspace history\n\n" +
                "⚠️ REAL SPOOFER changes your actual Windows Machine IDs!\n" +
                "💡 Most users want YES for proper spoofing",
                "Machine ID Spoofer",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning);

            if (methodChoice == MessageBoxResult.Cancel)
                return;

            bool useFullReset = methodChoice == MessageBoxResult.Yes;

            // Confirm the action
            var confirmMsg = useFullReset
                ? "⚠️ REAL MACHINE ID SPOOFER ⚠️\n\n" +
                  "This will PERMANENTLY change:\n" +
                  "✓ Windows Machine GUID (Registry)\n" +
                  "✓ Cryptography Machine GUID\n" +
                  "✓ SQM Client ID\n" +
                  "✓ Windows Update Client ID\n" +
                  "✓ Delete Cursor storage (regenerates)\n\n" +
                  "🔥 YOUR WINDOWS MACHINE IDs WILL BE CHANGED!\n" +
                  "💾 Backup will be created first\n\n" +
                  "🎯 After this, Cursor will generate NEW IDs\n" +
                  "    based on your SPOOFED system!\n\n" +
                  "⚠️ REQUIRES ADMINISTRATOR ACCESS ⚠️\n\n" +
                  "Continue with REAL spoofing?"
                : "📝 JSON METHOD (Lighter)\n\n" +
                  "This will only modify Cursor's storage.json:\n" +
                  "• Changes 5 Cursor machine IDs\n" +
                  "• Keeps Windows system unchanged\n" +
                  "• Keeps your settings & history\n" +
                  "• Backup will be created first\n\n" +
                  "Continue?";

            var confirm = MessageBox.Show(confirmMsg, "Confirm Reset", 
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                var success = await Task.Run(() => _service.ResetTrial(ChkAutoClose.IsChecked ?? true, useFullReset));
                
                if (success)
                {
                    var successMsg = useFullReset
                        ? "✅ MACHINE ID SPOOFING COMPLETE!\n\n" +
                          "🎯 Windows Machine IDs CHANGED:\n" +
                          "   ✓ Machine GUID (Registry)\n" +
                          "   ✓ Cryptography GUID\n" +
                          "   ✓ SQM Client ID\n" +
                          "   ✓ Windows Update ID\n\n" +
                          "🗑️ Cursor storage deleted\n\n" +
                          "🔄 When you start Cursor:\n" +
                          "   • It will detect NEW system IDs\n" +
                          "   • Generate fresh IDs automatically\n" +
                          "   • Based on SPOOFED system\n\n" +
                          "💡 Your Windows is now using different Machine IDs!"
                        : "✅ Cursor IDs changed (JSON Method)!\n\n" +
                          "All 5 Cursor identifiers regenerated.\n" +
                          "Windows system IDs unchanged.\n\n" +
                          "You can now restart Cursor.";

                    MessageBox.Show(successMsg, "Spoofing Complete", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    await RefreshStatus();
                }
                else
                {
                    MessageBox.Show(
                        "❌ Failed to reset machine IDs.\n\n" +
                        "Check the log for details.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private async void BackupSettings_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("Creating backup...");
            var backupPath = await _service.BackupSettings();
            
            if (backupPath != null)
            {
                MessageBox.Show(
                    $"✅ Backup created successfully!\n\n" +
                    $"Location: {backupPath}",
                    "Backup Created",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                
                await RefreshBackups();
            }
            else
            {
                MessageBox.Show(
                    "❌ Failed to create backup.\n\n" +
                    "Check the log for details.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void RestoreSettings_Click(object sender, RoutedEventArgs e)
        {
            if (BackupsList.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select a backup from the list below.",
                    "No Backup Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var selectedBackup = BackupsList.SelectedItem as dynamic;
            var backupPath = selectedBackup?.FullPath as string;

            if (string.IsNullOrEmpty(backupPath))
                return;

            var result = MessageBox.Show(
                $"This will restore Cursor settings from:\n\n" +
                $"{Path.GetFileName(backupPath)}\n\n" +
                $"⚠️ Current settings will be overwritten.\n" +
                $"⚠️ Cursor will be closed if it's running.\n\n" +
                $"Continue?",
                "Restore Settings",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var success = await _service.RestoreSettings(backupPath);
                
                if (success)
                {
                    MessageBox.Show(
                        "✅ Settings restored successfully!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    
                    await RefreshStatus();
                }
            }
        }

        private async void RestoreSpecificBackup_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var backupPath = button?.Tag as string;

            if (string.IsNullOrEmpty(backupPath))
                return;

            var result = MessageBox.Show(
                $"Restore from this backup?\n\n" +
                $"{Path.GetFileName(backupPath)}\n\n" +
                $"⚠️ Current settings will be overwritten.",
                "Restore Backup",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var success = await _service.RestoreSettings(backupPath);
                
                if (success)
                {
                    MessageBox.Show("✅ Settings restored successfully!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    await RefreshStatus();
                }
            }
        }

        private async void ClearCache_Click(object sender, RoutedEventArgs e)
        {
            var includeWorkspaces = ChkClearWorkspaces.IsChecked ?? false;
            
            var message = "This will clear Cursor's cache and temporary files.\n\n" +
                         "Items to be deleted:\n" +
                         "• Cache, CachedData\n" +
                         "• Code Cache, GPUCache\n" +
                         "• Session Storage, Local Storage\n" +
                         "• Blob Storage, WebStorage\n";

            if (includeWorkspaces)
            {
                message += "• Workspace Storage (recent workspaces)\n";
            }

            message += "\n⚠️ Cursor will be closed if it's running.\n\nContinue?";

            var result = MessageBox.Show(message, "Clear Cache", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var success = await _service.ClearCache(includeWorkspaces);
                
                if (success)
                {
                    MessageBox.Show(
                        "✅ Cache cleared successfully!\n\n" +
                        "Cursor will recreate these folders on next startup.",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    
                    await RefreshStatus();
                }
            }
        }

        private async void ViewMachineIds_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var info = _service.GetCursorInfo();
                
                if (!info.IsInstalled || !File.Exists(info.StorageJsonPath))
                {
                    MessageBox.Show(
                        "❌ Cursor storage.json not found!",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                var json = File.ReadAllText(info.StorageJsonPath);
                var config = JsonConvert.DeserializeObject<CursorConfig>(json);

                if (config == null)
                {
                    MessageBox.Show("❌ Failed to load configuration.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var message = "🔑 Current Machine IDs:\n\n" +
                             $"📱 Machine ID:\n{config.TelemetryMachineId}\n\n" +
                             $"🖥️ Mac Machine ID:\n{config.TelemetryMacMachineId}\n\n" +
                             $"📊 SQM ID:\n{config.TelemetrySqmId}\n\n" +
                             $"🆔 Device ID:\n{config.TelemetryDevDeviceId}\n\n" +
                             $"⚙️ Service Machine ID:\n{config.StorageServiceMachineId}";

                MessageBox.Show(message, "Machine IDs", 
                    MessageBoxButton.OK, MessageBoxImage.Information);

                _logger.LogInfo("Machine IDs displayed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error viewing machine IDs: {ex.Message}");
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenConfigFolder_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("Opening config folder...");
            _service.OpenConfigFolder();
        }

        private async void CloseCursor_Click(object sender, RoutedEventArgs e)
        {
            if (!_service.IsCursorRunning())
            {
                MessageBox.Show("Cursor is not running.", "Info", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                "Close all Cursor processes?",
                "Close Cursor",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var success = await _service.CloseCursor();
                
                if (success)
                {
                    MessageBox.Show("✅ Cursor closed successfully!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    await RefreshStatus();
                }
            }
        }

        private async void RefreshBackups_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("Refreshing backups list...");
            await RefreshBackups();
            _logger.LogSuccess("Backups list refreshed");
        }

        private async void DisableAutoUpdate_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "This will disable Cursor's automatic updates.\n\n" +
                "⚠️ You'll need to update manually in the future.\n" +
                "⚠️ Cursor will be closed if it's running.\n\n" +
                "Continue?",
                "Disable Auto-Update",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var success = await _service.DisableAutoUpdate();
                
                if (success)
                {
                    MessageBox.Show(
                        "✅ Auto-update disabled successfully!\n\n" +
                        "Cursor will no longer automatically update.",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }

        private async void TotallyResetCursor_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "⚠️ WARNING: This will COMPLETELY DELETE all Cursor data!\n\n" +
                "This includes:\n" +
                "• All settings and preferences\n" +
                "• Extensions and configurations\n" +
                "• Workspace history\n" +
                "• Machine IDs (fresh trial)\n" +
                "• Cache and temporary files\n\n" +
                "💾 A backup will be created automatically.\n\n" +
                "Are you ABSOLUTELY SURE?",
                "⚠️ Total Reset Cursor",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Double confirmation
                var confirm = MessageBox.Show(
                    "This action cannot be undone!\n\n" +
                    "Click YES to proceed with TOTAL RESET.",
                    "Final Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Stop);

                if (confirm == MessageBoxResult.Yes)
                {
                    var success = await _service.TotallyResetCursor();
                    
                    if (success)
                    {
                        MessageBox.Show(
                            "✅ Cursor totally reset!\n\n" +
                            "All data has been deleted.\n" +
                            "Cursor will create fresh configuration on next startup.",
                            "Reset Complete",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        
                        await RefreshStatus();
                    }
                }
            }
        }

        private void ShowFullConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogInfo("Loading full configuration...");
                var config = _service.GetFullConfig();
                
                // Create a window to display the config
                var configWindow = new Window
                {
                    Title = "Cursor Full Configuration",
                    Width = 700,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new ScrollViewer
                    {
                        Content = new TextBox
                        {
                            Text = config,
                            IsReadOnly = true,
                            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                            FontSize = 12,
                            TextWrapping = TextWrapping.Wrap,
                            Padding = new Thickness(16),
                            BorderThickness = new Thickness(0),
                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                        }
                    }
                };
                
                configWindow.ShowDialog();
                _logger.LogSuccess("Configuration displayed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error showing config: {ex.Message}");
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Helper Methods

        private static T? FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);
            
            if (parentObject == null) return null;
            
            if (parentObject is T parent)
                return parent;
            
            return FindVisualParent<T>(parentObject);
        }

        #endregion
    }
}

