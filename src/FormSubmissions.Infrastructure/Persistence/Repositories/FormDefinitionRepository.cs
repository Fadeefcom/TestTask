using FormSubmissions.Domain.Aggregates.FormAggregate;
using FormSubmissions.Domain.Interfaces;
using FormSubmissions.Infrastructure.Persistence.InMemoryStore;

namespace FormSubmissions.Infrastructure.Persistence.Repositories;

public sealed class FormDefinitionRepository : IFormDefinitionRepository
{
    private readonly InMemoryFormStore _store;

    public FormDefinitionRepository(InMemoryFormStore store)
    {
        _store = store;
    }

    public Task AddAsync(FormDefinition form, CancellationToken ct)
        => _store.AddAsync(form, ct);

    public Task<FormDefinition?> GetByIdAsync(Guid id, CancellationToken ct)
        => _store.GetByIdAsync(id, ct);

    public Task<IReadOnlyList<FormDefinition>> GetAllAsync(CancellationToken ct)
        => _store.GetAllAsync(ct);
}
