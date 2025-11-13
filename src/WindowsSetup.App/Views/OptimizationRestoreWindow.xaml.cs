using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WindowsSetup.App.Models;
using WindowsSetup.App.Services;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App.Views
{
    public partial class OptimizationRestoreWindow : Window
    {
        private readonly Logger _logger;
        private readonly OptimizationBackupService _backupService;
        private string? _selectedBackupPath;

        public OptimizationRestoreWindow()
        {
            InitializeComponent();
            _logger = new Logger(UpdateLog);
            _backupService = new OptimizationBackupService(_logger);
            
            Loaded += async (s, e) => await LoadBackups();
        }

        private void UpdateLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                var timestamp = DateTime.Now.ToString("HH:mm:ss");
                LogOutput.Text += $"[{timestamp}] {message}\n";
            });
        }

        private async Task LoadBackups()
        {
            try
            {
                _logger.LogInfo("Loading available backups...");
                var backups = await Task.Run(() => _backupService.ListBackupsDetailed());

                if (backups.Count == 0)
                {
                    NoBackupsMessage.Visibility = Visibility.Visible;
                    BackupsList.Visibility = Visibility.Collapsed;
                    _logger.LogWarning("No backups found");
                    return;
                }

                var backupItems = backups.Select(item => new
                {
                    FilePath = item.FilePath,
                    Description = item.Backup.Description ?? "Windows Optimization Backup",
                    Timestamp = item.Backup.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    Details = $"Backup ID: {item.Backup.BackupId.Substring(0, 8)}...",
                    RegistryCount = $"{item.Backup.RegistryChanges.Count} registry entries",
                    ServiceCount = $"{item.Backup.ServiceChanges.Count} services"
                }).ToList();

                BackupsList.ItemsSource = backupItems;
                _logger.LogSuccess($"Found {backupItems.Count} backup(s)");
                NoBackupsMessage.Visibility = Visibility.Collapsed;
                BackupsList.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading backups: {ex.Message}");
                MessageBox.Show($"Failed to load backups:\n{ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackupsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = BackupsList.SelectedItem;
            if (selected != null)
            {
                // Get the FilePath property from the anonymous object
                var filePathProp = selected.GetType().GetProperty("FilePath");
                if (filePathProp != null)
                {
                    _selectedBackupPath = filePathProp.GetValue(selected) as string;
                    RestoreButton.IsEnabled = !string.IsNullOrEmpty(_selectedBackupPath);
                    if (!string.IsNullOrEmpty(_selectedBackupPath))
                    {
                        _logger.LogInfo($"Selected backup: {Path.GetFileName(_selectedBackupPath)}");
                    }
                }
            }
            else
            {
                _selectedBackupPath = null;
                RestoreButton.IsEnabled = false;
            }
        }

        private async void RestoreBackup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedBackupPath))
            {
                MessageBox.Show("Please select a backup to restore.", "No Backup Selected", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                "This will restore your system settings from the selected backup.\n\n" +
                "Registry values and services will be reverted to their previous state.\n\n" +
                "⚠️ This may require administrator privileges.\n" +
                "⚠️ A system restart may be recommended.\n\n" +
                "Continue?",
                "Confirm Restore",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                RestoreButton.IsEnabled = false;
                
                try
                {
                    _logger.LogInfo("Starting restore process...");
                    var success = await _backupService.RestoreFromBackup(_selectedBackupPath);

                    if (success)
                    {
                        MessageBox.Show(
                            "✅ Settings restored successfully!\n\n" +
                            "Your system has been reverted to the previous state.\n\n" +
                            "⚠️ A system restart is HIGHLY RECOMMENDED for changes to take full effect.",
                            "Restore Complete",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        var restart = MessageBox.Show(
                            "Would you like to restart your computer now?",
                            "Restart Computer",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (restart == MessageBoxResult.Yes)
                        {
                            System.Diagnostics.Process.Start("shutdown", "/r /t 10 /c \"System will restart in 10 seconds to apply changes\"");
                            Application.Current.Shutdown();
                        }
                        
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(
                            "❌ Restore failed!\n\n" +
                            "Some settings may not have been restored correctly.\n" +
                            "Check the log for details.",
                            "Restore Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        
                        RestoreButton.IsEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Restore error: {ex.Message}");
                    MessageBox.Show($"Restore failed:\n{ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    RestoreButton.IsEnabled = true;
                }
            }
            else
            {
                RestoreButton.IsEnabled = true;
            }
        }

        private async void RefreshBackups_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("Refreshing backup list...");
            await LoadBackups();
        }
    }
}

