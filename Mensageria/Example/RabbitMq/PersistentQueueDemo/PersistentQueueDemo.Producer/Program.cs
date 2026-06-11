
using RabbitMQ.Client;

// Cria e configura a fábrica responsável por abrir conexões com o RabbitMQ.
var factory = new ConnectionFactory
{
    HostName = "localhost", // Define o endereço do servidor RabbitMQ.
    Port = 5672,  // Define a porta padrão do protocolo AMQP 0-9-1.
    UserName = "guest",  // Define o usuário utilizado na autenticação
    Password = "guest", // Define a senha utilizada na autenticação.
    VirtualHost = "/",   // Define o virtual host utilizado pela conexão.
    AutomaticRecoveryEnabled = true, // Habilita a recuperação automática após uma falha de conexão.
    NetworkRecoveryInterval = TimeSpan.FromSeconds(10) // Define o intervalo entre as tentativas de recuperação da conexão.
};

await using var connection = await factory.CreateConnectionAsync(); // Abre uma conexão assíncrona com o servidor RabbitMQ."await using" fecha e libera a conexão ao final da aplicação.

await using var channel = await connection.CreateChannelAsync(); // Cria um canal AMQP dentro da conexão."await using" fecha e libera o canal ao final da aplicação.

// Declara ou verifica a existência da fila no RabbitMQ.
await channel.QueueDeclareAsync(
    queue: "persistent_queue", // Define o nome da fila.
    durable: true, // Mantém a definição da fila após a reinicialização do RabbitMQ.
    exclusive: false,  // Permite que outras conexões utilizem a mesma fila.
    autoDelete: false,  // Impede que a fila seja removida automaticamente.
    arguments: null // Não define argumentos opcionais adicionais para a fila.
    );

// Define o conteúdo textual da mensagem.
var message = "Hello, Persistent Queue!";

// Converte o texto da mensagem em bytes utilizando UTF-8.
var body = System.Text.Encoding.UTF8.GetBytes(message);

// Cria as propriedades que acompanharão a mensagem.
var properties = new BasicProperties
{
    Persistent = true // Solicita que a mensagem seja armazenada de forma persistente. (Grava em Disco)
};

// Publica a mensagem no RabbitMQ.
await channel.BasicPublishAsync(
    exchange: "", // Usa o exchange padrão do RabbitMQ.
    routingKey: "persistent_queue",  // No exchange padrão, a routing key corresponde ao nome da fila.
    mandatory: false,  // Não exige o retorno da mensagem caso ela não possa ser roteada.
    basicProperties: properties, // Envia as propriedades, incluindo a configuração de persistência.
    body: body // Envia o conteúdo da mensagem convertido em bytes.
    );

Console.WriteLine(" [x] Mensagem enviada Com Sucesso: {0}", message); // Exibe uma confirmação no console após a publicação.