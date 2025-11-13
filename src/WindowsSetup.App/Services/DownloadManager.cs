using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using WindowsSetup.App.Models;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App.Services
{
    public class DownloadManager
    {
        private readonly HttpClient _httpClient;
        private readonly int _maxParallelDownloads;
        private readonly SemaphoreSlim _semaphore;
        private readonly Logger _logger;

        public DownloadManager(Logger logger, int maxParallelDownloads = 4)
        {
            _logger = logger;
            _maxParallelDownloads = maxParallelDownloads;
            _semaphore = new SemaphoreSlim(maxParallelDownloads);
            
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(30)
            };
        }

        public async Task DownloadMultipleAsync(List<DownloadItem> items, IProgress<DownloadProgress>? progress = null)
        {
            _logger.LogInfo($"Starting download of {items.Count} items (max {_maxParallelDownloads} parallel)");

            var tasks = items.Select(async item =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    await DownloadWithProgressAsync(item, progress);
                }
                finally
                {
                    _semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);

            _logger.LogSuccess("All downloads completed!");
        }

        public async Task DownloadWithProgressAsync(DownloadItem item, IProgress<DownloadProgress>? progress = null)
        {
            var retryCount = 0;
            var maxRetries = 3;

            while (retryCount < maxRetries)
            {
                try
                {
                    _logger.LogInfo($"Downloading: {item.FileName}");

                    using var response = await _httpClient.GetAsync(item.Url, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? 0;
                    var bytesDownloaded = 0L;
                    var startTime = DateTime.Now;

                    // Create directory if it doesn't exist
                    var directory = Path.GetDirectoryName(item.Destination);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    using var contentStream = await response.Content.ReadAsStreamAsync();
                    using var fileStream = new FileStream(item.Destination, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

                    var buffer = new byte[8192];
                    int bytesRead;

                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        bytesDownloaded += bytesRead;

                        // Calculate speed
                        var elapsed = (DateTime.Now - startTime).TotalSeconds;
                        var speedMBps = elapsed > 0 ? (bytesDownloaded / (1024.0 * 1024.0)) / elapsed : 0;

                        progress?.Report(new DownloadProgress
                        {
                            FileName = item.FileName,
                            BytesDownloaded = bytesDownloaded,
                            TotalBytes = totalBytes,
                            Percentage = totalBytes > 0 ? (int)((bytesDownloaded * 100) / totalBytes) : 0,
                            SpeedMBps = speedMBps
                        });
                    }

                    // Verify hash if provided
                    if (!string.IsNullOrEmpty(item.ExpectedHash))
                    {
                        _logger.LogInfo($"Verifying hash for {item.FileName}...");
                        var actualHash = await ComputeSHA256Async(item.Destination);
                        
                        if (!actualHash.Equals(item.ExpectedHash, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new Exception($"Hash verification failed! Expected: {item.ExpectedHash}, Got: {actualHash}");
                        }

                        _logger.LogSuccess($"Hash verified for {item.FileName}");
                    }

                    _logger.LogSuccess($"Downloaded: {item.FileName} ({bytesDownloaded / (1024.0 * 1024.0):F2} MB)");
                    return; // Success, exit retry loop
                }
                catch (Exception ex)
                {
                    retryCount++;
                    _logger.LogWarning($"Download failed (attempt {retryCount}/{maxRetries}): {ex.Message}");

                    if (retryCount >= maxRetries)
                    {
                        _logger.LogError($"Failed to download {item.FileName} after {maxRetries} attempts");
                        throw new Exception($"Failed after {maxRetries} attempts: {ex.Message}");
                    }

                    // Exponential backoff
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount));
                    _logger.LogInfo($"Retrying in {delay.TotalSeconds} seconds...");
                    await Task.Delay(delay);
                }
            }
        }

        private async Task<string> ComputeSHA256Async(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = await sha256.ComputeHashAsync(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}

