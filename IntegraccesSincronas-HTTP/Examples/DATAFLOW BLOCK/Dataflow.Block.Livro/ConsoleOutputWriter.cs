namespace Dataflow.Block.Livro;

public sealed class ConsoleOutputWriter : IOutputWriter
{
    // Escreve uma mensagem no console.
    public void WriteLine(string message)
    {
        Console.WriteLine(message);
    }
}