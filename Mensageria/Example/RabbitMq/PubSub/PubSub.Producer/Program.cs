
using System.Text;
using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;

// Define usuário, senha, servidor, porta e virtual host do RabbitMQ.
const string brokerUri = "amqp://guest:guest@localhost:5672/%2f";

// Define o nome do exchange que receberá as mensagens.
const string exchangeName = "logs";

// Usa os argumentos do terminal ou uma mensagem padrão.
string message = args.Length < 1
    ? "info: Hello World!"
    : string.Join(" ", args);

// Inicia a criação das configurações da conexão.
ConnectionSettings settings = ConnectionSettingsBuilder.Create()
                                .Uri(new Uri(brokerUri)) // Define o endereço de conexão com o RabbitMQ.
                                .ContainerId("tutorial-emitlog") // Define o identificador desta aplicação produtora.
                                .Build();// Finaliza e cria as configurações.

// Cria o ambiente responsável pelos recursos do cliente AMQP.
IEnvironment environment = AmqpEnvironment.Create(settings);

// Abre uma conexão com o RabbitMQ.
IConnection connection =
    await environment.CreateConnectionAsync();

// Inicia o bloco protegido que utiliza a conexão.
try
{
    // Obtém a API para gerenciar exchanges, filas e bindings.
    IManagement management = connection.Management();

    // Configura um exchange chamado "logs" do tipo fanout.
    IExchangeSpecification exchangeSpec = management
        .Exchange(exchangeName)
        .Type("fanout"); //ele envia uma cópia da mensagem para todas as filas vinculadas a ele, ignorando chaves de roteamento.

    // Declara o exchange no RabbitMQ.
    await exchangeSpec.DeclareAsync();

    // Cria um produtor que publica no exchange "logs".
    IPublisher publisher = await connection
        .PublisherBuilder()
        .Exchange(exchangeName)
        .BuildAsync();

    // Inicia o bloco protegido que utiliza o produtor.
    try
    {
        // Converte o texto para bytes e cria uma mensagem AMQP.
        var amqpMessage = new AmqpMessage(
            Encoding.UTF8.GetBytes(message));

        // Publica a mensagem e aguarda o resultado.
        PublishResult pr =
            await publisher.PublishAsync(amqpMessage);

        // Avalia o resultado retornado pela publicação.
        switch (pr.Outcome.State)
        {
            // Indica que a mensagem foi aceita pelo RabbitMQ.
            case OutcomeState.Accepted:

                // Encerra este caso sem executar outras ações.
                break;

            // Indica que a mensagem foi liberada sem ser aceita.
            case OutcomeState.Released:

                // Exibe a mensagem que foi liberada.
                Console.Error.WriteLine(
                    $"Released message: {pr.Message.BodyAsString()}");

                // Encerra a aplicação com código de erro.
                Environment.Exit(1);

                // Encerra este caso.
                break;

            // Indica que a mensagem foi rejeitada.
            case OutcomeState.Rejected:

                // Exibe a mensagem rejeitada e o motivo do erro.
                Console.Error.WriteLine(
                    $"[Publisher] Message: {pr.Message.BodyAsString()} " +
                    $"rejected with error: {pr.Outcome.Error}");

                // Encerra a aplicação com código de erro.
                Environment.Exit(1);

                // Encerra este caso.
                break;

            // Trata qualquer resultado diferente dos esperados.
            default:

                // Exibe o estado inesperado da publicação.
                Console.Error.WriteLine(
                    $"Unexpected publish outcome: {pr.Outcome.State}");

                // Encerra a aplicação com código de erro.
                Environment.Exit(1);

                // Encerra o caso padrão.
                break;
        }

        // Informa que a mensagem foi enviada.
        Console.WriteLine($" [x] Sent '{message}'");
    }

    // Este bloco sempre será executado.
    finally
    {
        // Fecha o produtor e libera seus recursos.
        await publisher.CloseAsync();
    }
}

// Este bloco sempre será executado.
finally
{
    // Fecha a conexão com o RabbitMQ.
    await connection.CloseAsync();

    // Encerra o ambiente AMQP e libera seus recursos.
    await environment.CloseAsync();
}