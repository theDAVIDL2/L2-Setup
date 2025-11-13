namespace WindowsSetup.App.Models
{
    public class RuntimeDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string SilentArgs { get; set; } = "/quiet /norestart";
        public bool Is64Bit { get; set; } = true;
        public bool Essential { get; set; } = true;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}

