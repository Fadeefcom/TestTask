namespace FormSubmissions.Domain.Specifications;

public sealed class SubmissionSearchSpecification
{
    public Guid? FormId { get; }
    public string? Query { get; }
    public DateTimeOffset? From { get; }
    public DateTimeOffset? To { get; }

    public SubmissionSearchSpecification(Guid? formId, string? query, DateTimeOffset? from, DateTimeOffset? to)
    {
        FormId = formId;
        Query = string.IsNullOrWhiteSpace(query) ? null : query.Trim();
        From = from;
        To = to;
    }
}
