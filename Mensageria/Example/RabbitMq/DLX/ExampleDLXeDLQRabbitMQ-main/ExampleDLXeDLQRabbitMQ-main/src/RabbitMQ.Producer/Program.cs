using RabbitMQ.Client;
using RabbitMQ.Model;

const string exchangeName = "pedido.exchange";
const string queueName = "pedido.criados";
const string routingKey = "pedido.criado";

const string dlxExchangeName = "pedido.dlx.exchange";
const string dlqQueueName = "pedido.dlq";
const string dlxRoutingKey = "pedido.falha";

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest",
    VirtualHost = "/",
    AutomaticRecoveryEnabled = true,
    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
};

await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

Console.WriteLine("===========================================");
Console.WriteLine("🚀 CONFIGURANDO EXCHANGES E FILAS...");
Console.WriteLine("===========================================");


await channel.ExchangeDeclareAsync(
    exchange: exchangeName,
    type: ExchangeType.Direct,
    durable: true,
    autoDelete: false);
Console.WriteLine($"✅ Exchange principal criado: {exchangeName}");


await channel.ExchangeDeclareAsync(
    exchange: dlxExchangeName,
    type: ExchangeType.Direct,
    durable: true,
    autoDelete: false);
Console.WriteLine($"✅ DLX criado: {dlxExchangeName}");


await channel.QueueDeclareAsync(
    queue: dlqQueueName,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);
Console.WriteLine($"✅ DLQ criada: {dlqQueueName}");


await channel.QueueBindAsync(
    queue: dlqQueueName,
    exchange: dlxExchangeName,
    routingKey: dlxRoutingKey);
Console.WriteLine($"✅ DLQ conectada à DLX com routing key: {dlxRoutingKey}");


var mainQueueArgs = new Dictionary<string, object>
{
    { "x-dead-letter-exchange", dlxExchangeName },
    { "x-dead-letter-routing-key", dlxRoutingKey }
};

await channel.QueueDeclareAsync(
    queue: queueName,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: mainQueueArgs); // ← AQUI! Arguments com DLX
Console.WriteLine($"✅ Fila principal criada: {queueName}");
Console.WriteLine($"   └─ Configurada para usar DLX: {dlxExchangeName}");


await channel.QueueBindAsync(
    queue: queueName,
    exchange: exchangeName,
    routingKey: routingKey);
Console.WriteLine($"✅ Fila principal conectada ao exchange com routing key: {routingKey}");

Console.WriteLine("===========================================");
Console.WriteLine();


Console.WriteLine("Quantos pedidos você quer enviar?");
if (!int.TryParse(Console.ReadLine(), out var quantidadePedidos))
{
    quantidadePedidos = 3;
}

Console.WriteLine();
Console.WriteLine("===========================================");
Console.WriteLine("📦 ENVIANDO PEDIDOS...");
Console.WriteLine("===========================================");

for (int i = 1; i <= quantidadePedidos; i++)
{
    var pedido = CriarPedidoErroFake(i);
    var body = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(pedido);

    var properties = new BasicProperties
    {
        Persistent = true,
        ContentType = "application/json",
        ContentEncoding = "utf-8",
        MessageId = pedido.Id.ToString(),
        Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
    };

    await channel.BasicPublishAsync(
        exchange: exchangeName,
        routingKey: routingKey,
        mandatory: false,
        basicProperties: properties,
        body: body);

    Console.WriteLine($"✉️  Pedido {i} enviado:");
    Console.WriteLine($"   ID: {pedido.Id}");
    Console.WriteLine($"   Cliente: {pedido.ClienteEmail}");
    Console.WriteLine($"   Valor: {pedido.ValorTotal:C}");
    Console.WriteLine();

    if (i < quantidadePedidos)
    {
        Console.WriteLine("Pressione ENTER para enviar o próximo pedido...");
        Console.ReadLine();
    }
}

Console.WriteLine("===========================================");
Console.WriteLine("✅ Todos os pedidos foram enviados!");
Console.WriteLine("===========================================");

static Pedido CriarPedidoFake(int index)
{
   
    var valor = Random.Shared.Next(-100, 5000); 
    return new Pedido
    {
        Id = Guid.NewGuid(),
        ClienteEmail = $"cliente{index}@email.com",
        ValorTotal = valor,
        DataCriacao = DateTime.UtcNow,
        Itens = new List<Item>
        {
            new Item
            {
                NomeProduto = $"Produto {index}",
                Quantidade = Random.Shared.Next(1, 5),
                PrecoUnitario = Random.Shared.Next(20, 1000)
            }
        }
    };
}


static Pedido CriarPedidoErroFake(int index)
{

    var valor = -100;
    return new Pedido
    {
        Id = Guid.NewGuid(),
        ClienteEmail = $"cliente{index}@email.com",
        ValorTotal = valor,
        DataCriacao = DateTime.UtcNow,
        Itens = new List<Item>
        {
            new Item
            {
                NomeProduto = $"Produto {index}",
                Quantidade = Random.Shared.Next(1, 5),
                PrecoUnitario = Random.Shared.Next(20, 1000)
            }
        }
    };
}