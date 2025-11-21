namespace FormSubmissions.BuildingBlocks.Domain;

public abstract class AggregateRoot<TId> : Entity<TId> where TId : struct
{
    private readonly List<DomainEvent> _domainEvents = new();

    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

    protected AggregateRoot(TId id) : base(id)
    {
    }

    protected void AddDomainEvent(DomainEvent ev)
    {
        _domainEvents.Add(ev);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
