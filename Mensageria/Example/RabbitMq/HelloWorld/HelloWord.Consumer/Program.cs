using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;
using System.Text;

const string brokerUri = "amqp://guest:guest@localhost:5672/%2f";

ConnectionSettings settings = ConnectionSettingsBuilder.Create()
    .Uri(new Uri(brokerUri))
    .ContainerId("tutorial-receive")
    .Build();

IEnvironment environment = AmqpEnvironment.Create(settings);
IConnection connection = await environment.CreateConnectionAsync();

try
{
    IManagement management = connection.Management();

    IQueueSpecification queueSpec = management
        .Queue("hello")
        .Type(QueueType.QUORUM);

    await queueSpec.DeclareAsync();

    IConsumer consumer = await connection.ConsumerBuilder()
        .Queue("hello")
        .MessageHandler((ctx, message) =>
        {
            string body = Encoding.UTF8.GetString(message.Body()!);

            Console.WriteLine($"Mensagem recebida: {body}");

            // Confirma que a mensagem foi processada.
            ctx.Accept();

            return Task.CompletedTask;
        })
        .BuildAndStartAsync();

    try
    {
        // Cria um mecanismo para detectar quando Ctrl+C for pressionado.
        using var cancellation = new CancellationTokenSource();

        Console.CancelKeyPress += (_, eventArgs) =>
        {
            // Impede que o processo seja encerrado imediatamente.
            eventArgs.Cancel = true;

            // Libera a espera infinita abaixo.
            cancellation.Cancel();
        };

        Console.WriteLine("Aguardando mensagens. Pressione Ctrl+C para encerrar.");

        // Mantém o programa ativo até o usuário pressionar Ctrl+C.
        await Task.Delay(Timeout.Infinite, cancellation.Token);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Encerrando consumidor...");
    }
    finally
    {
        // Fecha o consumidor.
        await consumer.CloseAsync();
    }
}
finally
{
    // Fecha a conexão com o RabbitMQ.
    await connection.CloseAsync();

    // Encerra o ambiente AMQP.
    await environment.CloseAsync();
}