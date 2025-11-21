namespace FormSubmissions.Domain.ValueObjects;

public readonly record struct SubmissionId(Guid Value)
{
    public static SubmissionId New() => new(Guid.NewGuid());
}
