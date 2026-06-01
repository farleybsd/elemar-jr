using Dataflow.Block.Livro;
using NSubstitute;

namespace Dataflow.Block.Livro.Tests;

public sealed class PipelinesTests
{
    [Fact]
    public async Task ExecuteAsync_DeveExecutarPipelineCompletoUsandoMocks()
    {
        var downloader = Substitute.For<ITextDownloader>();
        var outputWriter = Substitute.For<IOutputWriter>();

        downloader
            .DownloadAsync("fake-url")
            .Returns("amor, roma! casa teste");

        var pipeline = new Pipelines(
            new DownloadStringPipe(downloader, outputWriter),
            new CreateWordListPipe(outputWriter),
            new FilterWordListPipe(outputWriter),
            new FindReversedWordsPipe(outputWriter),
            new PrintReversedWordsPipe(outputWriter));

        await pipeline.ExecuteAsync("fake-url");

        await downloader.Received(1).DownloadAsync("fake-url");
        outputWriter.Received(1).WriteLine("Downloading 'fake-url'...");
        outputWriter.Received(1).WriteLine("Creating word list...");
        outputWriter.Received(1).WriteLine("Filtering word list...");
        outputWriter.Received(1).WriteLine("Finding reversed words...");
        outputWriter.Received(1).WriteLine("Found reversed words amor/roma");
        outputWriter.Received(1).WriteLine("Found reversed words roma/amor");
    }
}
