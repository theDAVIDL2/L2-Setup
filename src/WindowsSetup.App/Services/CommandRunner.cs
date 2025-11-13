using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WindowsSetup.App.Models;

namespace WindowsSetup.App.Services
{
    public class CommandRunner
    {
        public event EventHandler<string>? OutputReceived;
        public event EventHandler<string>? ErrorReceived;

        public async Task<ProcessResult> RunCommandAsync(string command, string args, bool requireAdmin = false)
        {
            var psi = new ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                RedirectStandardOutput = !requireAdmin,
                RedirectStandardError = !requireAdmin,
                RedirectStandardInput = !requireAdmin,
                UseShellExecute = requireAdmin,
                CreateNoWindow = !requireAdmin
            };

            if (requireAdmin)
            {
                psi.Verb = "runas";
            }

            var process = new Process { StartInfo = psi };
            var output = string.Empty;
            var error = string.Empty;

            if (!requireAdmin)
            {
                process.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        output += e.Data + "\n";
                        OutputReceived?.Invoke(this, e.Data);
                    }
                };

                process.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        error += e.Data + "\n";
                        ErrorReceived?.Invoke(this, e.Data);
                    }
                };
            }

            process.Start();

            if (!requireAdmin)
            {
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }

            await process.WaitForExitAsync();

            return new ProcessResult
            {
                ExitCode = process.ExitCode,
                Success = process.ExitCode == 0,
                Output = output,
                Error = error
            };
        }

        public async Task<ProcessResult> RunPowerShellAsync(string script, bool requireAdmin = false)
        {
            return await RunCommandAsync("powershell.exe", $"-NoProfile -Command \"{script}\"", requireAdmin);
        }
    }
}

