using System.Threading.Tasks.Dataflow;

namespace Dataflow.Block.Livro;

public sealed class Pipelines
{
    private readonly TransformBlock<string, string> _downloadString;
    private readonly TransformBlock<string, string[]> _createWordList;
    private readonly TransformBlock<string[], string[]> _filterWordList;
    private readonly TransformManyBlock<string[], string> _findReversedWords;
    private readonly ActionBlock<string> _printReversedWords;

    public Pipelines(
        IDownloadStringPipe downloadStringPipe,
        ICreateWordListPipe createWordListPipe,
        IFilterWordListPipe filterWordListPipe,
        IFindReversedWordsPipe findReversedWordsPipe,
        IPrintReversedWordsPipe printReversedWordsPipe)
    {
        // Opções compartilhadas por todos os blocos.
        // MaxDegreeOfParallelism = 1 faz cada bloco processar um item por vez.
        var executionOptions = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 1
        };

        // Cria cada etapa do pipeline usando as classes próprias de cada pipe.
        _downloadString = downloadStringPipe.Create(executionOptions);
        _createWordList = createWordListPipe.Create(executionOptions);
        _filterWordList = filterWordListPipe.Create(executionOptions);
        _findReversedWords = findReversedWordsPipe.Create(executionOptions);
        _printReversedWords = printReversedWordsPipe.Create(executionOptions);

        // Conecta as etapas para formar o fluxo completo.
        LinkPipeline();
    }

    // Ponto de entrada público do pipeline.
    // Envia a URL para a primeira etapa, finaliza a entrada e aguarda o último bloco terminar.
    public async Task ExecuteAsync(string uri)
    {
        await _downloadString.SendAsync(uri);

        _downloadString.Complete();

        await _printReversedWords.Completion;
    }

    // Conecta a saída de cada bloco na entrada do próximo bloco.
    // PropagateCompletion = true faz o sinal de conclusão avançar até o final do pipeline.
    private void LinkPipeline()
    {
        var linkOptions = new DataflowLinkOptions
        {
            PropagateCompletion = true
        };

        _downloadString.LinkTo(_createWordList, linkOptions);
        _createWordList.LinkTo(_filterWordList, linkOptions);
        _filterWordList.LinkTo(_findReversedWords, linkOptions);
        _findReversedWords.LinkTo(_printReversedWords, linkOptions);
    }
}