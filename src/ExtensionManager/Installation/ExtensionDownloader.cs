using System.Net.Http;

using ExtensionManager.VisualStudio.Extensions;

using Task = System.Threading.Tasks.Task;

namespace ExtensionManager.Installation;

internal sealed class ExtensionDownloader
{
    private readonly HttpMessageHandler _httpMessageHandler;
    private readonly IProgress<DownloadResult> _progress;
    private readonly CancellationToken _cancellationToken;

    public Uri DownloadUri { get; }
    public string TargetFilePath { get; }
    public IVSExtension Extension { get; }

    public Task DownloadTask { get; private set; }
    public Exception? DownloadException { get; private set; }

    public ExtensionDownloader(HttpMessageHandler httpMessageHandler, IProgress<DownloadResult> progress, Uri downloadUri, string targetFilePath, IVSExtension extension, CancellationToken cancellationToken)
    {
        _httpMessageHandler = httpMessageHandler;
        _progress = progress;
        _cancellationToken = cancellationToken;

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
        _cancellationToken.ThrowIfCancellationRequested();

        await Task.Delay(1000, _cancellationToken);

        try
        {
            using (var client = new HttpClient(_httpMessageHandler, disposeHandler: false))
            using (var response = await client.GetAsync(DownloadUri, _cancellationToken).ConfigureAwait(false))
            using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            using (var targetStream = new FileStream(TargetFilePath, FileMode.Create, FileAccess.Write))
                await responseStream.CopyToAsync(targetStream, 81920, _cancellationToken).ConfigureAwait(false);

            _progress.Report(DownloadResult.Success);
        }
        catch (Exception ex)
        {
            _progress.Report(DownloadResult.Failure);

            DownloadException = ex;
        }
    }
}
