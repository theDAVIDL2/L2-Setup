using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WindowsSetup.App.Models;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App.Services
{
    public class RuntimeInstallerService
    {
        private readonly Logger _logger;
        private readonly HttpClient _httpClient;
        private readonly string _tempPath;
        private readonly Action<int, string> _progressCallback;

        public RuntimeInstallerService(Logger logger, Action<int, string> progressCallback)
        {
            _logger = logger;
            _progressCallback = progressCallback;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
            _tempPath = Path.Combine(Path.GetTempPath(), "WindowsSetup_Runtimes");
            Directory.CreateDirectory(_tempPath);
        }

        public List<RuntimeDefinition> GetAllRuntimes()
        {
            return new List<RuntimeDefinition>
            {
                // ===== VISUAL C++ REDISTRIBUTABLES =====
                new() { Name = "VC++ 2005 x86", Url = "https://download.microsoft.com/download/8/B/4/8B42259F-5D70-43F4-AC2E-4B208FD8D66A/vcredist_x86.EXE", FileName = "vcredist2005_x86.exe", SilentArgs = "/q", Is64Bit = false, Category = "VC++ Redistributables" },
                new() { Name = "VC++ 2005 x64", Url = "https://download.microsoft.com/download/8/B/4/8B42259F-5D70-43F4-AC2E-4B208FD8D66A/vcredist_x64.EXE", FileName = "vcredist2005_x64.exe", SilentArgs = "/q", Category = "VC++ Redistributables" },
                
                new() { Name = "VC++ 2008 x86", Url = "https://download.microsoft.com/download/5/D/8/5D8C65CB-C849-4025-8E95-C3966CAFD8AE/vcredist_x86.exe", FileName = "vcredist2008_x86.exe", SilentArgs = "/q", Is64Bit = false, Category = "VC++ Redistributables" },
                new() { Name = "VC++ 2008 x64", Url = "https://download.microsoft.com/download/5/D/8/5D8C65CB-C849-4025-8E95-C3966CAFD8AE/vcredist_x64.exe", FileName = "vcredist2008_x64.exe", SilentArgs = "/q", Category = "VC++ Redistributables" },
                
                new() { Name = "VC++ 2010 x86", Url = "https://download.microsoft.com/download/1/6/5/165255E7-1014-4D0A-B094-B6A430A6BFFC/vcredist_x86.exe", FileName = "vcredist2010_x86.exe", SilentArgs = "/q /norestart", Is64Bit = false, Category = "VC++ Redistributables" },
                new() { Name = "VC++ 2010 x64", Url = "https://download.microsoft.com/download/1/6/5/165255E7-1014-4D0A-B094-B6A430A6BFFC/vcredist_x64.exe", FileName = "vcredist2010_x64.exe", SilentArgs = "/q /norestart", Category = "VC++ Redistributables" },
                
                new() { Name = "VC++ 2012 x86", Url = "https://download.microsoft.com/download/1/6/B/16B06F60-3B20-4FF2-B699-5E9B7962F9AE/VSU_4/vcredist_x86.exe", FileName = "vcredist2012_x86.exe", SilentArgs = "/install /quiet /norestart", Is64Bit = false, Category = "VC++ Redistributables" },
                new() { Name = "VC++ 2012 x64", Url = "https://download.microsoft.com/download/1/6/B/16B06F60-3B20-4FF2-B699-5E9B7962F9AE/VSU_4/vcredist_x64.exe", FileName = "vcredist2012_x64.exe", SilentArgs = "/install /quiet /norestart", Category = "VC++ Redistributables" },
                
                new() { Name = "VC++ 2013 x86", Url = "https://aka.ms/highdpimfc2013x86enu", FileName = "vcredist2013_x86.exe", SilentArgs = "/install /quiet /norestart", Is64Bit = false, Category = "VC++ Redistributables" },
                new() { Name = "VC++ 2013 x64", Url = "https://aka.ms/highdpimfc2013x64enu", FileName = "vcredist2013_x64.exe", SilentArgs = "/install /quiet /norestart", Category = "VC++ Redistributables" },
                
                new() { Name = "VC++ 2015-2022 x86", Url = "https://aka.ms/vs/17/release/vc_redist.x86.exe", FileName = "vcredist2015-2022_x86.exe", SilentArgs = "/install /quiet /norestart", Is64Bit = false, Category = "VC++ Redistributables" },
                new() { Name = "VC++ 2015-2022 x64", Url = "https://aka.ms/vs/17/release/vc_redist.x64.exe", FileName = "vcredist2015-2022_x64.exe", SilentArgs = "/install /quiet /norestart", Category = "VC++ Redistributables" },

                // ===== .NET FRAMEWORK =====
                new() { Name = ".NET Framework 3.5", Url = "DISM", FileName = "dotnet35.dism", SilentArgs = "/online /enable-feature /featurename:NetFx3 /all /norestart", Category = ".NET Framework", Description = "Via DISM" },
                new() { Name = ".NET Framework 4.5.2", Url = "https://go.microsoft.com/fwlink/?LinkId=397708", FileName = "NDP452-KB2901907-x86-x64-AllOS-ENU.exe", SilentArgs = "/q /norestart", Category = ".NET Framework" },
                new() { Name = ".NET Framework 4.6.2", Url = "https://go.microsoft.com/fwlink/?linkid=780600", FileName = "NDP462-KB3151800-x86-x64-AllOS-ENU.exe", SilentArgs = "/q /norestart", Category = ".NET Framework" },
                new() { Name = ".NET Framework 4.7.2", Url = "https://go.microsoft.com/fwlink/?LinkId=863265", FileName = "NDP472-KB4054530-x86-x64-AllOS-ENU.exe", SilentArgs = "/q /norestart", Category = ".NET Framework" },
                new() { Name = ".NET Framework 4.8", Url = "https://go.microsoft.com/fwlink/?LinkId=2085155", FileName = "ndp48-x86-x64-allos-enu.exe", SilentArgs = "/q /norestart", Category = ".NET Framework" },
                new() { Name = ".NET Framework 4.8.1", Url = "https://go.microsoft.com/fwlink/?linkid=2203304", FileName = "ndp481-x86-x64-allos-enu.exe", SilentArgs = "/q /norestart", Category = ".NET Framework" },

                // ===== .NET RUNTIMES (Core/Modern) =====
                new() { Name = ".NET 5.0 Runtime", Url = "https://download.visualstudio.microsoft.com/download/pr/1b2624c4-c45e-42ac-b5fd-bf6b91f71095/4ba8fe5ac57bb13c04e1f620e5a2c4fa/windowsdesktop-runtime-5.0.17-win-x64.exe", FileName = "dotnet5-runtime-x64.exe", SilentArgs = "/install /quiet /norestart", Category = ".NET Runtimes" },
                new() { Name = ".NET 6.0 Runtime", Url = "https://download.visualstudio.microsoft.com/download/pr/8bc41df1-cbb4-4da6-944f-6652378e9196/1014aacedc80bbcc030dabb168d2532f/windowsdesktop-runtime-6.0.28-win-x64.exe", FileName = "dotnet6-runtime-x64.exe", SilentArgs = "/install /quiet /norestart", Category = ".NET Runtimes" },
                new() { Name = ".NET 7.0 Runtime", Url = "https://download.visualstudio.microsoft.com/download/pr/d8ad4135-ee34-4a8e-ab7e-f6b0bb27f9eb/cb4d7b57f0c5c2c8b626cdf71d2c7e02/windowsdesktop-runtime-7.0.16-win-x64.exe", FileName = "dotnet7-runtime-x64.exe", SilentArgs = "/install /quiet /norestart", Category = ".NET Runtimes" },
                new() { Name = ".NET 8.0 Runtime", Url = "https://download.visualstudio.microsoft.com/download/pr/d8cf1fe3-21c2-4baf-988f-f0152996135e/0c00b94713ee93e7ad5b4f82e2b86607/windowsdesktop-runtime-8.0.2-win-x64.exe", FileName = "dotnet8-runtime-x64.exe", SilentArgs = "/install /quiet /norestart", Category = ".NET Runtimes" },

                // ===== DIRECTX & GAMING =====
                new() { Name = "DirectX End-User Runtime", Url = "https://download.microsoft.com/download/1/7/1/1718CCC4-6315-4D8E-9543-8E28A4E18C4C/dxwebsetup.exe", FileName = "dxwebsetup.exe", SilentArgs = "/Q", Category = "DirectX & Gaming" },
                new() { Name = "XNA Framework 4.0", Url = "https://download.microsoft.com/download/A/C/2/AC2C903B-E6E8-42C2-9FD7-BEBAC362A930/xnafx40_redist.msi", FileName = "xnafx40_redist.msi", SilentArgs = "/qn /norestart", Category = "DirectX & Gaming" },
                
                // ===== OPENAL =====
                new() { Name = "OpenAL", Url = "https://openal.org/downloads/oalinst.zip", FileName = "oalinst.exe", SilentArgs = "/SILENT", Category = "Audio", Description = "3D Audio library" },

                // ===== JAVA RUNTIMES =====
                new() { Name = "Java Runtime 8 x64", Url = "https://javadl.oracle.com/webapps/download/AutoDL?BundleId=249840_4d245f941845490c91024a7b87c0d11e", FileName = "jre-8-windows-x64.exe", SilentArgs = "/s", Category = "Java" },
                new() { Name = "Java Runtime 21 (Latest LTS)", Url = "https://download.oracle.com/java/21/latest/jdk-21_windows-x64_bin.exe", FileName = "jdk-21-windows-x64.exe", SilentArgs = "/s", Category = "Java" },

                // ===== VISUAL STUDIO RUNTIMES =====
                new() { Name = "Visual Studio Tools for Office Runtime", Url = "https://download.microsoft.com/download/1/2/A/12A26372-9ADE-4C08-A0DE-DFED63AC00D5/vstor_redist.exe", FileName = "vstor_redist.exe", SilentArgs = "/q", Category = "Office & Development" },
                
                // ===== MEDIA CODECS =====
                new() { Name = "K-Lite Codec Pack Basic", Url = "https://files3.codecguide.com/K-Lite_Codec_Pack_1805_Basic.exe", FileName = "klite-basic.exe", SilentArgs = "/VERYSILENT /NORESTART", Category = "Media Codecs", Essential = false },
            };
        }

        public async Task InstallAllRuntimes()
        {
            _logger.LogInfo("=== Installing All Runtimes (All-in-One) ===");
            
            var runtimes = GetAllRuntimes();
            var totalRuntimes = runtimes.Count;
            var currentRuntime = 0;

            foreach (var runtime in runtimes)
            {
                currentRuntime++;
                var percentage = (currentRuntime * 100) / totalRuntimes;
                
                try
                {
                    _progressCallback(percentage, $"Installing {runtime.Name}...");
                    _logger.LogInfo($"[{currentRuntime}/{totalRuntimes}] {runtime.Name}");

                    if (runtime.Url == "DISM")
                    {
                        await InstallViaDism(runtime);
                    }
                    else
                    {
                        await DownloadAndInstallRuntime(runtime);
                    }

                    _logger.LogSuccess($"✅ {runtime.Name} installed");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"⚠️ Failed to install {runtime.Name}: {ex.Message}");
                    // Continue with next runtime
                }
            }

            // Cleanup
            await CleanupTempFiles();

            _logger.LogSuccess("=== All Runtimes Installation Complete! ===");
        }

        private async Task DownloadAndInstallRuntime(RuntimeDefinition runtime)
        {
            var filePath = Path.Combine(_tempPath, runtime.FileName);

            try
            {
                // Download
                _logger.LogInfo($"Downloading {runtime.Name}...");
                using (var response = await _httpClient.GetAsync(runtime.Url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }
                }

                // Install
                _logger.LogInfo($"Installing {runtime.Name}...");
                var psi = new ProcessStartInfo
                {
                    FileName = filePath,
                    Arguments = runtime.SilentArgs,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(psi);
                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                }
            }
            finally
            {
                // Delete installer after installation
                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch { /* Ignore cleanup errors */ }
                }
            }
        }

        private async Task InstallViaDism(RuntimeDefinition runtime)
        {
            _logger.LogInfo($"Installing {runtime.Name} via DISM...");
            
            var psi = new ProcessStartInfo
            {
                FileName = "dism.exe",
                Arguments = runtime.SilentArgs,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(psi);
            if (process != null)
            {
                await Task.Run(() => process.WaitForExit());
            }
        }

        private async Task CleanupTempFiles()
        {
            try
            {
                _logger.LogInfo("Cleaning up temporary files...");
                
                if (Directory.Exists(_tempPath))
                {
                    await Task.Run(() => Directory.Delete(_tempPath, true));
                }

                _logger.LogSuccess("Temporary files cleaned up");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not clean up temp files: {ex.Message}");
            }
        }
    }
}

