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

using RabbitMq.Dlx.Demo.Model;
using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;
using System.Text;
using System.Text.Json;

const string brokerUri = "amqp://guest:guest@localhost:5672/%2f"; // Define o endereço e as credenciais do RabbitMQ.

const string queueName = "pedido.criados"; // Define o nome da fila principal.
const string deadLetterExchange = "pedido.dlx.exchange"; // Define o nome da Dead Letter Exchange.
const string deadLetterQueue = "pedido.falhas"; // Define a fila que armazenará as mensagens mortas.
const string deadLetterRoutingKey = "pedido.falha"; // Define a chave usada para rotear mensagens mortas.

IEnvironment? environment = null; // Armazena o ambiente do cliente AMQP.
IConnection? connection = null;  // Armazena a conexão com o RabbitMQ.
IPublisher? publisher = null;   // Armazena o publicador de mensagens.

try
{
    ConnectionSettings settings = ConnectionSettingsBuilder.Create() // Inicia a criação das configurações da conexão.
        .Uri(new Uri(brokerUri))  // Define a URI do RabbitMQ.
        .ContainerId("pedido-producer")  // Identifica esta aplicação no RabbitMQ.
        .Build();  // Finaliza as configurações.

    environment = AmqpEnvironment.Create(settings); // Cria o ambiente AMQP com as configurações.

    connection = await environment.CreateConnectionAsync(); // Abre a conexão com o RabbitMQ.

    IManagement management = connection.Management(); // Obtém a API de gerenciamento da topologia.

    
    IExchangeSpecification exchangeSpec = management  // Inicia a configuração da DLX.
    .Exchange(deadLetterExchange)  // Define o nome da exchange.
    .Type("direct"); // Define o roteamento exato por chave.

    await exchangeSpec.DeclareAsync();  // Declara a DLX no RabbitMQ.

    
    IQueueSpecification deadLetterQueueSpec = management // Inicia a configuração da fila de mensagens mortas.
        .Queue(deadLetterQueue) // Define o nome da fila de mensagens mortas.
        .Type(QueueType.QUORUM); // Define a fila como quorum.

    await deadLetterQueueSpec.DeclareAsync();  // Declara a fila de mensagens mortas no RabbitMQ.

    
    await management   // Inicia a criação do vínculo de roteamento.
        .Binding() // Cria uma especificação de binding.
        .SourceExchange(exchangeSpec)  // Define a DLX como origem.
        .DestinationQueue(deadLetterQueueSpec) // Define a fila de mensagens mortas como destino.
        .Key(deadLetterRoutingKey) // Define a chave aceita pelo binding.
        .BindAsync(); // Cria o binding no RabbitMQ.

    
    IQueueSpecification queueSpec = management // Inicia a configuração da fila principal.
        .Queue(queueName) // Define o nome da fila principal.
        .Type(QueueType.QUORUM); // Define a fila como quorum.

    await queueSpec.DeclareAsync(); // Declara a fila e permite a aplicação da policy.

    publisher = await connection // Inicia a criação do publicador.
        .PublisherBuilder()  // Cria um publicador configurável.
        .Queue(queueName) // Define a fila principal como destino.
        .BuildAsync();  // Finaliza a criação do publicador.


   // foreach (Pedido pedido in CriarPedidosFake())
        foreach (Pedido pedido in CriarPedidosComErroFake())
        {
        JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true
        };
        // Serializa o pedido para JSON.
        string pedidoJson = JsonSerializer.Serialize(pedido, jsonOptions);

        // Cria explicitamente uma mensagem durável. 
        IMessage message = new AmqpMessage(
            Encoding.UTF8.GetBytes(pedidoJson))
            .Durable(true);  // Inicia a criação da mensagem AMQP.

        PublishResult result =
            await publisher.PublishAsync(message);   // Publica a mensagem e recebe o resultado.

        if (result.Outcome.State != OutcomeState.Accepted) // Verifica se o RabbitMQ não aceitou a publicação.
        {
            Console.Error.WriteLine(
                $"Publicação não aceita: {result.Outcome.State}");

            Environment.ExitCode = 1; // Define o código de saída como erro.
            break;  // Interrompe o envio das próximas mensagens.
        }

        Console.WriteLine(
            $"Mensagem enviada com sucesso: {pedidoJson}"); // Informa que a mensagem foi publicada.
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
    if (publisher is not null) // Verifica se o publicador foi criado.
    {
        await publisher.CloseAsync(); // Encerra o publicador.
    }

    if (connection is not null) // Verifica se a conexão foi criada.
    {
        await connection.CloseAsync();  // Encerra a conexão.
    }

    if (environment is not null)  // Verifica se o ambiente foi criado.
    {
        await environment.CloseAsync(); // Encerra o ambiente AMQP.
    }

    Console.WriteLine();
    Console.WriteLine("Pressione ENTER para encerrar...");
    Console.ReadLine();
}
static List<Pedido> CriarPedidosFake()
{
    return Enumerable.Range(1, 5)
        .Select(index => new Pedido
        {
            Id = Guid.NewGuid(),
            ClienteEmail = $"cliente{index}@email.com",
            ValorTotal = Random.Shared.Next(-100, 5000),
            DataCriacao = DateTime.UtcNow,
            Itens =
            [
                new Item
                {
                    NomeProduto = $"Produto {index}",
                    Quantidade = Random.Shared.Next(1, 5),
                    PrecoUnitario = Random.Shared.Next(20, 1000)
                }
            ]
        })
        .ToList();
}

static List<Pedido> CriarPedidosComErroFake()
{
    var valor = -100;
    return Enumerable.Range(1, 2)
        .Select(index => new Pedido
        {
            Id = Guid.NewGuid(),
            ClienteEmail = $"cliente{index}@email.com",
            ValorTotal = valor,
            DataCriacao = DateTime.UtcNow,
            Itens =
            [
                new Item
                {
                    NomeProduto = $"Produto {index}",
                    Quantidade = Random.Shared.Next(1, 5),
                    PrecoUnitario = Random.Shared.Next(20, 1000)
                }
            ]
        })
        .ToList();
}