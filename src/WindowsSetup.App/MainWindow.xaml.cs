using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WindowsSetup.App.Models;
using WindowsSetup.App.Services;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App
{
    public partial class MainWindow : Window
    {
        private readonly Logger _logger;
        private readonly ToolInstallerService? _toolInstaller;
        private readonly BrowserBackupService? _browserBackup;
        private readonly WindowsOptimizerService? _windowsOptimizer;
        private readonly WindowsActivationService? _windowsActivation;
        private readonly RuntimeInstallerService? _runtimeInstaller;
        private readonly List<ToolCheckBox> _toolCheckBoxes;

        private class ToolCheckBox
        {
            public CheckBox? CheckBox { get; set; }
            public ToolDefinition? Tool { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();

            _logger = new Logger(Log);
            _toolCheckBoxes = new List<ToolCheckBox>();

            try
            {
                _toolInstaller = new ToolInstallerService(_logger, UpdateProgress);
                _browserBackup = new BrowserBackupService(_logger);
                _windowsOptimizer = new WindowsOptimizerService(_logger);
                _windowsActivation = new WindowsActivationService(_logger);
                _runtimeInstaller = new RuntimeInstallerService(_logger, UpdateProgress);

                InitializeCustomToolsList();

                _logger.LogSuccess("Application initialized successfully!");
                _logger.LogInfo("Select an option to begin...");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Initialization error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeCustomToolsList()
        {
            try
            {
                if (_toolInstaller == null) return;

                var allTools = _toolInstaller.GetAllToolsList();
                
                // Group tools by category
                var categories = new Dictionary<string, List<ToolDefinition>>
                {
                    ["Essentials"] = allTools.Where(t => t.Essential).ToList(),
                    ["IDEs & Editors"] = allTools.Where(t => t.Priority >= 10 && t.Priority < 20).ToList(),
                    ["Browsers"] = allTools.Where(t => t.Priority >= 20 && t.Priority < 28).ToList(),
                    ["Languages"] = allTools.Where(t => t.Priority >= 28 && t.Priority < 40).ToList(),
                    ["Package Managers"] = allTools.Where(t => t.Priority >= 40 && t.Priority < 50).ToList(),
                    ["Gaming & Communication"] = allTools.Where(t => t.Priority >= 50 && t.Priority < 60).ToList(),
                    ["Utilities"] = allTools.Where(t => t.Priority >= 60 && t.Priority < 70).ToList(),
                    ["System Tools"] = allTools.Where(t => t.Priority >= 70 && t.Priority < 75).ToList(),
                    ["Development Tools"] = allTools.Where(t => t.Priority >= 75 && t.Priority < 100).ToList(),
                    ["Runtimes"] = allTools.Where(t => t.Priority >= 100).ToList()
                };

                foreach (var category in categories)
                {
                    if (category.Value.Any())
                    {
                        // Category header
                        var header = new TextBlock
                        {
                            Text = category.Key,
                            FontSize = 16,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(0, 10, 0, 5)
                        };
                        CustomToolsList.Children.Add(header);

                        // Tools in category
                        foreach (var tool in category.Value.OrderBy(t => t.Priority))
                        {
                            var checkBox = new CheckBox
                            {
                                Content = tool.Name,
                                Margin = new Thickness(20, 2, 0, 2),
                                IsChecked = tool.Essential
                            };

                            _toolCheckBoxes.Add(new ToolCheckBox
                            {
                                CheckBox = checkBox,
                                Tool = tool
                            });

                            CustomToolsList.Children.Add(checkBox);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error initializing tools list: {ex.Message}");
            }
        }

        private void Log(string message)
        {
            Dispatcher.Invoke(() =>
            {
                LogTextBox.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
                LogTextBox.ScrollToEnd();
            });
        }

        private void UpdateProgress(int percentage, string message)
        {
            Dispatcher.Invoke(() =>
            {
                Log($"[{percentage}%] {message}");
            });
        }

        // Quick Setup Handlers
        private async void InstallEssentials_Click(object sender, RoutedEventArgs e)
        {
            if (_toolInstaller == null) return;
            await ExecuteSafe(async () =>
            {
                _logger.LogInfo("Starting essential tools installation...");
                await _toolInstaller.InstallEssentialTools();
                _logger.LogSuccess("Essential tools installation complete!");
                MessageBox.Show("Essential tools installed successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private async void InstallAll_Click(object sender, RoutedEventArgs e)
        {
            if (_toolInstaller == null) return;

            var result = MessageBox.Show(
                "This will install 44+ tools.\nContinue?",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await ExecuteSafe(async () =>
                {
                    _logger.LogInfo("Starting full installation...");
                    
                    // Install tools
                    await _toolInstaller.InstallAllTools();
                    
                    // Install ALL runtimes (All-in-One)
                    if (_runtimeInstaller != null)
                    {
                        _logger.LogInfo("Installing ALL runtimes (All-in-One)...");
                        await _runtimeInstaller.InstallAllRuntimes();
                    }
                    
                    // Install GPU drivers
                    await _toolInstaller.InstallGPUDrivers();
                    
                    _logger.LogSuccess("Full installation complete!");
                    MessageBox.Show("All tools and runtimes installed successfully!\nYour system is fully configured!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
        }

        private async void InstallGPUDrivers_Click(object sender, RoutedEventArgs e)
        {
            if (_toolInstaller == null) return;
            await ExecuteSafe(async () =>
            {
                _logger.LogInfo("Starting GPU driver installation...");
                await _toolInstaller.InstallGPUDrivers();
                _logger.LogSuccess("GPU driver installation complete!");
            });
        }

        private async void InstallRuntimes_Click(object sender, RoutedEventArgs e)
        {
            if (_runtimeInstaller == null) return;

            var result = MessageBox.Show(
                "This will install 30+ runtimes including:\n\n" +
                "• VC++ Redistributables (2005-2022)\n" +
                "• .NET Framework (3.5-4.8.1)\n" +
                "• .NET Runtimes (5.0-8.0)\n" +
                "• DirectX, XNA, OpenAL\n" +
                "• Java 8 & 21\n\n" +
                "Continue?",
                "Install All Runtimes", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await ExecuteSafe(async () =>
                {
                    _logger.LogInfo("Starting All-in-One runtimes installation...");
                    await _runtimeInstaller.InstallAllRuntimes();
                    _logger.LogSuccess("All runtimes installed successfully!");
                    MessageBox.Show("All runtimes installed!\nYour system is now fully compatible with all applications and games!", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
        }

        // Custom Selection Handlers
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _toolCheckBoxes)
            {
                if (item.CheckBox != null)
                    item.CheckBox.IsChecked = true;
            }
            _logger.LogInfo("All tools selected");
        }

        private void DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _toolCheckBoxes)
            {
                if (item.CheckBox != null)
                    item.CheckBox.IsChecked = false;
            }
            _logger.LogInfo("All tools deselected");
        }

        private void SelectEssentials_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _toolCheckBoxes)
            {
                if (item.CheckBox != null && item.Tool != null)
                    item.CheckBox.IsChecked = item.Tool.Essential;
            }
            _logger.LogInfo("Essential tools selected");
        }

        private async void InstallSelected_Click(object sender, RoutedEventArgs e)
        {
            if (_toolInstaller == null) return;

            var selectedTools = _toolCheckBoxes
                .Where(t => t.CheckBox?.IsChecked == true && t.Tool != null)
                .Select(t => t.Tool!)
                .ToList();

            if (!selectedTools.Any())
            {
                MessageBox.Show("Please select at least one tool to install.", 
                    "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Install {selectedTools.Count} selected tools?",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await ExecuteSafe(async () =>
                {
                    _logger.LogInfo($"Installing {selectedTools.Count} selected tools...");
                    await _toolInstaller.InstallCustomTools(selectedTools);
                    _logger.LogSuccess("Selected tools installation complete!");
                    MessageBox.Show("Selected tools installed successfully!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
        }

        // Browser Handlers
        private async void BackupBrave_Click(object sender, RoutedEventArgs e)
        {
            if (_browserBackup == null) return;

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"BraveBackup_{DateTime.Now:yyyyMMdd_HHmmss}",
                DefaultExt = ".zip",
                Filter = "ZIP files (*.zip)|*.zip"
            };

            if (dialog.ShowDialog() == true)
            {
                await ExecuteSafe(async () =>
                {
                    _logger.LogInfo("Starting Brave profile backup...");
                    var result = await _browserBackup.BackupBraveProfile(dialog.FileName);
                    
                    if (result.Success)
                    {
                        _logger.LogSuccess($"Backup complete! Size: {result.SizeInMB:F2} MB");
                        MessageBox.Show($"Backup successful!\nLocation: {result.BackupPath}", 
                            "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        throw new Exception(result.ErrorMessage);
                    }
                });
            }
        }

        private async void RestoreBrave_Click(object sender, RoutedEventArgs e)
        {
            if (_browserBackup == null) return;

            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".zip",
                Filter = "ZIP files (*.zip)|*.zip"
            };

            if (dialog.ShowDialog() == true)
            {
                await ExecuteSafe(async () =>
                {
                    _logger.LogInfo("Starting Brave profile restore...");
                    var result = await _browserBackup.RestoreBraveProfile(dialog.FileName);
                    
                    if (result.Success)
                    {
                        _logger.LogSuccess("Restore complete!");
                        MessageBox.Show("Profile restored successfully!", "Success", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        throw new Exception(result.ErrorMessage ?? "Restore failed");
                    }
                });
            }
        }

        private async void SetBraveDefault_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteSafe(async () =>
            {
                _logger.LogInfo("Setting Brave as default browser...");
                await Task.Run(() =>
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "ms-settings:defaultapps",
                        UseShellExecute = true
                    });
                });
                _logger.LogSuccess("Please select Brave in the settings window.");
                MessageBox.Show("Please select Brave as default in the Settings window that opened.", 
                    "Action Required", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        // Windows Handlers
        private async void OptimizeWindows_Click(object sender, RoutedEventArgs e)
        {
            if (_windowsOptimizer == null) return;

            // Open custom optimization window
            var optimizationWindow = new Views.OptimizationWindow();
            optimizationWindow.ShowDialog();

            if (optimizationWindow.WasApplied)
            {
                var settings = optimizationWindow.Settings;
                
                await ExecuteSafe(async () =>
                {
                    _logger.LogInfo("Applying selected Windows optimizations...");
                    await _windowsOptimizer.ApplyCustomOptimizations(settings);
                    _logger.LogSuccess("Windows optimizations complete!");
                    MessageBox.Show("Selected optimizations applied successfully!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
        }

        private async void ActivateWindows_Click(object sender, RoutedEventArgs e)
        {
            if (_windowsActivation == null) return;

            var result = MessageBox.Show(
                "This will attempt to activate Windows.\nUse only if you have a valid license.\nContinue?",
                "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await ExecuteSafe(async () =>
                {
                    _logger.LogInfo("Starting Windows activation...");
                    await _windowsActivation.ActivateWindowsAutomatic();
                    _logger.LogSuccess("Activation process complete!");
                    MessageBox.Show("Activation process completed!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
        }

        private void OpenCursorTools_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogInfo("Opening Cursor Tools window...");
                var cursorToolsWindow = new Views.CursorToolsWindow();
                cursorToolsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error opening Cursor Tools: {ex.Message}");
                MessageBox.Show($"Failed to open Cursor Tools:\n{ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenSystemIdSpoofer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogInfo("Opening System ID Spoofer window...");
                var spooferWindow = new Views.SystemIdSpooferWindow();
                spooferWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error opening System ID Spoofer: {ex.Message}");
                MessageBox.Show($"Failed to open System ID Spoofer:\n{ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RestoreOptimizations_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogInfo("Opening Restore Settings window...");
                var restoreWindow = new Views.OptimizationRestoreWindow();
                restoreWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error opening Restore window: {ex.Message}");
                MessageBox.Show($"Failed to open Restore window:\n{ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ResetToSafeDefaults_Click(object sender, RoutedEventArgs e)
        {
            if (_windowsOptimizer == null) return;

            var result = MessageBox.Show(
                "This will reset Windows to SAFE DEFAULTS without needing a backup.\n\n" +
                "What will be restored:\n" +
                "• Network settings (TCP/IP, DNS, Winsock)\n" +
                "• Important services (Windows Search, Superfetch, etc.)\n" +
                "• Visual effects (transparency, animations)\n" +
                "• Windows features (background apps, startup programs)\n\n" +
                "⚠️ This will UNDO most risky optimizations.\n" +
                "⚠️ A system restart is recommended after this.\n\n" +
                "Continue?",
                "Reset to Safe Defaults",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await ExecuteSafe(async () =>
                {
                    _logger.LogInfo("Resetting to safe defaults...");
                    await _windowsOptimizer.ResetToSafeDefaults();
                    
                    MessageBox.Show(
                        "✅ System reset to safe defaults!\n\n" +
                        "All risky optimizations have been reverted.\n\n" +
                        "⚠️ Please RESTART your computer for all changes to take effect.",
                        "Reset Complete",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                });
            }
        }

        // Error Handling Wrapper
        private async Task ExecuteSafe(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"Access denied: {ex.Message}");
                MessageBox.Show("This operation requires administrator privileges.\nPlease run as administrator.", 
                    "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                _logger.LogError($"Network error: {ex.Message}");
                MessageBox.Show("Network error. Please check your internet connection.", 
                    "Network Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError($"Operation timed out: {ex.Message}");
                MessageBox.Show("Operation timed out. Please try again.", 
                    "Timeout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                MessageBox.Show($"An error occurred:\n{ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
