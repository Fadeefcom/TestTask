using FormSubmissions.BuildingBlocks.Domain;

namespace FormSubmissions.BuildingBlocks.Infrastructure;

public sealed class UnitOfWork : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
