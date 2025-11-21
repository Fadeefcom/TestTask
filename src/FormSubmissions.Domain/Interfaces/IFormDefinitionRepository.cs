using FormSubmissions.Domain.Aggregates.FormAggregate;

namespace FormSubmissions.Domain.Interfaces;

public interface IFormDefinitionRepository
{
    Task AddAsync(FormDefinition form, CancellationToken ct);
    Task<FormDefinition?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<FormDefinition>> GetAllAsync(CancellationToken ct);
}
