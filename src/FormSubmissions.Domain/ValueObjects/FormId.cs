namespace FormSubmissions.Domain.ValueObjects;

public readonly record struct FormId(Guid Value)
{
    public static FormId New() => new(Guid.NewGuid());
}
