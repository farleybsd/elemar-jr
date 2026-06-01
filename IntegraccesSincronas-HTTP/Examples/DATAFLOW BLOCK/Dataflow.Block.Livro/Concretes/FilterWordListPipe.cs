using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks.Dataflow;

namespace Dataflow.Block.Livro;

public sealed class FilterWordListPipe : IFilterWordListPipe
{
    private readonly IOutputWriter _outputWriter;

    public FilterWordListPipe(IOutputWriter outputWriter)
    {
        _outputWriter = outputWriter;
    }

    // Remove palavras curtas e duplicadas.
    public TransformBlock<string[], string[]> Create(ExecutionDataflowBlockOptions executionOptions)
    {
        return new TransformBlock<string[], string[]>(words =>
        {
            _outputWriter.WriteLine("Filtering word list...");

            return words
                .Where(word => word.Length > 3)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }, executionOptions);
    }
}
