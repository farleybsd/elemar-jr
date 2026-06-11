using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

const string exchangeName = "pedido.exchange";
const string queueName = "pedido.criados";
const string routingKey = "pedido.criado";
const string dlxExchangeName = "pedido.dlx.exchange";
const string dlqQueueName = "pedido.dlq";
const string dlxRoutingKey = "pedido.falha";

var factory = new ConnectionFactory
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
Console.WriteLine("🔧 CONFIGURANDO CONSUMER...");
Console.WriteLine("===========================================");

// GARANTIR QUE TUDO ESTÁ CRIADO (caso rode antes do Producer)

// Exchange Principal
await channel.ExchangeDeclareAsync(
    exchange: exchangeName,
    type: ExchangeType.Direct,
    durable: true,
    autoDelete: false);

// DLX
await channel.ExchangeDeclareAsync(
    exchange: dlxExchangeName,
    type: ExchangeType.Direct,
    durable: true,
    autoDelete: false);

// DLQ
await channel.QueueDeclareAsync(
    queue: dlqQueueName,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

// Bind DLQ à DLX
await channel.QueueBindAsync(
    queue: dlqQueueName,
    exchange: dlxExchangeName,
    routingKey: dlxRoutingKey);

// Fila Principal com DLX configurada
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
    arguments: mainQueueArgs);

// Bind fila principal
await channel.QueueBindAsync(
    queue: queueName,
    exchange: exchangeName,
    routingKey: routingKey);

// QoS - processar 1 mensagem por vez
await channel.BasicQosAsync(
    prefetchSize: 0,
    prefetchCount: 1,
    global: false);

Console.WriteLine("✅ Configuração completa!");
Console.WriteLine("===========================================");
Console.WriteLine();
Console.WriteLine("👂 Aguardando mensagens...");
Console.WriteLine("===========================================");
Console.WriteLine();

// CONSUMER
var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (_, ea) =>
{
    var body = ea.Body.ToArray();
    var json = Encoding.UTF8.GetString(body);

    try
    {
        var pedido = JsonSerializer.Deserialize<Pedido>(json);

        Console.WriteLine("┌─────────────────────────────────────────┐");
        Console.WriteLine("│ 📨 NOVA MENSAGEM RECEBIDA               │");
        Console.WriteLine("└─────────────────────────────────────────┘");
        Console.WriteLine($"  ID:      {pedido?.Id}");
        Console.WriteLine($"  Cliente: {pedido?.ClienteEmail}");
        Console.WriteLine($"  Valor:   {pedido?.ValorTotal:C}");
        Console.WriteLine($"  Data:    {pedido?.DataCriacao:dd/MM/yyyy HH:mm:ss}");
        Console.WriteLine();

        // REGRA DE NEGÓCIO: validar pedido
        if (pedido == null)
        {
            throw new Exception("Pedido veio nulo!");
        }

        // VALIDAÇÃO 1: Valor não pode ser negativo
        if (pedido.ValorTotal < 0)
        {
            Console.WriteLine("❌ ERRO: Valor total negativo!");
            Console.WriteLine($"   Valor inválido: {pedido.ValorTotal:C}");
            Console.WriteLine("   ⚠️  Rejeitando mensagem → vai para DLQ");
            Console.WriteLine();

            // ERRO DEFINITIVO - NÃO VAI CONSEGUIR PROCESSAR NUNCA
            // Manda para DLQ (requeue: false)
            await channel.BasicNackAsync(
                deliveryTag: ea.DeliveryTag,
                multiple: false,
                requeue: false); // ← AQUI! Vai para DLX/DLQ

            return;
        }

        // VALIDAÇÃO 2: Email deve estar preenchido
        if (string.IsNullOrWhiteSpace(pedido.ClienteEmail))
        {
            Console.WriteLine("❌ ERRO: Email do cliente vazio!");
            Console.WriteLine("   ⚠️  Rejeitando mensagem → vai para DLQ");
            Console.WriteLine();

            await channel.BasicNackAsync(
                deliveryTag: ea.DeliveryTag,
                multiple: false,
                requeue: false);

            return;
        }

        // SIMULAÇÃO: Erro aleatório temporário (ex: banco fora do ar)
        if (Random.Shared.Next(0, 10) == 5)
        {
            throw new Exception("Banco de dados temporariamente indisponível!");
        }

        // ✅ TUDO OK! Processar pedido
        Console.WriteLine("⚙️  Processando pedido...");
        await Task.Delay(1000); // Simula processamento

        Console.WriteLine("✅ PEDIDO PROCESSADO COM SUCESSO!");
        Console.WriteLine();

        // CONFIRMAR que processou
        await channel.BasicAckAsync(
            deliveryTag: ea.DeliveryTag,
            multiple: false);
    }
    catch (JsonException ex)
    {
        // Erro de JSON = mensagem corrompida = erro definitivo
        Console.WriteLine("┌─────────────────────────────────────────┐");
        Console.WriteLine("│ ❌ ERRO DE DESERIALIZAÇÃO               │");
        Console.WriteLine("└─────────────────────────────────────────┘");
        Console.WriteLine($"  Erro: {ex.Message}");
        Console.WriteLine($"  JSON: {json}");
        Console.WriteLine("  ⚠️  Mensagem corrompida → vai para DLQ");
        Console.WriteLine();

        // ERRO DEFINITIVO - mensagem está quebrada
        await channel.BasicNackAsync(
            ea.DeliveryTag,
            multiple: false,
            requeue: false); // Vai para DLQ
    }
    catch (Exception ex)
    {
        // Erro temporário (ex: banco fora, API lenta)
        Console.WriteLine("┌─────────────────────────────────────────┐");
        Console.WriteLine("│ ⚠️  ERRO TEMPORÁRIO                     │");
        Console.WriteLine("└─────────────────────────────────────────┘");
        Console.WriteLine($"  Erro: {ex.Message}");
        Console.WriteLine("  🔄 Recolocando na fila para retry...");
        Console.WriteLine();

        // ERRO TEMPORÁRIO - tenta de novo
        await channel.BasicNackAsync(
            ea.DeliveryTag,
            multiple: false,
            requeue: true); // ← Volta para a fila, tenta de novo
    }
};

await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: false, // IMPORTANTE! Confirmação manual
    consumer: consumer);

Console.WriteLine("===========================================");
Console.WriteLine("✅ Consumer rodando!");
Console.WriteLine("===========================================");
Console.WriteLine();
Console.WriteLine("Pressione ENTER para parar o consumer...");
Console.ReadLine();

// Classe local (ou você pode usar a do Model)
public class Pedido
{
    public Guid Id { get; set; }
    public string ClienteEmail { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public DateTime DataCriacao { get; set; }
}