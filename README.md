# FileDownloader

## Task

To release software to many hundreds of thousands of clinicians multiple times a week, our component for downloading updates needs to be reliable in the challenging networking conditions clinicians can face: intermittent internet disconnection and slow internet speeds. 

In this task, performing a normal GET request on a file won't be reliable for two reasons. Firstly, we need to be able to recover from internet disconnections. Secondly, we need to not have to start from scratch every time, with intermittent internet disconnection and slow internet, it's unlikely we'll be able to download the whole file in one go. Luckily, some CDNs support downloading partial content so if we can get part of the way through, we can resume from this point. If the URL does not support partial content then we attempt to just download the whole file.

The solution implements the following core requirements:
- Download the installer even when internet disconnections occur (we use 2 minutes as a disconnection time benchmark for this)
- Implement partial downloading so that we don’t need to start from scratch every time, if the CDN supports this
- Implement downloading the file in one go, if the CDN does not support partial downloading
- Recover from failures and not exit until the file has been successfully downloaded
- Check the integrity of the file after downloading and delete the file if this check fails. You can use the Content-MD5 for this: https://www.oreilly.com/library/view/http-the-definitive/1565925092/re17.html
- Report progress to the user throughout the download
- Add the ability to cancel so the user can stop any in progress downloads


### .NET 

```IWebSystemCalls.cs``` allows you to get the HTTP headers for a URL, download the whole content, or download the partial content. All these calls return an ```HttpResponseMessage``` object which contains properties for headers and the content.

As in the example here: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Accept-Ranges, the HTTP header "Accept Ranges" will be set to "Bytes" if the CDN supports partial content.
