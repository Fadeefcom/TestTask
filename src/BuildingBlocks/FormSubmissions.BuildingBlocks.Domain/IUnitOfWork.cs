namespace FormSubmissions.BuildingBlocks.Domain;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct);
}
