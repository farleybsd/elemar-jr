using System.Collections.Concurrent;

namespace ConcurrentQueue;
/// <summary>
/// Essa classe nos permite enfileirar tarefas de forma thread-safe usando ConcurrentQueue
/// e sinaliza o serviço em segundo plano para iniciar o processamento quando uma tarefa é adicionada.
/// </summary>
public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    // Controla a quantidade de tarefas disponíveis na fila.
    // O valor inicial zero indica que não há tarefas disponíveis.
    private readonly SemaphoreSlim _signal = new(0);

    // Armazena funções assíncronas que serão executadas posteriormente.
    private readonly ConcurrentQueue<Func<IServiceProvider, CancellationToken, Task>> _workItems = new();

    // Recebe uma função assíncrona e a adiciona à fila.
    public void QueueBackgroundWorkItem(Func<IServiceProvider, CancellationToken, Task> workItem)
    {
        // Impede que uma função nula seja adicionada à fila.
        if (workItem == null) throw new ArgumentNullException(nameof(workItem));

        // Adiciona a função ao final da fila de forma thread-safe.
        _workItems.Enqueue(workItem);
        // Incrementa o semáforo e libera um consumidor que esteja aguardando.
        _signal.Release(); // Signal that a new item is available
    }

    // Aguarda e retorna a próxima função disponível na fila.
    public async Task<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        // Aguarda até que Release seja chamado ou a operação seja cancelada.
        await _signal.WaitAsync(cancellationToken);
        // Remove a função mais antiga da fila e a coloca em workItem.
        _workItems.TryDequeue(out var workItem);

        // Retorna a função removida.
        // O operador ! informa ao compilador que ela não será nula.
        return workItem!;
    }
}
