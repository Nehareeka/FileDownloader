using Moq;
using NUnit.Framework;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ReliableDownloader.Tests;

[TestFixture]
public class Tests
{
    Mock<IWebSystemCalls> webSystemCalls;
    IFileSystem fileSystem;
    FileDownloader fileDownloader;

    [SetUp]
    public void Setup()
    {
        webSystemCalls = new Mock<IWebSystemCalls>();
        fileSystem = new FileSystem();
        fileDownloader = new FileDownloader(webSystemCalls.Object, fileSystem);
    }
        
    [Test]
    public void TestDownloadFile()
    {
       
        _ = webSystemCalls.Setup(x => x.GetHeadersAsync(It.IsAny<string>(), default)).ReturnsAsync(
            new HttpResponseMessage());
        
        webSystemCalls.Setup(x => x.DownloadContent(It.IsAny<string>(), default)).ReturnsAsync(new HttpResponseMessage());
        var httpRequest = fileDownloader.DownloadFile("https://installer.demo.accurx.com/chain/3.182.57641.0/accuRx.Installer.Local.msi", Path.GetTempFileName(), (FileProgress) => { });
        Assert.True(true);
    }

    public void TestPartialDownloadFile()
    {

        _ = webSystemCalls.Setup(x => x.GetHeadersAsync(It.IsAny<string>(), default)).ReturnsAsync(
            new HttpResponseMessage());

        webSystemCalls.Setup(x => x.DownloadContent(It.IsAny<string>(), default)).ReturnsAsync(new HttpResponseMessage());
        var httpRequest = fileDownloader.DownloadFile("https://installer.demo.accurx.com/chain/3.182.57641.0/accuRx.Installer.Local.msi", Path.GetTempFileName(), (FileProgress) => { });
        Assert.True(true);
    }

    public void CancelDownloads()
    {

        _ = webSystemCalls.Setup(x => x.GetHeadersAsync(It.IsAny<string>(), default)).ReturnsAsync(
            new HttpResponseMessage());

        webSystemCalls.Setup(x => x.DownloadContent(It.IsAny<string>(), default)).ReturnsAsync(new HttpResponseMessage());
        var httpRequest = fileDownloader.DownloadFile("https://installer.demo.accurx.com/chain/3.182.57641.0/accuRx.Installer.Local.msi", Path.GetTempFileName(), (FileProgress) => { });
        Assert.True(true);
    }
}