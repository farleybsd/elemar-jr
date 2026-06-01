namespace Dataflow.Block.Livro;

public sealed class HttpTextDownloader : ITextDownloader, IDisposable
{
    private readonly HttpClient _httpClient;

    public HttpTextDownloader()
    {
        _httpClient = new HttpClient(
            new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip
            });
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    // Faz o download do recurso solicitado como uma string.
    public Task<string> DownloadAsync(string uri)
    {
        return _httpClient.GetStringAsync(uri);
    }
}