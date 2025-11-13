using System;

namespace WindowsSetup.App.Utils
{
    public class Logger
    {
        private readonly Action<string, LogLevel> _logAction;

        public Logger(Action<string, LogLevel> logAction)
        {
            _logAction = logAction;
        }

        public void LogInfo(string message)
        {
            _logAction?.Invoke($"[INFO] {message}", LogLevel.Info);
        }

        public void LogSuccess(string message)
        {
            _logAction?.Invoke($"[SUCCESS] {message}", LogLevel.Success);
        }

        public void LogWarning(string message)
        {
            _logAction?.Invoke($"[WARNING] {message}", LogLevel.Warning);
        }

        public void LogError(string message)
        {
            _logAction?.Invoke($"[ERROR] {message}", LogLevel.Error);
        }
    }
}

