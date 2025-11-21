namespace FormSubmissions.Domain.Aggregates.FormAggregate;

public readonly record struct FormVersion(int Value)
{
    public static FormVersion Initial() => new(1);
    public FormVersion Next() => new(Value + 1);
}
