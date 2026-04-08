using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GymErp.Common;

static class ServiceBusExtension
{
    public static async Task DispatchDomainEventsAsync(this IServiceBus serviceBus, DbContext ctx)
    {
        var logger = Log.ForContext("SourceContext", nameof(ServiceBusExtension));

        // EF Core não trata `Aggregate` (classe base) como entity type mapeada,
        // então `Entries<Aggregate>()` pode retornar vazio.
        // Por isso, percorremos todas as entidades rastreadas e filtramos as que herdam `Aggregate`.
        var aggregateEntities = ctx.ChangeTracker
            .Entries()
            .Select(x => x.Entity)
            .OfType<Aggregate>()
            .Where(a => a.DomainEvents.Count != 0)
            .ToList();

        var domainEvents = aggregateEntities
            .SelectMany(a => a.DomainEvents)
            .ToList();
        aggregateEntities.ForEach(entity => entity.ClearDomainEvents());
        foreach (var domainEvent in domainEvents)
        {
            var eventType = domainEvent.GetType().Name;

            logger.Debug("Dispatching domain event {EventType}", eventType);
            try
            {
                await serviceBus.PublishAsync(domainEvent);

                logger.Debug("Domain event {EventType} published", eventType);
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "Failed to publish domain event {EventType}", eventType);
                throw;
            }
        }
        
    }
}