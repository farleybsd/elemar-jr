
using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;
using System.Text;


const string brokerUri = "amqp://guest:guest@localhost:5672/%2f";

// Inicia a construção das configurações da conexão.
ConnectionSettings settings = ConnectionSettingsBuilder.Create() 
                                                       .Uri(new Uri(brokerUri))  // Define o endereço do servidor RabbitMQ.
                                                       .ContainerId("tutorial-send") // Define um identificador para esta aplicação/conexão AMQP.
                                                       .Build();  // Finaliza e cria o objeto de configurações.

// Cria o ambiente AMQP usando as configurações definidas.
IEnvironment environment = AmqpEnvironment.Create(settings);

// Abre de forma assíncrona uma conexão com o RabbitMQ.
IConnection connection = await environment.CreateConnectionAsync();




try
{
    // Obtém a API de gerenciamento associada à conexão.
    // Ela permite declarar filas, exchanges e bindings.
    IManagement management = connection.Management();

    // Cria a especificação da fila chamada "hello".
    // Define a fila como QUORUM, ou seja, replicada e tolerante a falhas.
    IQueueSpecification queueSpec = management
                                              .Queue("hello")
                                              .Type(QueueType.QUORUM);

    // Declara efetivamente a fila no RabbitMQ.
    // Se uma fila compatível já existir, ela será reutilizada.
    await queueSpec.DeclareAsync();
    // Cria um publicador que enviará mensagens diretamente para a fila "hello".
    IPublisher publisher = await connection
                                           .PublisherBuilder()
                                           .Queue("hello")
                                           .BuildAsync();
    try {

        // Define o conteúdo textual que será enviado na mensagem.
       // const string body = "Hello World!";

        List<string> messages = new List<string> { "Farley 1", "Farley 2", "Farley 3" };

        foreach(string body in messages)
        {
            // Converte o texto para bytes em UTF-8 e cria a mensagem AMQP.
            var message = new AmqpMessage(Encoding.UTF8.GetBytes(body));

            // Publica a mensagem de forma assíncrona e obtém o resultado da publicação.
            PublishResult pr = await publisher.PublishAsync(message);

            // Verifica se o RabbitMQ não aceitou a mensagem publicada.
            if (pr.Outcome.State != OutcomeState.Accepted)
            {
                // Exibe no fluxo de erro o estado inesperado retornado pelo RabbitMQ.
                Console.Error.WriteLine($"Unexpected publish outcome: {pr.Outcome.State}");
                // Encerra a aplicação imediatamente com o código de erro 1.
                Environment.Exit(1);
            }

            // Informa no console que a mensagem foi enviada com sucesso.
            Console.WriteLine($" [x] Sent-Successfully {body}");
        }

        
    }
    finally {
        // Fecha o publisher de forma assíncrona e libera seus recursos.
        await publisher.CloseAsync();
    }
}
finally {
    // Fecha a conexão ativa com o RabbitMQ e libera os recursos associados.
    await connection.CloseAsync();
    // Encerra o ambiente AMQP e libera todos os recursos gerenciados por ele.
    await environment.CloseAsync();
}