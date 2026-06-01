using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks.Dataflow;

namespace Dataflow.Block.Livro;

public sealed class PrintReversedWordsPipe : IPrintReversedWordsPipe
{
    private readonly IOutputWriter _outputWriter;

    public PrintReversedWordsPipe(IOutputWriter outputWriter)
    {
        _outputWriter = outputWriter;
    }

    // Imprime as palavras fornecidas em ordem inversa no console.
    public ActionBlock<string> Create(ExecutionDataflowBlockOptions executionOptions)
    {
        return new ActionBlock<string>(reversedWord =>
        {
            string reversed = new string(reversedWord.Reverse().ToArray());

            _outputWriter.WriteLine($"Found reversed words {reversedWord}/{reversed}");
        }, executionOptions);
    }
}
