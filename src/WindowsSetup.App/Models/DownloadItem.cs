namespace WindowsSetup.App.Models
{
    public class DownloadItem
    {
        public string FileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string? ExpectedHash { get; set; }
        public long TotalBytes { get; set; }
    }

    public class DownloadProgress
    {
        public string FileName { get; set; } = string.Empty;
        public long BytesDownloaded { get; set; }
        public long TotalBytes { get; set; }
        public int Percentage { get; set; }
        public double SpeedMBps { get; set; }
    }
}

