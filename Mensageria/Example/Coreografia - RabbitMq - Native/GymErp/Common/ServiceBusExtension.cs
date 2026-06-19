using Microsoft.EntityFrameworkCore;

namespace GymErp.Common;

static class ServiceBusExtension
{
    public static async Task DispatchDomainEventsAsync(this IServiceBus serviceBus, DbContext ctx)
    {
        var domainEntities = ctx.ChangeTracker // Acessa as entidades rastreadas pelo EF.
           .Entries<Aggregate>()  // Filtra entradas do tipo Aggregate.
           .Where(x => x.Entity.DomainEvents.Count != 0); // Mantém apenas entidades com eventos.

        
        var entityEntries = domainEntities.ToList(); // Materializa a consulta em uma lista.

        var domainEvents = entityEntries // Começa a coletar os eventos das entidades.
            .SelectMany(x => x.Entity.DomainEvents) // Junta todos os eventos em uma sequência.
            .ToList();  // Materializa os eventos em uma lista.

        entityEntries.ForEach(entity => entity.Entity.ClearDomainEvents()); // Limpa os eventos das entidades.

        foreach (var domainEvent in domainEvents) // Percorre cada evento coletado.
            await serviceBus.PublishAsync(domainEvent); // Publica o evento no service bus.
    }
}
