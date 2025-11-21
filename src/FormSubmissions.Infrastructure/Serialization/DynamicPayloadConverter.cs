using System.Text.Json;

namespace FormSubmissions.Infrastructure.Serialization;

public static class DynamicPayloadConverter
{
    public static string ToJson(Dictionary<string, JsonElement> payload)
        => JsonSerializer.Serialize(payload);

    public static Dictionary<string, JsonElement> FromJson(string json)
        => JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) 
           ?? new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
}
