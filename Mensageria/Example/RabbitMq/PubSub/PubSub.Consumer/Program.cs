using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;

const string brokerUri = "amqp://guest:guest@localhost:5672/%2f";

// Define o nome do exchange que distribuirá os logs.
const string exchangeName = "logs";

// Inicia a construção das configurações da conexão.
ConnectionSettings settings = ConnectionSettingsBuilder.Create()
                                                        .Uri(new Uri(brokerUri)) // Define o endereço do servidor RabbitMQ.
                                                        .ContainerId("tutorial-receivelogs") // Identifica esta aplicação consumidora no RabbitMQ.
                                                        .Build();// Finaliza e cria as configurações.

// Cria o ambiente que administra os recursos do cliente AMQP.
IEnvironment environment = AmqpEnvironment.Create(settings);

// Abre uma conexão com o RabbitMQ.
IConnection connection =
    await environment.CreateConnectionAsync();

try
{
    // Obtém a API usada para administrar exchanges, filas e bindings.
    IManagement management = connection.Management();

    // Configura o exchange "logs" como fanout.
    // Cada fila vinculada recebe uma cópia das mensagens.
    IExchangeSpecification exchangeSpec = management
        .Exchange(exchangeName)
        .Type("fanout");

    // Declara o exchange no RabbitMQ.
    await exchangeSpec.DeclareAsync();

    // Configura uma fila temporária com nome gerado pelo RabbitMQ.
    IQueueSpecification tempQueue = management
        .Queue()

        // Permite que somente esta conexão utilize a fila.
        .Exclusive(true)

        // Remove a fila automaticamente quando ela deixa de ser utilizada.
        .AutoDelete(true);

    // Declara a fila temporária e obtém suas informações.
    IQueueInfo queueInfo = await tempQueue.DeclareAsync();

    // Obtém o nome gerado automaticamente para a fila.
    string queueName = queueInfo.Name();

    // Inicia a configuração do vínculo entre o exchange e a fila.
    IBindingSpecification binding = management.Binding()

        // Define o exchange "logs" como origem das mensagens.
        .SourceExchange(exchangeSpec)

        // Define a fila temporária como destino.
        .DestinationQueue(queueName)

        // Define uma routing key vazia.
        // Exchanges fanout ignoram essa chave.
        .Key(string.Empty);

    // Cria efetivamente o vínculo no RabbitMQ.
    await binding.BindAsync();

    // Inicia a criação do consumidor.
    IConsumer consumer = await connection.ConsumerBuilder()

        // Define a fila temporária que será consumida.
        .Queue(queueName)

        // Define a função chamada para cada mensagem recebida.
        .MessageHandler((ctx, message) =>
        {
            // Converte o conteúdo da mensagem para texto.
            string body = message.BodyAsString();

            // Exibe a mensagem recebida.
            Console.WriteLine($" [x] Received '{body}'");

            // Confirma que a mensagem foi processada com sucesso.
            ctx.Accept();

            // Retorna uma tarefa concluída.
            return Task.CompletedTask;
        })

        // Cria o consumidor e inicia o recebimento das mensagens.
        .BuildAndStartAsync();

    try
    {
        // Informa que o consumidor está aguardando mensagens.
        Console.WriteLine(
            " [*] Waiting for messages. To exit press CTRL+C");

        // Cria um controlador de cancelamento.
        using var cts = new CancellationTokenSource();

        // Registra a ação executada quando Ctrl+C é pressionado.
        Console.CancelKeyPress += (_, e) =>
        {
            // Impede que o programa termine imediatamente.
            e.Cancel = true;

            // Solicita o cancelamento da espera.
            cts.Cancel();
        };

        // Mantém o consumidor ativo indefinidamente.
        await Task.Delay(Timeout.Infinite, cts.Token);
    }
    catch (OperationCanceledException)
    {
        // A exceção é esperada quando Ctrl+C é pressionado.
    }
    finally
    {
        // Fecha o consumidor e libera seus recursos.
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