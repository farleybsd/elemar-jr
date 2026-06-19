namespace OutBox.Infrastructure;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public required string Uri { get; init; }
    public required string QueueName { get; init; }
    public string ContainerId { get; init; } = "producer-order";
}
