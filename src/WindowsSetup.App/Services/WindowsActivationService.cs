using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WindowsSetup.App.Models;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App.Services
{
    public class WindowsActivationService
    {
        private readonly Logger _logger;
        private readonly CommandRunner _commandRunner;

        public WindowsActivationService(Logger logger)
        {
            _logger = logger;
            _commandRunner = new CommandRunner();
        }

        public async Task ActivateWindowsAutomatic()
        {
            try
            {
                _logger.LogInfo("Starting Windows activation process...");
                _logger.LogWarning("This will run Microsoft Activation Scripts");

                // Run PowerShell script
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"irm https://get.activated.win | iex\"",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = false // Show window for user to see progress
                };

                var process = Process.Start(psi);
                if (process == null)
                {
                    _logger.LogError("Failed to start activation process");
                    return;
                }

                // Wait for menu to appear
                _logger.LogInfo("Waiting for activation menu...");
                await Task.Delay(5000); // Give time for script to load

                // Send option "1" for HWID Activation
                try
                {
                    await process.StandardInput.WriteLineAsync("1");
                    await process.StandardInput.FlushAsync();
                    _logger.LogInfo("Sent activation option '1' (HWID)");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Could not send input automatically: {ex.Message}");
                    _logger.LogInfo("Please select option 1 manually in the PowerShell window");
                }

                // Wait for process to complete
                await process.WaitForExitAsync();

                _logger.LogSuccess("Activation process completed!");

                // Check activation status
                await Task.Delay(2000);
                var status = await CheckWindowsActivationStatus();
                
                if (status.IsActivated)
                {
                    _logger.LogSuccess($"Windows is activated! License: {status.LicenseType}");
                }
                else
                {
                    _logger.LogWarning("Activation status could not be verified. Please check manually.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Activation error: {ex.Message}");
                throw;
            }
        }

        public async Task<ActivationStatus> CheckWindowsActivationStatus()
        {
            try
            {
                _logger.LogInfo("Checking Windows activation status...");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cscript",
                        Arguments = "//nologo C:\\Windows\\System32\\slmgr.vbs /xpr",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                var isActivated = output.Contains("permanently activated", StringComparison.OrdinalIgnoreCase);
                var licenseType = ExtractLicenseType(output);

                if (isActivated)
                {
                    _logger.LogSuccess($"Windows is activated - {licenseType}");
                }
                else
                {
                    _logger.LogWarning("Windows is not activated");
                }

                return new ActivationStatus
                {
                    IsActivated = isActivated,
                    LicenseType = licenseType,
                    RawOutput = output
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to check activation status: {ex.Message}");
                return new ActivationStatus
                {
                    IsActivated = false,
                    LicenseType = "Unknown",
                    RawOutput = ex.Message
                };
            }
        }

        private string ExtractLicenseType(string output)
        {
            if (output.Contains("permanently activated", StringComparison.OrdinalIgnoreCase))
            {
                return "Permanently Activated";
            }
            if (output.Contains("activated", StringComparison.OrdinalIgnoreCase))
            {
                return "Activated";
            }
            if (output.Contains("trial", StringComparison.OrdinalIgnoreCase))
            {
                return "Trial";
            }
            return "Not Activated";
        }
    }
}

