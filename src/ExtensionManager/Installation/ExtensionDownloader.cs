using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

using ExtensionManager.VisualStudio.Extensions;

using Task = System.Threading.Tasks.Task;

namespace ExtensionManager.Installation;

internal sealed class ExtensionDownloader
{
    private readonly HttpMessageHandler _httpMessageHandler;
    private readonly IProgress<DownloadResult> _progress;

    public Uri DownloadUri { get; }
    public string TargetFilePath { get; }
    public IVSExtension Extension { get; }

    public Task DownloadTask { get; private set; }
    public Exception? DownloadException { get; private set; }

    public ExtensionDownloader(HttpMessageHandler httpMessageHandler, IProgress<DownloadResult> progress, Uri downloadUri, string targetFilePath, IVSExtension extension)
    {
        _httpMessageHandler = httpMessageHandler;
        _progress = progress;

        DownloadUri = downloadUri;
        TargetFilePath = targetFilePath;
        Extension = extension;

        DownloadTask = Task.FromException(new InvalidOperationException("The download has not yet been started"));
    }

    public void BeginDownload()
    {
        DownloadException = null;
        DownloadTask = DownloadAsync();
    }

    private async Task DownloadAsync()
    {
        try
        {
            using (var client = new HttpClient(_httpMessageHandler, disposeHandler: false))
            using (var response = await client.GetAsync(DownloadUri).ConfigureAwait(false))
            using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            using (var targetStream = new FileStream(TargetFilePath, FileMode.Create, FileAccess.Write))
                await responseStream.CopyToAsync(targetStream, 81920).ConfigureAwait(false);

            _progress.Report(DownloadResult.Success);
        }
        catch (Exception ex)
        {
            _progress.Report(DownloadResult.Failure);

            DownloadException = ex;
        }
    }
}
