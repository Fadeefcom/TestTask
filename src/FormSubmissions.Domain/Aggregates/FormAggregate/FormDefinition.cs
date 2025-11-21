using FormSubmissions.BuildingBlocks.Domain;
using FormSubmissions.Domain.ValueObjects;

namespace FormSubmissions.Domain.Aggregates.FormAggregate;

public sealed class FormDefinition : AggregateRoot<FormId>
{
    private readonly List<FormFieldDefinition> _fields;

    public string Name { get; private set; }
    public FormVersion Version { get; private set; }
    public IReadOnlyList<FormFieldDefinition> Fields => _fields;

    private FormDefinition(FormId id, string name, FormVersion version, List<FormFieldDefinition> fields)
        : base(id)
    {
        Name = name;
        Version = version;
        _fields = fields;
    }

    public static FormDefinition Create(string name, List<FormFieldDefinition> fields)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        if (fields == null || fields.Count == 0)
            throw new ArgumentException("Fields are required");

        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in fields)
        {
            if (string.IsNullOrWhiteSpace(f.Key))
                throw new ArgumentException("Field key is required");

            if (!keys.Add(f.Key))
                throw new ArgumentException($"Duplicate field key: {f.Key}");
        }

        return new FormDefinition(FormId.New(), name.Trim(), FormVersion.Initial(), fields);
    }

    public void BumpVersion()
    {
        Version = Version.Next();
    }
}
