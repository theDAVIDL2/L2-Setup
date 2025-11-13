namespace WindowsSetup.App.Models
{
    public class ActivationStatus
    {
        public bool IsActivated { get; set; }
        public string? LicenseType { get; set; }
        public string? RawOutput { get; set; }
    }
}

