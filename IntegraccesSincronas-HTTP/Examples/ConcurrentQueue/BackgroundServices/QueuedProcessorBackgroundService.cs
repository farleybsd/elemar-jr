namespace ConcurrentQueue.BackgroundServices;

/// <summary>
/// Criando o processador em segundo plano
//Com a fila configurada, precisamos de um serviço em segundo plano para processar as tarefas enfileiradas.
//O BackgroundService do ASP.NET Core é o candidato ideal para isso:
/// </summary>
public sealed class QueuedProcessorBackgroundService : BackgroundService
{
    // Armazena a fila que contém as tarefas pendentes.
    private readonly IBackgroundTaskQueue _taskQueue;

    // Permite criar escopos para resolver dependências Scoped.
    private readonly IServiceScopeFactory _scopeFactory;

    // Registra informações e erros ocorridos no processamento.
    private readonly ILogger<QueuedProcessorBackgroundService> _logger;

    public QueuedProcessorBackgroundService(
        IBackgroundTaskQueue taskQueue,
        IServiceScopeFactory scopeFactory,
        ILogger<QueuedProcessorBackgroundService> logger)
    {
        _taskQueue = taskQueue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    // Método chamado automaticamente quando a aplicação é iniciada.
    protected override async Task ExecuteAsync(
        CancellationToken cancellationToken)
    {
        // Continua processando enquanto o encerramento não for solicitado.
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Aguarda e retira a próxima tarefa disponível na fila.
                var workItem =
                    await _taskQueue.DequeueAsync(cancellationToken);

                // Cria um escopo assíncrono para usar serviços Scoped.
                await using var scope = _scopeFactory.CreateAsyncScope();

                // Executa a tarefa retirada da fila.
                await workItem(
                    // Fornece o provedor de serviços do escopo atual.
                    scope.ServiceProvider,
                    // Permite que a tarefa seja cancelada no encerramento.
                    cancellationToken);
            }
            // Captura um cancelamento causado pelo encerramento da aplicação.
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)   // Confirma que o token realmente solicitou o cancelamento.
            {
                break;
            }
            catch (Exception ex) // Captura qualquer outro erro ocorrido no processamento.
            {
                _logger.LogError(
                    ex,
                    "Ocorreu um erro ao executar workItem.");
            }
        }
    }
}
