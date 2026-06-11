/*
 Name: pedido-dlx-policy
Pattern: ^pedido\.criados$
Apply to: Queues
Priority: 7
dead-letter-routing-key: pedido.falha
dead-letter-exchange: pedido.dlx.exchange

services:
  rabbitmq:
    image: rabbitmq:4-management
    volumes:
      - rabbitmq_data:/var/lib/rabbitmq

volumes:
  rabbitmq_data:
*/

using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;
using System.Text;

const string brokerUri =
    "amqp://guest:guest@localhost:5672/%2f";

const string queueName = "pedido.criados";

IEnvironment? environment = null;
IConnection? connection = null;
IPublisher? publisher = null;

try
{
    ConnectionSettings settings = ConnectionSettingsBuilder.Create()
        .Uri(new Uri(brokerUri))
        .ContainerId("pedido-producer")
        .Build();

    environment = AmqpEnvironment.Create(settings);
    connection = await environment.CreateConnectionAsync();

    IManagement management = connection.Management();

    IQueueSpecification queueSpec = management
        .Queue(queueName)
        .Type(QueueType.QUORUM);

    await queueSpec.DeclareAsync();

    publisher = await connection
        .PublisherBuilder()
        .Queue(queueName)
        .BuildAsync();

    List<string> messages =
    [
        "Farley 1",
        "Farley 2",
        "Farley 3"
    ];

    foreach (string body in messages)
    {
        // Cria explicitamente uma mensagem durável.
        IMessage message = new AmqpMessage(
            Encoding.UTF8.GetBytes(body))
            .Durable(true);

        PublishResult result =
            await publisher.PublishAsync(message);

        if (result.Outcome.State != OutcomeState.Accepted)
        {
            Console.Error.WriteLine(
                $"Publicação não aceita: {result.Outcome.State}");

            Environment.ExitCode = 1;
            break;
        }

        Console.WriteLine(
            $"Mensagem enviada com sucesso: {body}");
    }
}
catch (Exception exception)
{
    // Exibe o erro em vez de fechar o console sem explicação.
    Console.Error.WriteLine();
    Console.Error.WriteLine("Ocorreu um erro:");
    Console.Error.WriteLine(exception.Message);

    Environment.ExitCode = 1;
}
finally
{
    if (publisher is not null)
    {
        await publisher.CloseAsync();
    }

    if (connection is not null)
    {
        await connection.CloseAsync();
    }

    if (environment is not null)
    {
        await environment.CloseAsync();
    }

    Console.WriteLine();
    Console.WriteLine("Pressione ENTER para encerrar...");
    Console.ReadLine();
}