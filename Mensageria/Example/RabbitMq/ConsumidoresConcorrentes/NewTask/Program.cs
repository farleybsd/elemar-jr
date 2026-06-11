

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

// Obtém a tarefa passada pelo terminal.
string body = args.Length > 0
    ? string.Join(" ", args)
    : "Hello World!";

ConnectionSettings settings = ConnectionSettingsBuilder.Create()
    .Uri(new Uri(brokerUri))
    .ContainerId("tutorial-new-task")
    .Build();

IEnvironment environment = AmqpEnvironment.Create(settings);
IConnection connection = await environment.CreateConnectionAsync();

try
{
    IManagement management = connection.Management();

    IQueueSpecification queueSpec = management
                                            .Queue(taskQueueName)
                                            .Type(QueueType.QUORUM);

    await queueSpec.DeclareAsync();

    IPublisher publisher = await connection
                                            .PublisherBuilder()
                                            .Queue(taskQueueName)
                                            .BuildAsync();

    try
    {
        // Converte a tarefa em bytes e cria a mensagem AMQP.
        var message = new AmqpMessage(
            Encoding.UTF8.GetBytes(body));

        // Publica a mensagem e aguarda o resultado.
        PublishResult result =
            await publisher.PublishAsync(message);

        // Verifica se o RabbitMQ aceitou a mensagem.
        if (result.Outcome.State != OutcomeState.Accepted)
        {
            Console.Error.WriteLine(
                $"Publicação não aceita: {result.Outcome.State}");

            Environment.ExitCode = 1;
            return;
        }

        Console.WriteLine($" [x] Sent '{body}'");
    }
    finally
    {
        await publisher.CloseAsync();
    }
}
finally
{
    await connection.CloseAsync();
    await environment.CloseAsync();
}