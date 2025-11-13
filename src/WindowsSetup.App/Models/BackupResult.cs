namespace WindowsSetup.App.Models
{
    public class BackupResult
    {
        public bool Success { get; set; }
        public string? BackupPath { get; set; }
        public double SizeInMB { get; set; }
        public int ExtensionsCount { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class RestoreResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

