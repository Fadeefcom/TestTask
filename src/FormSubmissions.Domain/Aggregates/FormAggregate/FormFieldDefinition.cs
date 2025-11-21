namespace FormSubmissions.Domain.Aggregates.FormAggregate;

public enum FormFieldType
{
    Text,
    Dropdown,
    Date,
    Radio,
    Checkbox,
    Number
}

public sealed class FormFieldDefinition
{
    public string Key { get; }
    public string Label { get; }
    public FormFieldType Type { get; }
    public bool Required { get; }
    public IReadOnlyList<string>? Options { get; }
    public string? Pattern { get; }
    public double? Min { get; }
    public double? Max { get; }

    public FormFieldDefinition(
        string key,
        string label,
        FormFieldType type,
        bool required,
        IReadOnlyList<string>? options,
        string? pattern,
        double? min,
        double? max)
    {
        Key = key;
        Label = label;
        Type = type;
        Required = required;
        Options = options;
        Pattern = pattern;
        Min = min;
        Max = max;
    }
}
