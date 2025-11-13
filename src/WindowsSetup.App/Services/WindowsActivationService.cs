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
                _logger.LogInfo("═══════════════════════════════════════════════════════════");
                _logger.LogInfo("Starting Windows Activation Process...");
                _logger.LogWarning("⚠️ MANUAL INTERACTION MAY BE REQUIRED!");
                _logger.LogInfo("═══════════════════════════════════════════════════════════");
                _logger.LogInfo("");
                _logger.LogInfo("📋 What will happen:");
                _logger.LogInfo("   1. A CMD window will open with Microsoft Activation Scripts");
                _logger.LogInfo("   2. The script will try to auto-select option [1] (HWID)");
                _logger.LogInfo("   3. ⚠️ If auto-selection fails, YOU MUST press '1' + ENTER");
                _logger.LogInfo("   4. Wait for activation to complete");
                _logger.LogInfo("");
                _logger.LogWarning("⏱️ The process may take 30-60 seconds...");
                _logger.LogWarning("👀 WATCH THE CMD WINDOW THAT WILL APPEAR!");
                _logger.LogInfo("═══════════════════════════════════════════════════════════");
                _logger.LogInfo("");

                // Give user time to read instructions
                await Task.Delay(3000);

                _logger.LogInfo("🚀 Launching Microsoft Activation Scripts...");

                // Create batch wrapper for better control
                var batContent = @"@echo off
title L2 Setup - Windows Activation (Microsoft Activation Scripts)
color 0B
echo.
echo ========================================================
echo    Microsoft Activation Scripts (MAS)
echo    L2 Setup - Automatic Activation Attempt
echo ========================================================
echo.
echo [INFO] Downloading and running MAS...
echo [INFO] Please wait...
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command ""irm https://get.activated.win | iex""
echo.
echo ========================================================
if %ERRORLEVEL% EQU 0 (
    echo    [SUCCESS] Activation process completed!
) else (
    echo    [WARNING] Process exited with code: %ERRORLEVEL%
    echo    This doesn't necessarily mean failure.
)
echo ========================================================
echo.
echo Press any key to close this window...
pause >nul
";

                var batPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "L2Setup_WindowsActivation.bat");
                await System.IO.File.WriteAllTextAsync(batPath, batContent);

                _logger.LogInfo("⏳ Starting activation script...");
                _logger.LogInfo("");
                _logger.LogWarning("⚠️ IMPORTANT INSTRUCTIONS:");
                _logger.LogInfo("   → A CMD window will appear");
                _logger.LogInfo("   → If you see a menu, press '1' for HWID Activation");
                _logger.LogInfo("   → Press ENTER to confirm");
                _logger.LogInfo("   → Wait for the process to complete");
                _logger.LogInfo("   → The window will close automatically when done");
                _logger.LogInfo("");

                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{batPath}\"",
                    UseShellExecute = true,
                    Verb = "runas", // Run as admin
                    WindowStyle = ProcessWindowStyle.Normal
                };

                var process = Process.Start(psi);
                if (process == null)
                {
                    _logger.LogError("❌ Failed to start activation process");
                    return;
                }

                _logger.LogSuccess("✅ Activation window launched!");
                _logger.LogInfo("📝 The CMD window is now running...");
                _logger.LogInfo("");
                _logger.LogInfo("⏱️ Waiting for activation to complete...");
                _logger.LogInfo("   (This process runs in a separate window)");
                _logger.LogInfo("");
                _logger.LogWarning("👉 Remember: Press '1' + ENTER if you see a menu!");

                // Monitor process in background
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Run(() => process.WaitForExit());
                        
                        // Clean up temp file
                        try { System.IO.File.Delete(batPath); } catch { }
                        
                        _logger.LogInfo("");
                        _logger.LogInfo("═══════════════════════════════════════════════════════════");
                        _logger.LogInfo("✅ Activation window has closed");
                        _logger.LogInfo("═══════════════════════════════════════════════════════════");
                        _logger.LogInfo("");
                        _logger.LogInfo("📋 Next steps:");
                        _logger.LogInfo("   1. Click 'Check Status' button below to verify");
                        _logger.LogInfo("   2. If not activated, check the CMD output for errors");
                        _logger.LogInfo("   3. You may need to try again or use manual activation");
                        _logger.LogInfo("");
                        _logger.LogSuccess("💡 TIP: Activation can take a few minutes to reflect in the system");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error monitoring activation process: {ex.Message}");
                    }
                });

                // Don't wait for completion - let it run in background
                _logger.LogInfo("ℹ️ You can continue using the app while activation runs");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Failed to start Windows activation: {ex.Message}");
                _logger.LogInfo("");
                _logger.LogInfo("💡 Try manual activation:");
                _logger.LogInfo("   1. Open PowerShell as Administrator");
                _logger.LogInfo("   2. Run: irm https://get.activated.win | iex");
                _logger.LogInfo("   3. Select option [1] HWID Activation");
                _logger.LogInfo("   4. Follow the on-screen instructions");
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

