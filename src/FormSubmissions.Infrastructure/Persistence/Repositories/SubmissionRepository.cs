using FormSubmissions.Domain.Aggregates.SubmissionAggregate;
using FormSubmissions.Domain.Interfaces;
using FormSubmissions.Domain.Specifications;
using FormSubmissions.Infrastructure.Persistence.InMemoryStore;

namespace FormSubmissions.Infrastructure.Persistence.Repositories;

public sealed class SubmissionRepository : ISubmissionRepository
{
    private readonly InMemorySubmissionStore _store;

    public SubmissionRepository(InMemorySubmissionStore store)
    {
        _store = store;
    }

    public Task AddAsync(Submission submission, CancellationToken ct)
        => _store.AddAsync(submission, ct);

    public Task<Submission?> GetByIdAsync(Guid id, CancellationToken ct)
        => _store.GetByIdAsync(id, ct);

    public Task<IReadOnlyList<Submission>> SearchAsync(SubmissionSearchSpecification specification, CancellationToken ct)
        => _store.SearchAsync(specification.FormId, specification.Query, specification.From, specification.To, ct);
}
