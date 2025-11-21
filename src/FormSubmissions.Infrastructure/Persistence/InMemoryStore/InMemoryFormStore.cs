using System.Collections.Concurrent;
using FormSubmissions.Domain.Aggregates.FormAggregate;

namespace FormSubmissions.Infrastructure.Persistence.InMemoryStore;

public sealed class InMemoryFormStore
{
    private readonly ConcurrentDictionary<Guid, FormDefinition> _forms = new();

    public Task AddAsync(FormDefinition form, CancellationToken ct)
    {
        _forms[form.Id.Value] = form;
        return Task.CompletedTask;
    }

    public Task<FormDefinition?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        _forms.TryGetValue(id, out var form);
        return Task.FromResult(form);
    }

    public Task<IReadOnlyList<FormDefinition>> GetAllAsync(CancellationToken ct)
    {
        return Task.FromResult<IReadOnlyList<FormDefinition>>(_forms.Values.OrderBy(f => f.Name).ToList());
    }
}
