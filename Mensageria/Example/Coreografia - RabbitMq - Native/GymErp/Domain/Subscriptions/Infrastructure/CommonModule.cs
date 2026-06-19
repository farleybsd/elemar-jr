using Autofac;

namespace GymErp.Domain.Subscriptions.Infrastructure;

public class CommonModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // IServiceBus is registered by AddRabbitMq (RabbitMqServiceBus).
    }
}
