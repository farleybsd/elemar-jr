using RabbitMq.Dlx.Demo.Model;
using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

const string brokerUri = "amqp://guest:guest@localhost:5672/%2f"; // Define o endereço, usuário, senha, porta e virtual host.
const string queueName = "pedido.criados"; // Define o nome da fila consumida.

ConnectionSettings settings = ConnectionSettingsBuilder.Create() // Inicia a configuração da conexão.
    .Uri(new Uri(brokerUri)) // Informa o endereço do RabbitMQ.
    .ContainerId("pedido-receive") // Identifica este consumidor no RabbitMQ.
    .Build(); // Finaliza a configuração.

IEnvironment environment = AmqpEnvironment.Create(settings); // Cria o ambiente do cliente AMQP.
IConnection connection = await environment.CreateConnectionAsync(); // Abre a conexão com o RabbitMQ.

try
{
    IManagement management = connection.Management();  // Obtém o gerenciador de filas e exchanges.

    IQueueSpecification queueSpec = management // Inicia a configuração da fila principal.
        .Queue(queueName) // Define a fila que será declarada.
        .Type(QueueType.QUORUM);   // Define a fila como quorum.

    await queueSpec.DeclareAsync();   // Declara a fila caso ela ainda não exista.

    IConsumer consumer = await connection.ConsumerBuilder()  // Inicia a criação do consumidor.
        .Queue(queueName) // Define a fila que será consumida.
        .MessageHandler((ctx, message) =>  // Define a função executada para cada mensagem.
        {
            try
            {
                byte[] body = message.Body().ToArray();  // Obtém o corpo da mensagem como bytes.
                string json = Encoding.UTF8.GetString(body);  // Converte os bytes em texto JSON.

                Pedido? pedido = JsonSerializer.Deserialize<Pedido>(json);  // Converte o JSON em um objeto Pedido.

                if (pedido is null)  // Verifica se o pedido não foi criado.
                {
                    Console.WriteLine("Pedido nulo. Enviando para a DLQ.");  // Informa que o pedido recebido é nulo.
                    ctx.Discard(); // Rejeita a mensagem e permite o envio à DLQ.
                    return Task.CompletedTask;  // Encerra o processamento desta mensagem.
                }

                if (pedido.ValorTotal < 0) // Verifica se o valor do pedido é negativo.
                {
                    Console.WriteLine(
                        $"Valor total negativo: {pedido.ValorTotal:C}"); // Exibe o valor inválido.

                    ctx.Discard();  // Rejeita a mensagem e permite o envio à DLQ.
                    return Task.CompletedTask; // Encerra o processamento desta mensagem.
                }

                if (string.IsNullOrWhiteSpace(pedido.ClienteEmail)) // Verifica se o e-mail está vazio.
                {
                    Console.WriteLine("E-mail do cliente não informado.");  // Informa que o e-mail não foi preenchido.

                    ctx.Discard();  // Rejeita a mensagem e permite o envio à DLQ.
                    return Task.CompletedTask;  // Encerra o processamento desta mensagem.
                }

                // Informa o início do processamento válido.
                Console.WriteLine($"Processando pedido: {json}");

                ctx.Accept();  // Confirma o processamento e remove a mensagem da fila.

                Console.WriteLine("Pedido processado com sucesso.");  // Informa que o pedido foi processado.
            }
            catch (JsonException exception) // Captura erros de JSON inválido.
            {
                Console.WriteLine($"JSON inválido: {exception.Message}"); // Exibe o erro de desserialização.

                
                ctx.Discard(); // Rejeita o JSON inválido e permite o envio à DLQ.
            }
            catch (Exception exception)  // Captura outros erros de processamento.
            {
                Console.WriteLine($"Erro temporário: {exception.Message}"); // Exibe o erro considerado temporário.

                
                ctx.Requeue(); // Devolve a mensagem para uma nova tentativa.
            }

            return Task.CompletedTask;  // Finaliza a execução assíncrona do handler.
        })
        .BuildAndStartAsync(); // Cria e inicia o consumidor.

    try  // Inicia o controle do tempo de vida do consumidor.
    {
        // Cria um mecanismo para detectar quando Ctrl+C for pressionado.
        using var cancellation = new CancellationTokenSource();  // Cria um sinal de cancelamento.

        Console.CancelKeyPress += (_, eventArgs) => // Registra o evento disparado por Ctrl+C.
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