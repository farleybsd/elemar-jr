using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks.Dataflow;

namespace Dataflow.Block.Livro;

public sealed class FindReversedWordsPipe : IFindReversedWordsPipe
{
    private readonly IOutputWriter _outputWriter;

    public FindReversedWordsPipe(IOutputWriter outputWriter)
    {
        _outputWriter = outputWriter;
    }

    // Encontra todas as palavras na coleção especificada cujo reverso também existe na coleção.
    public TransformManyBlock<string[], string> Create(ExecutionDataflowBlockOptions executionOptions)
    {
        return new TransformManyBlock<string[], string>(words =>
        {
            _outputWriter.WriteLine("Finding reversed words...");

            // HashSet melhora a busca, evitando procurar palavra por palavra em todo o array.
            var wordsSet = new HashSet<string>(
                words,
                StringComparer.OrdinalIgnoreCase);

            return from word in words
                   let reverse = new string(word.Reverse().ToArray())
                   where !word.Equals(reverse, StringComparison.OrdinalIgnoreCase)
                   where wordsSet.Contains(reverse)
                   select word;
        }, executionOptions);
    }
}