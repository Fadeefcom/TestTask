using System.Text.Json;
using FormSubmissions.BuildingBlocks.Domain;
using FormSubmissions.Domain.ValueObjects;

namespace FormSubmissions.Domain.Aggregates.SubmissionAggregate;

public sealed class Submission : AggregateRoot<SubmissionId>
{
    public FormId FormId { get; }
    public DateTimeOffset CreatedAt { get; }
    public IReadOnlyDictionary<string, JsonElement> Payload { get; }
    public IReadOnlyList<AttachmentMetadata> Attachments { get; }

    private Submission(
        SubmissionId id,
        FormId formId,
        DateTimeOffset createdAt,
        Dictionary<string, JsonElement> payload,
        List<AttachmentMetadata> attachments)
        : base(id)
    {
        FormId = formId;
        CreatedAt = createdAt;
        Payload = payload;
        Attachments = attachments;
    }

    public static Submission Create(FormId formId, Dictionary<string, JsonElement> payload)
    {
        if (formId.Value == Guid.Empty)
            throw new ArgumentException("FormId is required");

        payload ??= new Dictionary<string, JsonElement>();

        return new Submission(
            SubmissionId.New(),
            formId,
            DateTimeOffset.UtcNow,
            new Dictionary<string, JsonElement>(payload, StringComparer.OrdinalIgnoreCase),
            new List<AttachmentMetadata>()
        );
    }

    public Submission AddAttachment(AttachmentMetadata attachment)
    {
        var list = Attachments.ToList();
        list.Add(attachment);
        return new Submission(Id, FormId, CreatedAt, Payload.ToDictionary(k => k.Key, v => v.Value, StringComparer.OrdinalIgnoreCase), list);
    }
}
