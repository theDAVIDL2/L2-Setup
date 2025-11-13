using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WindowsSetup.App.Models;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App.Services
{
    public class ToolInstallerService
    {
        private readonly Logger _logger;
        private readonly DownloadManager _downloadManager;
        private readonly CommandRunner _commandRunner;
        private readonly Action<int, string> _progressCallback;
        private readonly string _cacheDir;

        public ToolInstallerService(Logger logger, Action<int, string> progressCallback)
        {
            _logger = logger;
            _progressCallback = progressCallback;
            _downloadManager = new DownloadManager(logger);
            _commandRunner = new CommandRunner();
            
            _cacheDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WindowsSetupTool", "Cache");

            if (!Directory.Exists(_cacheDir))
            {
                Directory.CreateDirectory(_cacheDir);
            }
        }

        public async Task InstallEssentialTools()
        {
            _logger.LogInfo("=== Installing Essential Tools ===");

            var essentialTools = new List<ToolDefinition>
            {
                new() { Name = "Git", WingetId = "Git.Git", Priority = 1, Essential = true },
                new() { Name = "Python 3.13", WingetId = "Python.Python.3.13", Priority = 2, Essential = true },
                new() { Name = "Node.js LTS", WingetId = "OpenJS.NodeJS.LTS", Priority = 3, Essential = true },
                new() { Name = "Visual Studio Code", WingetId = "Microsoft.VisualStudioCode", Priority = 4, Essential = true },
                new() { Name = "Brave Browser", WingetId = "Brave.Brave", Priority = 5, Essential = true }
            };

            await InstallTools(essentialTools);
        }

        public async Task InstallAllTools()
        {
            _logger.LogInfo("=== Installing ALL Tools ===");

            var allTools = GetAllToolDefinitions();
            await InstallTools(allTools);
        }

        private List<ToolDefinition> GetAllToolDefinitions()
        {
            return new List<ToolDefinition>
            {
                // CRITICAL: Git must be first
                new() { Name = "Git", WingetId = "Git.Git", Priority = 1, Essential = true },
                
                // Development Runtimes
                new() { Name = "Python 3.13", WingetId = "Python.Python.3.13", Priority = 2, Essential = true },
                new() { Name = "Node.js LTS", WingetId = "OpenJS.NodeJS.LTS", Priority = 3, Essential = true },
                
                // IDEs and Editors
                new() { Name = "Visual Studio Code", WingetId = "Microsoft.VisualStudioCode", Priority = 10 },
                new() { Name = "Cursor", Method = "direct", DirectUrl = "https://download.cursor.sh/windows/nsis/x64", SilentArgs = "/S", Priority = 11 },
                new() { Name = "Notepad++", WingetId = "Notepad++.Notepad++", Priority = 12 },
                
                // Browsers
                new() { Name = "Brave Browser", WingetId = "Brave.Brave", Priority = 20, Essential = true },
                
                // JDKs
                new() { Name = "Amazon Corretto 8", WingetId = "Amazon.Corretto.8", Priority = 30 },
                new() { Name = "Amazon Corretto 17", WingetId = "Amazon.Corretto.17", Priority = 31 },
                
                // Package Managers
                new() { Name = "Yarn", WingetId = "Yarn.Yarn", Priority = 40 },
                
                // Communication & Gaming
                new() { Name = "Discord", WingetId = "Discord.Discord", Priority = 50 },
                new() { Name = "Steam", WingetId = "Valve.Steam", Priority = 51 },
                
                // Utilities
                new() { Name = "WinRAR", WingetId = "RARLab.WinRAR", Priority = 60, PostInstallCommand = "ActivateWinRAR" },
                new() { Name = "Lightshot", WingetId = "Skillbrains.Lightshot", Priority = 61 },
                new() { Name = "JDownloader 2", WingetId = "AppWork.JDownloader", Priority = 62 },
                
                // System Tools
                new() { Name = "System Informer", Method = "direct", DirectUrl = "https://github.com/winsiderss/systeminformer/releases/download/v3.1.24274/systeminformer-3.1.24274-release-setup.exe", SilentArgs = "/VERYSILENT", Priority = 70 },
                
                // Development Tools
                new() { Name = "Postman", WingetId = "Postman.Postman", Priority = 80 },
                new() { Name = "DBeaver", WingetId = "dbeaver.dbeaver", Priority = 81 },
                new() { Name = "FileZilla", WingetId = "FileZilla.FileZilla", Priority = 82 },
                new() { Name = "PuTTY", WingetId = "PuTTY.PuTTY", Priority = 83 },
                
                // Runtimes (install after applications)
                new() { Name = "Microsoft Visual C++ 2015-2022 x64", WingetId = "Microsoft.VCRedist.2015+.x64", Priority = 100 },
                new() { Name = "Microsoft Visual C++ 2015-2022 x86", WingetId = "Microsoft.VCRedist.2015+.x86", Priority = 101 },
                new() { Name = ".NET Framework 4.8", WingetId = "Microsoft.DotNet.Framework.DeveloperPack_4", Priority = 102 }
            };
        }

        private async Task InstallTools(List<ToolDefinition> tools)
        {
            var orderedTools = tools.OrderBy(t => t.Priority).ToList();
            var totalTools = orderedTools.Count;
            var currentTool = 0;

            foreach (var tool in orderedTools)
            {
                currentTool++;
                var percentage = (currentTool * 100) / totalTools;
                _progressCallback(percentage, $"Installing {tool.Name} ({currentTool}/{totalTools})");

                try
                {
                    // Check if already installed
                    if (await IsToolInstalled(tool))
                    {
                        _logger.LogInfo($"[{currentTool}/{totalTools}] {tool.Name} is already installed, skipping...");
                        continue;
                    }

                    _logger.LogInfo($"[{currentTool}/{totalTools}] Installing {tool.Name}...");

                    if (tool.Method == "winget" || string.IsNullOrEmpty(tool.Method))
                    {
                        await InstallViaWinget(tool);
                    }
                    else if (tool.Method == "direct")
                    {
                        await InstallViaDirect(tool);
                    }

                    _logger.LogSuccess($"[{currentTool}/{totalTools}] {tool.Name} installed successfully!");

                    // Run post-install command if specified
                    if (!string.IsNullOrEmpty(tool.PostInstallCommand))
                    {
                        _logger.LogInfo($"Running post-install action for {tool.Name}...");
                        
                        // Special handling for internal methods
                        if (tool.PostInstallCommand == "ActivateWinRAR")
                        {
                            await ActivateWinRAR();
                        }
                        else
                        {
                            await _commandRunner.RunCommandAsync("cmd", $"/c {tool.PostInstallCommand}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to install {tool.Name}: {ex.Message}");
                    
                    if (tool.Essential)
                    {
                        _logger.LogWarning($"{tool.Name} is essential. Some features may not work properly.");
                    }
                }

                // Small delay between installations
                await Task.Delay(1000);
            }

            _progressCallback(100, "Installation complete!");
            _logger.LogSuccess($"=== Installation Complete! Installed {totalTools} tools ===");
        }

        private async Task<bool> IsToolInstalled(ToolDefinition tool)
        {
            try
            {
                if (!string.IsNullOrEmpty(tool.WingetId))
                {
                    var result = await _commandRunner.RunCommandAsync("winget", $"list --id {tool.WingetId}");
                    return result.Success && result.Output?.Contains(tool.WingetId) == true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task InstallViaWinget(ToolDefinition tool)
        {
            if (string.IsNullOrEmpty(tool.WingetId))
            {
                throw new ArgumentException("WingetId is required for winget installation");
            }

            _logger.LogInfo($"Installing {tool.Name} via winget...");

            var result = await _commandRunner.RunCommandAsync("winget", 
                $"install --id {tool.WingetId} --silent --accept-package-agreements --accept-source-agreements");

            if (!result.Success)
            {
                throw new Exception($"Winget installation failed with exit code {result.ExitCode}");
            }
        }

        private async Task InstallViaDirect(ToolDefinition tool)
        {
            if (string.IsNullOrEmpty(tool.DirectUrl))
            {
                throw new ArgumentException("DirectUrl is required for direct installation");
            }

            _logger.LogInfo($"Downloading {tool.Name} from {tool.DirectUrl}...");

            var fileName = Path.GetFileName(new Uri(tool.DirectUrl).LocalPath);
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = $"{tool.Name.Replace(" ", "_")}_installer.exe";
            }

            var downloadPath = Path.Combine(_cacheDir, fileName);

            // Download if not in cache
            if (!File.Exists(downloadPath))
            {
                var downloadItem = new DownloadItem
                {
                    FileName = fileName,
                    Url = tool.DirectUrl,
                    Destination = downloadPath
                };

                await _downloadManager.DownloadWithProgressAsync(downloadItem);
            }
            else
            {
                _logger.LogInfo($"Using cached installer: {fileName}");
            }

            // Install
            _logger.LogInfo($"Installing {tool.Name}...");
            
            var silentArgs = tool.SilentArgs ?? "/S";
            
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = downloadPath,
                Arguments = silentArgs,
                UseShellExecute = true,
                Verb = "runas"
            });

            if (process != null)
            {
                await process.WaitForExitAsync();
                
                if (process.ExitCode != 0)
                {
                    _logger.LogWarning($"Installer exited with code {process.ExitCode}");
                }
            }
        }

        public async Task InstallRuntimes()
        {
            _logger.LogInfo("=== Installing Runtime Packages ===");

            var runtimes = new List<ToolDefinition>
            {
                new() { Name = "Microsoft Visual C++ 2015-2022 x64", WingetId = "Microsoft.VCRedist.2015+.x64", Priority = 1 },
                new() { Name = "Microsoft Visual C++ 2015-2022 x86", WingetId = "Microsoft.VCRedist.2015+.x86", Priority = 2 },
                new() { Name = ".NET Framework 4.8", WingetId = "Microsoft.DotNet.Framework.DeveloperPack_4", Priority = 3 },
                new() { Name = "DirectX End-User Runtime", Method = "direct", 
                    DirectUrl = "https://download.microsoft.com/download/1/7/1/1718CCC4-6315-4D8E-9543-8E28A4E18C4C/dxwebsetup.exe",
                    SilentArgs = "/Q", Priority = 4 }
            };

            await InstallTools(runtimes);
        }

        private async Task ActivateWinRAR()
        {
            try
            {
                _logger.LogInfo("Activating WinRAR...");

                // WinRAR installation paths
                var winrarPaths = new[]
                {
                    @"C:\Program Files\WinRAR",
                    @"C:\Program Files (x86)\WinRAR",
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "WinRAR"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "WinRAR")
                };

                string? winrarPath = null;
                foreach (var path in winrarPaths)
                {
                    if (Directory.Exists(path))
                    {
                        winrarPath = path;
                        break;
                    }
                }

                if (winrarPath == null)
                {
                    _logger.LogWarning("WinRAR installation directory not found!");
                    return;
                }

                // WinRAR license key content
                var licenseKey = @"RAR registration data
Federal Agency for Education
1000000 PC usage license
UID=b621cca9a84bc5deffbf
6412212250ffbf533df6db2dfe8ccc3aae5362c06d54762105357d
5e3b1489e751c76bf6e0640001014be50a52303fed29664b074145
7e567d04159ad8defc3fb6edf32831fd1966f72c21c0c53c02fbbb
2f91cfca671d9c482b11b8ac3281cb21378e85606494da349941fa
e9ee328f12dc73e90b6356b921fbfb8522d6562a6a4b97e8ef6c9f
fb866be1e3826b5aa126a4d2bfe9336ad63003fc0e71c307fc2c60
64416495d4c55a0cc82d402110498da970812063934815d81470829
".Replace("\r\n", "\n").Replace("\n", "\r\n");

                // Write rarreg.key file
                var keyPath = Path.Combine(winrarPath, "rarreg.key");
                await File.WriteAllTextAsync(keyPath, licenseKey);

                _logger.LogSuccess($"WinRAR activated successfully! License file created at: {keyPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to activate WinRAR: {ex.Message}");
            }
        }
    }
}

