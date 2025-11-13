namespace WindowsSetup.App.Models
{
    public class ToolDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Method { get; set; } = "winget"; // "winget" or "direct"
        public string? WingetId { get; set; }
        public string? DirectUrl { get; set; }
        public string? SilentArgs { get; set; }
        public int Priority { get; set; }
        public bool Essential { get; set; }
        public string? PostInstallCommand { get; set; }
    }
}

