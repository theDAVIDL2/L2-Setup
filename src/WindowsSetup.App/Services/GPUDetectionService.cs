using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Threading.Tasks;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App.Services
{
    public class GPUDetectionService
    {
        private readonly Logger _logger;
        private readonly HttpClient _httpClient;

        public enum GPUVendor
        {
            Unknown,
            NVIDIA,
            AMD,
            Intel
        }

        public class GPUInfo
        {
            public string Name { get; set; } = string.Empty;
            public GPUVendor Vendor { get; set; }
            public string DriverVersion { get; set; } = string.Empty;
            public string DownloadUrl { get; set; } = string.Empty;
        }

        public GPUDetectionService(Logger logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Windows-Setup-Tool/1.0");
        }

        public GPUInfo DetectGPU()
        {
            try
            {
                _logger.LogInfo("Detecting GPU...");

                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                var gpuInfo = new GPUInfo();

                foreach (ManagementObject obj in searcher.Get())
                {
                    var name = obj["Name"]?.ToString() ?? "";
                    var driverVersion = obj["DriverVersion"]?.ToString() ?? "";

                    _logger.LogInfo($"Found GPU: {name}");

                    // Detect vendor
                    if (name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("GeForce", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("GTX", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("RTX", StringComparison.OrdinalIgnoreCase))
                    {
                        gpuInfo.Vendor = GPUVendor.NVIDIA;
                        gpuInfo.Name = name;
                        gpuInfo.DriverVersion = driverVersion;
                        _logger.LogSuccess($"Detected NVIDIA GPU: {name}");
                        break; // NVIDIA takes priority
                    }
                    else if (name.Contains("AMD", StringComparison.OrdinalIgnoreCase) ||
                             name.Contains("Radeon", StringComparison.OrdinalIgnoreCase) ||
                             name.Contains("RX ", StringComparison.OrdinalIgnoreCase))
                    {
                        gpuInfo.Vendor = GPUVendor.AMD;
                        gpuInfo.Name = name;
                        gpuInfo.DriverVersion = driverVersion;
                        _logger.LogSuccess($"Detected AMD GPU: {name}");
                        // Don't break, in case there's also an NVIDIA GPU
                    }
                    else if (name.Contains("Intel", StringComparison.OrdinalIgnoreCase))
                    {
                        if (gpuInfo.Vendor == GPUVendor.Unknown)
                        {
                            gpuInfo.Vendor = GPUVendor.Intel;
                            gpuInfo.Name = name;
                            gpuInfo.DriverVersion = driverVersion;
                            _logger.LogInfo($"Detected Intel GPU: {name}");
                        }
                    }
                }

                if (gpuInfo.Vendor == GPUVendor.Unknown)
                {
                    _logger.LogWarning("Could not detect GPU vendor");
                }

                return gpuInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error detecting GPU: {ex.Message}");
                return new GPUInfo { Vendor = GPUVendor.Unknown };
            }
        }

        public string GetDriverDownloadUrl(GPUVendor vendor)
        {
            return vendor switch
            {
                GPUVendor.NVIDIA => "https://www.nvidia.com/Download/index.aspx",
                GPUVendor.AMD => "https://www.amd.com/en/support",
                GPUVendor.Intel => "https://www.intel.com/content/www/us/en/download-center/home.html",
                _ => ""
            };
        }

        public async Task<bool> DownloadAndInstallDrivers(GPUVendor vendor, Action<int, string> progressCallback)
        {
            try
            {
                _logger.LogInfo($"Preparing to download {vendor} drivers...");
                progressCallback(10, $"Detecting {vendor} GPU drivers...");

                switch (vendor)
                {
                    case GPUVendor.NVIDIA:
                        return await InstallNvidiaDrivers(progressCallback);

                    case GPUVendor.AMD:
                        return await InstallAMDDrivers(progressCallback);

                    case GPUVendor.Intel:
                        return await InstallIntelDrivers(progressCallback);

                    default:
                        _logger.LogWarning("Unknown GPU vendor, cannot download drivers");
                        return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error downloading drivers: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> InstallNvidiaDrivers(Action<int, string> progressCallback)
        {
            try
            {
                _logger.LogInfo("Installing NVIDIA GeForce Experience (includes drivers)...");
                progressCallback(30, "Downloading NVIDIA GeForce Experience...");

                // GeForce Experience includes driver management
                var nvidiaUrl = "https://us.download.nvidia.com/GFE/GFEClient/3.27.0.120/GeForce_Experience_v3.27.0.120.exe";
                var outputPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "GeForce_Experience_Setup.exe");

                _logger.LogInfo("Downloading from NVIDIA...");
                progressCallback(50, "Downloading NVIDIA installer...");

                using (var response = await _httpClient.GetAsync(nvidiaUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    using var fileStream = System.IO.File.Create(outputPath);
                    await response.Content.CopyToAsync(fileStream);
                }

                progressCallback(80, "Installing NVIDIA GeForce Experience...");
                _logger.LogInfo("Running NVIDIA installer...");

                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = outputPath,
                    Arguments = "-s",
                    UseShellExecute = true,
                    Verb = "runas"
                });

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    progressCallback(100, "NVIDIA installation complete!");
                    _logger.LogSuccess("NVIDIA GeForce Experience installed successfully!");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error installing NVIDIA drivers: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> InstallAMDDrivers(Action<int, string> progressCallback)
        {
            try
            {
                _logger.LogInfo("Preparing AMD driver installation...");
                progressCallback(30, "Downloading AMD Auto-Detect tool...");

                // AMD Auto-Detect and Install tool
                var amdUrl = "https://drivers.amd.com/drivers/installer/22.40/beta/amd-software-adrenalin-edition-22.40.53.02-minimalsetup-221219_web.exe";
                var outputPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "AMD_Driver_Setup.exe");

                _logger.LogInfo("Downloading from AMD...");
                progressCallback(50, "Downloading AMD installer...");

                using (var response = await _httpClient.GetAsync(amdUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using var fileStream = System.IO.File.Create(outputPath);
                        await response.Content.CopyToAsync(fileStream);

                        progressCallback(80, "Installing AMD drivers...");
                        _logger.LogInfo("Running AMD installer...");

                        var process = Process.Start(new ProcessStartInfo
                        {
                            FileName = outputPath,
                            Arguments = "/S",
                            UseShellExecute = true,
                            Verb = "runas"
                        });

                        if (process != null)
                        {
                            await Task.Run(() => process.WaitForExit());
                            progressCallback(100, "AMD installation complete!");
                            _logger.LogSuccess("AMD drivers installed successfully!");
                            return true;
                        }
                    }
                    else
                    {
                        // Fallback: Open AMD support page
                        _logger.LogWarning("Could not auto-download AMD drivers. Opening AMD support page...");
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://www.amd.com/en/support",
                            UseShellExecute = true
                        });
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error installing AMD drivers: {ex.Message}");
                _logger.LogInfo("Opening AMD support page...");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://www.amd.com/en/support",
                    UseShellExecute = true
                });
                return false;
            }
        }

        private async Task<bool> InstallIntelDrivers(Action<int, string> progressCallback)
        {
            try
            {
                _logger.LogInfo("Installing Intel Driver & Support Assistant...");
                progressCallback(30, "Downloading Intel DSA...");

                var intelUrl = "https://dsadata.intel.com/installer";
                var outputPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Intel_DSA_Setup.exe");

                progressCallback(50, "Downloading Intel installer...");

                using (var response = await _httpClient.GetAsync(intelUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using var fileStream = System.IO.File.Create(outputPath);
                        await response.Content.CopyToAsync(fileStream);

                        progressCallback(80, "Installing Intel DSA...");

                        var process = Process.Start(new ProcessStartInfo
                        {
                            FileName = outputPath,
                            Arguments = "/quiet",
                            UseShellExecute = true,
                            Verb = "runas"
                        });

                        if (process != null)
                        {
                            await Task.Run(() => process.WaitForExit());
                            progressCallback(100, "Intel DSA installed!");
                            _logger.LogSuccess("Intel Driver & Support Assistant installed!");
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error installing Intel drivers: {ex.Message}");
                return false;
            }
        }

        public string GetDriverInstallationInstructions(GPUVendor vendor)
        {
            return vendor switch
            {
                GPUVendor.NVIDIA => 
                    "NVIDIA GeForce Experience instalado! Use-o para:\n" +
                    "1. Baixar os drivers mais recentes\n" +
                    "2. Otimizar configurações de jogos\n" +
                    "3. Gravar gameplay com ShadowPlay",

                GPUVendor.AMD => 
                    "AMD Software instalado! Use-o para:\n" +
                    "1. Manter drivers atualizados\n" +
                    "2. Configurar Radeon Settings\n" +
                    "3. Otimizar performance de jogos",

                GPUVendor.Intel => 
                    "Intel DSA instalado! Use-o para:\n" +
                    "1. Detectar hardware Intel\n" +
                    "2. Baixar drivers automaticamente\n" +
                    "3. Manter sistema atualizado",

                _ => "Não foi possível detectar a GPU."
            };
        }
    }
}

