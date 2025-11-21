using FormSubmissions.Domain.Aggregates.SubmissionAggregate;
using FormSubmissions.Domain.Specifications;

namespace FormSubmissions.Domain.Interfaces;

public interface ISubmissionRepository
{
    Task AddAsync(Submission submission, CancellationToken ct);
    Task<Submission?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Submission>> SearchAsync(SubmissionSearchSpecification specification, CancellationToken ct);
}
