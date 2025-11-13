namespace WindowsSetup.App.Models
{
    public class ProcessResult
    {
        public int ExitCode { get; set; }
        public bool Success { get; set; }
        public string? Output { get; set; }
        public string? Error { get; set; }
    }
}

