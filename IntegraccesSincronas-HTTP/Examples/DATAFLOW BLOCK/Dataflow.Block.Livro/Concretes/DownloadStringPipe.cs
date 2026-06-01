using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks.Dataflow;

namespace Dataflow.Block.Livro;

public sealed class DownloadStringPipe : IDownloadStringPipe
{
    private readonly ITextDownloader _textDownloader;
    private readonly IOutputWriter _outputWriter;

    public DownloadStringPipe(
        ITextDownloader textDownloader,
        IOutputWriter outputWriter)
    {
        _textDownloader = textDownloader;
        _outputWriter = outputWriter;
    }

    // Faz o download do recurso solicitado como uma string.
    public TransformBlock<string, string> Create(ExecutionDataflowBlockOptions executionOptions)
    {
        return new TransformBlock<string, string>(async uri =>
        {
            _outputWriter.WriteLine($"Downloading '{uri}'...");

            return await _textDownloader.DownloadAsync(uri);
        }, executionOptions);
    }
}
