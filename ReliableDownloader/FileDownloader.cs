using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader;

public class FileDownloader : IFileDownloader
{
    private IWebSystemCalls webSystemCalls;
    private IFileSystem fileSystem;
    public long TotalFileLength { get; set; }
    public bool AllowPartialDownload { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; set; }

    public delegate void ProgressChanged(FileProgress fileProgress);
    public event ProgressChanged progressChangedEvent;

    public FileDownloader(IWebSystemCalls _webSystemCalls, IFileSystem _fileSystem)
    {
        fileSystem = _fileSystem;
        webSystemCalls = _webSystemCalls;
    }

    public async Task<bool> DownloadFile(string contentFileUrl, string localFilePath, Action<FileProgress> onProgressChanged)
    {
        ProgressChanged progress = onProgressChanged.Invoke;
        progressChangedEvent += progress;
        //get file content length for marking progress
        CancellationTokenSource = new CancellationTokenSource();
        TotalFileLength = GetContentLength(contentFileUrl).Result;
        HttpResponseMessage result;

        if(TotalFileLength <= 0)
        {
            Console.WriteLine("No content found at the specified path.");
            return false;
        }
        try
        {
            if (CancellationTokenSource.IsCancellationRequested)
            {
                CancelDownloads();
            }
            if (fileSystem.FileExists(localFilePath) && AllowPartialDownload)
            {
                var currFileLength = fileSystem.GetFileInfo(localFilePath).Length;
                if(currFileLength >= TotalFileLength)
                {
                    fileSystem.DeleteFile(localFilePath);
                    currFileLength = 0;
                }
                result = await webSystemCalls.DownloadPartialContent(contentFileUrl, currFileLength, TotalFileLength, CancellationTokenSource.Token);
            }
            else
            {
                if (fileSystem.FileExists(localFilePath))
                {
                    fileSystem.DeleteFile(localFilePath);
                }
                result = await webSystemCalls.DownloadContent(contentFileUrl, CancellationTokenSource.Token);
            }
            if (result.IsSuccessStatusCode)
            {
                await WriteToFile(localFilePath, result);
                fileSystem.VerifyIntegrity(localFilePath, result);
            }

        }
        catch (Exception ex)
        {
            return false;
            Console.WriteLine(ex.Message);
        }

        if(fileSystem.GetFileInfo(localFilePath).Length == TotalFileLength)
        {
            Console.WriteLine("Download Complete.");
        }

        return true;
    }

    public void CancelDownloads()
    {
        if (CancellationTokenSource.Token.CanBeCanceled)
        {
            Console.WriteLine("Download cancelled");
            CancellationTokenSource.Cancel();
        }
    }

    private async Task<long> GetContentLength(string url)
    {
        var response = await webSystemCalls.GetHeadersAsync(url, new System.Threading.CancellationToken());
        AllowPartialDownload = response.Headers.AcceptRanges.Contains("Bytes");
        return response.Content.Headers.ContentLength ?? -1L;
    }

    private async Task WriteToFile(string path, HttpResponseMessage response)
    {
        Console.WriteLine("Download started");
        var startTime = DateTime.Now;
        try
        {
            if (!fileSystem.FileExists(path))
            {
                fileSystem.CreateFile(path);
            }

            using (var downloadStream = await response.Content.ReadAsStreamAsync()) {
                using (var fileStream = fileSystem.OpenFile(path))
                {

                    var totalRead = fileSystem.GetFileInfo(path).Length;
                    var buffer = new byte[TotalFileLength / 10];
                    bool readCompleted = false;

                    do
                    {
                        if (CancellationTokenSource.Token.IsCancellationRequested)
                        {
                            CancelDownloads();
                        }
                        var readData = await downloadStream.ReadAsync(buffer, 0, buffer.Length, CancellationTokenSource.Token);

                        if (readData == 0)
                        {
                            readCompleted = true;
                            break;
                        }

                        await fileStream.WriteAsync(buffer, 0, readData, CancellationTokenSource.Token);
                        totalRead += readData;

                        var elapsedTime = ((DateTime.Now - startTime) * (TotalFileLength - totalRead)) / totalRead;
                        var downloadPercentage = ((totalRead * 1d) / (TotalFileLength * 1d)) * 100;
                        progressChangedEvent.Invoke(new FileProgress(TotalFileLength, totalRead, downloadPercentage, elapsedTime));

                    } while (!readCompleted);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}