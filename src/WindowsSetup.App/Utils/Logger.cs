using System;

namespace WindowsSetup.App.Utils
{
    public enum LogLevel
    {
        Info,
        Success,
        Warning,
        Error
    }

    public class Logger
    {
        private readonly Action<string> _logCallback;

        public Logger(Action<string> logCallback)
        {
            _logCallback = logCallback;
        }

        public void LogInfo(string message)
        {
            _logCallback?.Invoke($"[INFO] {message}");
        }

        public void LogSuccess(string message)
        {
            _logCallback?.Invoke($"✅ [SUCCESS] {message}");
        }

        public void LogWarning(string message)
        {
            _logCallback?.Invoke($"⚠️  [WARNING] {message}");
        }

        public void LogError(string message)
        {
            _logCallback?.Invoke($"❌ [ERROR] {message}");
        }
    }
}
