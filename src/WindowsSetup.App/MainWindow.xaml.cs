using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using WindowsSetup.App.Services;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App
{
    public partial class MainWindow : Window
    {
        private readonly BrowserBackupService _browserService;
        private readonly ToolInstallerService _toolInstaller;
        private readonly WindowsOptimizerService _optimizer;
        private readonly WindowsActivationService _activation;
        private readonly Logger _logger;

        public MainWindow()
        {
            InitializeComponent();

            _logger = new Logger(LogMessage);
            _browserService = new BrowserBackupService(_logger);
            _toolInstaller = new ToolInstallerService(_logger, UpdateProgress);
            _optimizer = new WindowsOptimizerService(_logger);
            _activation = new WindowsActivationService(_logger);

            _logger.LogInfo("Windows Post-Format Setup Tool initialized");
            _logger.LogInfo($"Running as Administrator: {AdminHelper.IsAdministrator()}");
        }

        private void LogMessage(string message, LogLevel level)
        {
            Dispatcher.Invoke(() =>
            {
                var timestamp = DateTime.Now.ToString("HH:mm:ss");
                var color = level switch
                {
                    LogLevel.Info => "#2196F3",
                    LogLevel.Success => "#4CAF50",
                    LogLevel.Warning => "#FF9800",
                    LogLevel.Error => "#F44336",
                    _ => "#000000"
                };

                LogTextBox.AppendText($"[{timestamp}] {message}\n");
                LogTextBox.ScrollToEnd();
            });
        }

        private void UpdateProgress(int percentage, string status)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar.Value = percentage;
                ProgressText.Text = $"Progress: {percentage}%";
                StatusText.Text = status;
            });
        }

        private async void BackupBraveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogInfo("Starting Brave profile backup...");
                UpdateProgress(0, "Preparing backup...");

                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = $"Brave_Backup_{DateTime.Now:yyyy-MM-dd_HHmmss}",
                    DefaultExt = ".zip",
                    Filter = "Backup files (*.zip)|*.zip"
                };

                if (dialog.ShowDialog() == true)
                {
                    var result = await _browserService.BackupBraveProfile(dialog.FileName);
                    
                    if (result.Success)
                    {
                        _logger.LogSuccess($"Backup completed successfully! Saved to: {dialog.FileName}");
                        MessageBox.Show($"Backup completed!\n\nSize: {result.SizeInMB:F2} MB\nExtensions: {result.ExtensionsCount}\nLocation: {dialog.FileName}",
                            "Backup Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        _logger.LogError($"Backup failed: {result.ErrorMessage}");
                        MessageBox.Show($"Backup failed:\n{result.ErrorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                UpdateProgress(100, "Ready");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backup error: {ex.Message}");
                MessageBox.Show($"Error during backup:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RestoreBraveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    DefaultExt = ".zip",
                    Filter = "Backup files (*.zip)|*.zip"
                };

                if (dialog.ShowDialog() == true)
                {
                    _logger.LogInfo("Starting Brave profile restore...");
                    UpdateProgress(0, "Restoring backup...");

                    var result = await _browserService.RestoreBraveProfile(dialog.FileName);
                    
                    if (result.Success)
                    {
                        _logger.LogSuccess("Restore completed successfully!");
                        MessageBox.Show("Profile restored successfully!\n\nYou can now open Brave Browser.",
                            "Restore Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        _logger.LogError($"Restore failed: {result.ErrorMessage}");
                        MessageBox.Show($"Restore failed:\n{result.ErrorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                UpdateProgress(100, "Ready");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Restore error: {ex.Message}");
                MessageBox.Show($"Error during restore:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SetBraveDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogInfo("Setting Brave as default browser...");
                await _browserService.SetBraveAsDefaultBrowser();
                _logger.LogSuccess("Brave set as default browser!");
                MessageBox.Show("Brave has been set as your default browser!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting default browser: {ex.Message}");
                MessageBox.Show($"Error:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void InstallEssentialsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogInfo("Starting essential tools installation...");
                await _toolInstaller.InstallEssentialTools();
                _logger.LogSuccess("Essential tools installed successfully!");
                MessageBox.Show("Essential tools have been installed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Installation error: {ex.Message}");
                MessageBox.Show($"Error:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void InstallAllButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "This will install ALL tools and may take 30-60 minutes.\n\nDo you want to continue?",
                    "Install All Tools",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _logger.LogInfo("Starting full installation...");
                    await _toolInstaller.InstallAllTools();
                    _logger.LogSuccess("All tools installed successfully!");
                    MessageBox.Show("All tools have been installed!\n\nA restart is recommended.",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Installation error: {ex.Message}");
                MessageBox.Show($"Error:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CustomInstallButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Custom selection dialog coming soon!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void OptimizeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "This will apply Windows optimizations.\n\nA system restore point will be created first.\n\nContinue?",
                    "Apply Optimizations",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _logger.LogInfo("Starting Windows optimization...");
                    await _optimizer.ApplyAllOptimizations();
                    _logger.LogSuccess("Optimizations applied successfully!");
                    MessageBox.Show("Optimizations have been applied!\n\nA restart is recommended.",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Optimization error: {ex.Message}");
                MessageBox.Show($"Error:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AdvancedOptimizeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "This will run Chris Titus Tech's advanced optimization script.\n\nThis makes significant system changes.\n\nContinue?",
                    "Advanced Optimization",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _logger.LogInfo("Starting advanced optimization (CTT)...");
                    await _optimizer.RunChrisTitusScript();
                    _logger.LogSuccess("Advanced optimization completed!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Advanced optimization error: {ex.Message}");
                MessageBox.Show($"Error:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ActivateWindowsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "This will activate Windows using Microsoft Activation Scripts.\n\nThis is for users with valid licenses only.\n\nContinue?",
                    "Activate Windows",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _logger.LogInfo("Starting Windows activation...");
                    await _activation.ActivateWindowsAutomatic();
                    _logger.LogSuccess("Windows activation process completed!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Activation error: {ex.Message}");
                MessageBox.Show($"Error:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CheckActivationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogInfo("Checking Windows activation status...");
                var status = await _activation.CheckWindowsActivationStatus();
                
                var message = status.IsActivated
                    ? $"Windows is activated!\n\nLicense: {status.LicenseType}"
                    : "Windows is not activated.";

                MessageBox.Show(message, "Activation Status", MessageBoxButton.OK,
                    status.IsActivated ? MessageBoxImage.Information : MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking activation: {ex.Message}");
                MessageBox.Show($"Error:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            LogTextBox.Clear();
            _logger.LogInfo("Log cleared");
        }

        private void ExportLogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = $"WindowsSetup_Log_{DateTime.Now:yyyy-MM-dd_HHmmss}",
                    DefaultExt = ".txt",
                    Filter = "Text files (*.txt)|*.txt"
                };

                if (dialog.ShowDialog() == true)
                {
                    File.WriteAllText(dialog.FileName, LogTextBox.Text);
                    _logger.LogSuccess($"Log exported to: {dialog.FileName}");
                    MessageBox.Show($"Log exported successfully!\n\n{dialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting log:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public enum LogLevel
    {
        Info,
        Success,
        Warning,
        Error
    }
}

