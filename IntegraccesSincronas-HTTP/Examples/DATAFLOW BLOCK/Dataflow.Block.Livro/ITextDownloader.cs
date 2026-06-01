namespace Dataflow.Block.Livro;

public interface ITextDownloader
{
    Task<string> DownloadAsync(string uri);
}