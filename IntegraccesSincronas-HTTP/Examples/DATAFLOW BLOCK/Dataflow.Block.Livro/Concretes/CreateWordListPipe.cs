using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks.Dataflow;

namespace Dataflow.Block.Livro;

public sealed class CreateWordListPipe : ICreateWordListPipe
{
    private readonly IOutputWriter _outputWriter;

    public CreateWordListPipe(IOutputWriter outputWriter)
    {
        _outputWriter = outputWriter;
    }

    // Separa o texto especificado em uma matriz de palavras.
    public TransformBlock<string, string[]> Create(ExecutionDataflowBlockOptions executionOptions)
    {
        return new TransformBlock<string, string[]>(text =>
        {
            _outputWriter.WriteLine("Creating word list...");

            // Remove pontuação e outros caracteres que não são letras,
            // substituindo por espaço para facilitar a separação das palavras.
            char[] tokens = text
                .Select(c => char.IsLetter(c) ? c : ' ')
                .ToArray();

            text = new string(tokens);

            return text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }, executionOptions);
    }
}
