using System.Text.Json;
using FormSubmissions.Domain.Aggregates.SubmissionAggregate;
using FormSubmissions.Domain.ValueObjects;
using Xunit;

namespace FormSubmissions.Domain.Tests;

public sealed class SubmissionTests
{
    private static Dictionary<string, JsonElement> Payload(object o)
    {
        var json = JsonSerializer.Serialize(o);
        return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
               ?? new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_WithValidFormId_CreatesSubmission()
    {
        var formId = FormId.New();
        var payload = Payload(new { name = "Alice" });

        var sub = Submission.Create(formId, payload);

        Assert.NotEqual(Guid.Empty, sub.Id.Value);
        Assert.Equal(formId.Value, sub.FormId.Value);
        Assert.True(sub.CreatedAt <= DateTimeOffset.UtcNow);
        Assert.True(sub.Payload.ContainsKey("name"));
        Assert.Empty(sub.Attachments);
    }

    [Fact]
    public void Create_WithEmptyFormId_Throws()
    {
        var formId = new FormId(Guid.Empty);
        var payload = Payload(new { name = "Alice" });

        Assert.Throws<ArgumentException>(() => Submission.Create(formId, payload));
    }

    [Fact]
    public void AddAttachment_ReturnsNewSubmissionWithAttachment()
    {
        var formId = FormId.New();
        var sub = Submission.Create(formId, Payload(new { name = "Alice" }));

        var att = new AttachmentMetadata("a.txt", "text/plain", 10, "key");

        var updated = sub.AddAttachment(att);

        Assert.Single(updated.Attachments);
        Assert.Equal("a.txt", updated.Attachments[0].FileName);
        Assert.Empty(sub.Attachments);
    }
}
