namespace FormSubmissions.Domain.Aggregates.SubmissionAggregate;

public sealed record AttachmentMetadata(
    string FileName,
    string ContentType,
    long SizeBytes,
    string StorageKey
);
