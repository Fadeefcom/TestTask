namespace FormSubmissions.BuildingBlocks.Domain;

public abstract record DomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
