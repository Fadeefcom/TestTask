using System.Text.Json;
using System.Text.RegularExpressions;
using FormSubmissions.Domain.Aggregates.FormAggregate;

namespace FormSubmissions.Domain.Services;

public sealed class SubmissionDomainService
{
    public void Validate(FormDefinition form, Dictionary<string, JsonElement> payload)
    {
        payload ??= new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);

        foreach (var field in form.Fields)
        {
            if (!payload.TryGetValue(field.Key, out var value))
            {
                if (field.Required)
                    throw new ArgumentException($"Missing required field: {field.Key}");
                continue;
            }

            if (field.Required && IsEmpty(value))
                throw new ArgumentException($"Field is required: {field.Key}");

            if (field.Options != null && field.Options.Count > 0)
            {
                var s = JsonToString(value);
                if (s != null && !field.Options.Contains(s, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid option for field: {field.Key}");
            }

            if (!string.IsNullOrWhiteSpace(field.Pattern))
            {
                var s = JsonToString(value);
                if (s != null && !Regex.IsMatch(s, field.Pattern))
                    throw new ArgumentException($"Invalid format for field: {field.Key}");
            }

            if (field.Min.HasValue || field.Max.HasValue)
            {
                if (value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out var d))
                {
                    if (field.Min.HasValue && d < field.Min.Value)
                        throw new ArgumentException($"Field below min: {field.Key}");
                    if (field.Max.HasValue && d > field.Max.Value)
                        throw new ArgumentException($"Field above max: {field.Key}");
                }
            }

            if (field.Type == FormFieldType.Checkbox && value.ValueKind != JsonValueKind.True && value.ValueKind != JsonValueKind.False)
                throw new ArgumentException($"Invalid boolean for field: {field.Key}");
        }
    }

    private static bool IsEmpty(JsonElement e)
    {
        return e.ValueKind switch
        {
            JsonValueKind.String => string.IsNullOrWhiteSpace(e.GetString()),
            JsonValueKind.Null => true,
            JsonValueKind.Undefined => true,
            JsonValueKind.Array => e.GetArrayLength() == 0,
            _ => false
        };
    }

    private static string? JsonToString(JsonElement e)
    {
        if (e.ValueKind == JsonValueKind.String)
            return e.GetString();
        if (e.ValueKind == JsonValueKind.Number)
            return e.GetRawText();
        if (e.ValueKind == JsonValueKind.True)
            return "true";
        if (e.ValueKind == JsonValueKind.False)
            return "false";
        return null;
    }
}
