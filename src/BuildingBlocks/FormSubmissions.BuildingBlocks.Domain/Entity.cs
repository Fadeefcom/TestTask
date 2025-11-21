namespace FormSubmissions.BuildingBlocks.Domain;

public abstract class Entity<TId> where TId : struct
{
    public TId Id { get; protected set; }

    protected Entity(TId id)
    {
        Id = id;
    }
}
