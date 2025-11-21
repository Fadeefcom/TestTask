using System.Collections.Concurrent;
using System.Text.Json;
using FormSubmissions.Domain.Aggregates.SubmissionAggregate;

namespace FormSubmissions.Infrastructure.Persistence.InMemoryStore;

public sealed class InMemorySubmissionStore
{
    private readonly ConcurrentDictionary<Guid, Submission> _submissions = new();

    public Task AddAsync(Submission submission, CancellationToken ct)
    {
        _submissions[submission.Id.Value] = submission;
        return Task.CompletedTask;
    }

    public Task<Submission?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        _submissions.TryGetValue(id, out var sub);
        return Task.FromResult(sub);
    }

    public Task<IReadOnlyList<Submission>> GetAllAsync(CancellationToken ct)
    {
        return Task.FromResult<IReadOnlyList<Submission>>(_submissions.Values.OrderByDescending(s => s.CreatedAt).ToList());
    }

    public Task<IReadOnlyList<Submission>> SearchAsync(
        Guid? formId,
        string? query,
        DateTimeOffset? from,
        DateTimeOffset? to,
        CancellationToken ct)
    {
        IEnumerable<Submission> q = _submissions.Values;

        if (formId.HasValue && formId.Value != Guid.Empty)
            q = q.Where(s => s.FormId.Value == formId.Value);

        if (from.HasValue)
            q = q.Where(s => s.CreatedAt >= from.Value);

        if (to.HasValue)
            q = q.Where(s => s.CreatedAt <= to.Value);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var needle = query.Trim();
            q = q.Where(s => PayloadContains(s.Payload, needle));
        }

        return Task.FromResult<IReadOnlyList<Submission>>(q.OrderByDescending(s => s.CreatedAt).ToList());
    }

    private static bool PayloadContains(IReadOnlyDictionary<string, JsonElement> payload, string needle)
    {
        foreach (var kv in payload)
        {
            var v = kv.Value;
            if (v.ValueKind == JsonValueKind.String && v.GetString() != null && v.GetString()!.Contains(needle, StringComparison.OrdinalIgnoreCase))
                return true;

            if (v.ValueKind == JsonValueKind.Number && v.GetRawText().Contains(needle, StringComparison.OrdinalIgnoreCase))
                return true;

            if (v.ValueKind == JsonValueKind.True || v.ValueKind == JsonValueKind.False)
            {
                if (v.GetRawText().Contains(needle, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        return false;
    }
}
