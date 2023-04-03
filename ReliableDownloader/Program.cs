using System;
using System.IO;
using System.Threading.Tasks;
using ReliableDownloader;

namespace ReliableDownloader;

internal class Program
{
    public static async Task Main(string[] args)
    {
        // If this url 404's, you can get a live one from https://installer.demo.accurx.com/chain/latest.json.
        var exampleUrl = "https://installer.demo.accurx.com/chain/3.182.57641.0/accuRx.Installer.Local.msi";

        //get local file path
        var exampleFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        exampleFilePath += @"\Accurx";

        IFileSystem fileSystem = new FileSystem();
        if (!fileSystem.DirectoryExists(exampleFilePath)){
            fileSystem.CreateDirectory(exampleFilePath);
        }

        exampleFilePath = Path.Combine(exampleFilePath, "myFirstDownload.msi");

        var fileDownloader = new FileDownloader(new WebSystemCalls(), fileSystem);
        await fileDownloader.DownloadFile(exampleUrl, exampleFilePath, progress => { Console.WriteLine($"Percent progress is {progress.ProgressPercent:#0.##} %"); });
    }
}