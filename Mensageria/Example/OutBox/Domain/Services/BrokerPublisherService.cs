using Microsoft.Extensions.Options;
using OutBox.Infrastructure;
using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;
using System.Text;
using System.Text.Json;

namespace OutBox.Domain.Services;

public interface IBrokerPublisherService
{
    Task PublishAsync(
        object payload,
        CancellationToken cancellationToken = default);
}
public sealed class RabbitMqMessageProducer :
    IBrokerPublisherService,
    IHostedService

{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqMessageProducer> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private IEnvironment? _environment;
    private IConnection? _connection;
    private IPublisher? _publisher;

    public RabbitMqMessageProducer(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqMessageProducer> logger)
    {
        _options = options.Value;
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    // Executado uma vez quando a aplicação inicia.
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var settings = ConnectionSettingsBuilder
            .Create()
            .Uri(new Uri(_options.Uri))
            .ContainerId(_options.ContainerId)
            .Build();

        _environment = AmqpEnvironment.Create(settings);
        _connection = await _environment.CreateConnectionAsync();

        var management = _connection.Management();

        await management
            .Queue(_options.QueueName)
            .Type(QueueType.QUORUM)
            .DeclareAsync();

        _publisher = await _connection
            .PublisherBuilder()
            .Queue(_options.QueueName)
            .BuildAsync();

        _logger.LogInformation(
            "Produtor RabbitMQ iniciado para a fila {QueueName}",
            _options.QueueName);
    }

    public async Task PublishAsync(
        object payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);
        cancellationToken.ThrowIfCancellationRequested();

        if (_publisher is null)
        {
            throw new InvalidOperationException(
                "O produtor RabbitMQ ainda não foi inicializado.");
        }

        // Usa o tipo concreto do evento, mesmo que a variável seja object.
        var body = JsonSerializer.SerializeToUtf8Bytes(
            payload,
            payload.GetType(),
            _jsonOptions);

        var message = new AmqpMessage(body);

        var result = await _publisher.PublishAsync(message);

        if (result.Outcome.State != OutcomeState.Accepted)
        {
            throw new InvalidOperationException(
                $"RabbitMQ recusou a mensagem. Resultado: {result.Outcome.State}.");
        }

        _logger.LogInformation(
            "Evento {EventType} publicado na fila {QueueName}",
            payload.GetType().Name,
            _options.QueueName);
    }

    // Executado uma vez quando a aplicação é encerrada.
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_publisher is not null)
        {
            await _publisher.CloseAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
        }

        if (_environment is not null)
        {
            await _environment.CloseAsync();
        }

        _logger.LogInformation("Produtor RabbitMQ encerrado");
    }
}
