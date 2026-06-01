using Dataflow.Block.Livro;
using NSubstitute;
using System.Threading.Tasks.Dataflow;

namespace Dataflow.Block.Livro.Tests;

public sealed class PipeTests
{
    private static ExecutionDataflowBlockOptions CreateExecutionOptions()
    {
        return new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 1
        };
    }

    [Fact]
    public async Task DownloadStringPipe_DeveBaixarTextoUsandoDownloader()
    {
        var downloader = Substitute.For<ITextDownloader>();
        var outputWriter = Substitute.For<IOutputWriter>();

        downloader
            .DownloadAsync("fake-url")
            .Returns("amor roma casa");

        var pipe = new DownloadStringPipe(downloader, outputWriter);
        var block = pipe.Create(CreateExecutionOptions());

        await block.SendAsync("fake-url");
        block.Complete();

        string result = await block.ReceiveAsync();

        Assert.Equal("amor roma casa", result);
        await downloader.Received(1).DownloadAsync("fake-url");
        outputWriter.Received(1).WriteLine("Downloading 'fake-url'...");
    }

    [Fact]
    public async Task CreateWordListPipe_DeveRemoverPontuacaoESepararPalavras()
    {
        var outputWriter = Substitute.For<IOutputWriter>();

        var pipe = new CreateWordListPipe(outputWriter);
        var block = pipe.Create(CreateExecutionOptions());

        await block.SendAsync("Ola, mundo! Amor/Roma.");
        block.Complete();

        string[] result = await block.ReceiveAsync();

        Assert.Equal(new[] { "Ola", "mundo", "Amor", "Roma" }, result);
        outputWriter.Received(1).WriteLine("Creating word list...");
    }

    [Fact]
    public async Task FilterWordListPipe_DeveRemoverPalavrasCurtasEDuplicadas()
    {
        var outputWriter = Substitute.For<IOutputWriter>();

        var pipe = new FilterWordListPipe(outputWriter);
        var block = pipe.Create(CreateExecutionOptions());

        await block.SendAsync(new[] { "amor", "sol", "AMOR", "roma", "casa" });
        block.Complete();

        string[] result = await block.ReceiveAsync();

        Assert.Equal(new[] { "amor", "roma", "casa" }, result);
        outputWriter.Received(1).WriteLine("Filtering word list...");
    }

    [Fact]
    public async Task FindReversedWordsPipe_DeveEncontrarPalavrasCujoReversoExiste()
    {
        var outputWriter = Substitute.For<IOutputWriter>();

        var pipe = new FindReversedWordsPipe(outputWriter);
        var block = pipe.Create(CreateExecutionOptions());

        await block.SendAsync(new[] { "amor", "roma", "casa", "teste" });
        block.Complete();

        var result = new List<string>();

        while (await block.OutputAvailableAsync())
        {
            result.Add(await block.ReceiveAsync());
        }

        Assert.Equal(new[] { "amor", "roma" }, result);
        outputWriter.Received(1).WriteLine("Finding reversed words...");
    }

    [Fact]
    public async Task PrintReversedWordsPipe_DeveImprimirPalavraESeuReverso()
    {
        var outputWriter = Substitute.For<IOutputWriter>();

        var pipe = new PrintReversedWordsPipe(outputWriter);
        var block = pipe.Create(CreateExecutionOptions());

        await block.SendAsync("amor");
        block.Complete();
        await block.Completion;

        outputWriter.Received(1).WriteLine("Found reversed words amor/roma");
    }
}
