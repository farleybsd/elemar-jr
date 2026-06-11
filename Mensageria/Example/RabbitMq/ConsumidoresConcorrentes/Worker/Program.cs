using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;
using System.Text;

/// <Produtor>
/// dotnet run --project NewTask/NewTask.csproj "First message."
/// dotnet run --project NewTask/NewTask.csproj "Second message.."
///  Agora, enviaremos strings que representam tarefas complexas.
///  Simulamos o trabalho colocando um "sleep" em cada string: 
///  cada caractere .na string adiciona um segundo.
/// </summary>
/// 
const string taskQueueName = "tarefas_complexas";
const string brokerUri = "amqp://guest:guest@localhost:5672/%2f";

ConnectionSettings settings = ConnectionSettingsBuilder.Create()
    .Uri(new Uri(brokerUri))
    // Cada Worker recebe uma identificação própria.
    .ContainerId($"tutorial-worker-{Guid.NewGuid()}")
    .Build();

IEnvironment environment = AmqpEnvironment.Create(settings);
IConnection connection = await environment.CreateConnectionAsync();

try
{
    // Garante que a fila existe e possui o mesmo tipo usado pelo produtor.
    IManagement management = connection.Management();

    await management
        .Queue(taskQueueName)
        .Type(QueueType.QUORUM)
        .DeclareAsync();

    IConsumer consumer = await connection
        .ConsumerBuilder()
        .Queue(taskQueueName)

        // Entrega somente uma mensagem não confirmada por vez.
        .InitialCredits(1)

        .MessageHandler((ctx, message) =>
        {
            string body =
                Encoding.UTF8.GetString(message.Body()!);

            Console.WriteLine($" [x] Received '{body}'");

            try
            {
                DoWork(body);
                Console.WriteLine(" [x] Done");
            }
            finally
            {
                // Confirma o processamento e remove a mensagem da fila.
                ctx.Accept();
            }

            return Task.CompletedTask;
        })
        .BuildAndStartAsync();

    try
    {
        using var cancellation = new CancellationTokenSource();

        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellation.Cancel();
        };

        Console.WriteLine(
            " [*] Aguardando tarefas. Pressione Ctrl+C para sair.");

        // Impede que o programa termine enquanto aguarda mensagens.
        await Task.Delay(Timeout.Infinite, cancellation.Token);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Encerrando Worker...");
    }
    finally
    {
        await consumer.CloseAsync();
    }
}
finally
{
    // Fecha os recursos corretamente, evitando o aviso no RabbitMQ.
    await connection.CloseAsync();
    await environment.CloseAsync();
}

static void DoWork(string task)
{
    foreach (char character in task)
    {
        if (character == '.')
        {
            Thread.Sleep(1000);
        }
    }
}