using System.Text.Json;
using FormSubmissions.Domain.Aggregates.FormAggregate;
using FormSubmissions.Domain.Aggregates.SubmissionAggregate;
using FormSubmissions.Domain.Specifications;
using FormSubmissions.Domain.ValueObjects;
using FormSubmissions.Infrastructure.Persistence.InMemoryStore;
using FormSubmissions.Infrastructure.Persistence.Repositories;
using Xunit;

namespace FormSubmissions.Infrastructure.Tests;

public sealed class SubmissionRepositoryTests
{
    private static Dictionary<string, JsonElement> Payload(object o)
    {
        var json = JsonSerializer.Serialize(o);
        return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
               ?? new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_ReturnsSameSubmission()
    {
        var store = new InMemorySubmissionStore();
        var repo = new SubmissionRepository(store);

        var formId = FormId.New();
        var sub = Submission.Create(formId, Payload(new { name = "Alice" }));

        await repo.AddAsync(sub, CancellationToken.None);

        var loaded = await repo.GetByIdAsync(sub.Id.Value, CancellationToken.None);

        Assert.NotNull(loaded);
        Assert.Equal(sub.Id.Value, loaded!.Id.Value);
        Assert.Equal(formId.Value, loaded.FormId.Value);
        Assert.True(loaded.Payload.ContainsKey("name"));
    }

    [Fact]
    public async Task SearchAsync_ByFormId_FiltersCorrectly()
    {
        var store = new InMemorySubmissionStore();
        var repo = new SubmissionRepository(store);

        var formA = FormId.New();
        var formB = FormId.New();

        var s1 = Submission.Create(formA, Payload(new { name = "One" }));
        var s2 = Submission.Create(formA, Payload(new { name = "Two" }));
        var s3 = Submission.Create(formB, Payload(new { name = "Three" }));

        await repo.AddAsync(s1, CancellationToken.None);
        await repo.AddAsync(s2, CancellationToken.None);
        await repo.AddAsync(s3, CancellationToken.None);

        var spec = new SubmissionSearchSpecification(formA.Value, null, null, null);
        var found = await repo.SearchAsync(spec, CancellationToken.None);

        Assert.Equal(2, found.Count);
        Assert.All(found, x => Assert.Equal(formA.Value, x.FormId.Value));
    }

    [Fact]
    public async Task SearchAsync_ByQuery_MatchesPayloadStrings()
    {
        var store = new InMemorySubmissionStore();
        var repo = new SubmissionRepository(store);

        var formId = FormId.New();
        var unique = "needle-" + Guid.NewGuid().ToString("N");

        var s1 = Submission.Create(formId, Payload(new { name = "abc", note = unique }));
        var s2 = Submission.Create(formId, Payload(new { name = "def", note = "zzz" }));

        await repo.AddAsync(s1, CancellationToken.None);
        await repo.AddAsync(s2, CancellationToken.None);

        var spec = new SubmissionSearchSpecification(formId.Value, "needle", null, null);
        var found = await repo.SearchAsync(spec, CancellationToken.None);

        Assert.Single(found);
        Assert.Equal(s1.Id.Value, found[0].Id.Value);
    }

    [Fact]
    public async Task SearchAsync_WithFutureFrom_ReturnsEmpty()
    {
        var store = new InMemorySubmissionStore();
        var repo = new SubmissionRepository(store);

        var formId = FormId.New();
        var s1 = Submission.Create(formId, Payload(new { name = "One" }));

        await repo.AddAsync(s1, CancellationToken.None);

        var futureFrom = DateTimeOffset.UtcNow.AddDays(1);
        var spec = new SubmissionSearchSpecification(formId.Value, null, futureFrom, null);
        var found = await repo.SearchAsync(spec, CancellationToken.None);

        Assert.Empty(found);
    }

    [Fact]
    public async Task SearchAsync_WithPastTo_ExcludesNewOnes()
    {
        var store = new InMemorySubmissionStore();
        var repo = new SubmissionRepository(store);

        var formId = FormId.New();
        var s1 = Submission.Create(formId, Payload(new { name = "One" }));

        await repo.AddAsync(s1, CancellationToken.None);

        var pastTo = DateTimeOffset.UtcNow.AddDays(-1);
        var spec = new SubmissionSearchSpecification(formId.Value, null, null, pastTo);
        var found = await repo.SearchAsync(spec, CancellationToken.None);

        Assert.Empty(found);
    }

    [Fact]
    public async Task SearchAsync_ByFormIdAndQuery_CombinesFilters()
    {
        var store = new InMemorySubmissionStore();
        var repo = new SubmissionRepository(store);

        var formA = FormId.New();
        var formB = FormId.New();
        var unique = "combo-" + Guid.NewGuid().ToString("N");

        var s1 = Submission.Create(formA, Payload(new { name = unique }));
        var s2 = Submission.Create(formB, Payload(new { name = unique }));
        var s3 = Submission.Create(formA, Payload(new { name = "other" }));

        await repo.AddAsync(s1, CancellationToken.None);
        await repo.AddAsync(s2, CancellationToken.None);
        await repo.AddAsync(s3, CancellationToken.None);

        var spec = new SubmissionSearchSpecification(formA.Value, "combo", null, null);
        var found = await repo.SearchAsync(spec, CancellationToken.None);

        Assert.Single(found);
        Assert.Equal(s1.Id.Value, found[0].Id.Value);
    }
}
