namespace GymErp.Common;

public interface IDomainEvent
{ }
public abstract class Aggregate
{
    private List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    internal void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents ??= [];
        _domainEvents.Add(domainEvent);
    }

    internal void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents?.Remove(domainEvent);
    }

    internal void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }   
}
