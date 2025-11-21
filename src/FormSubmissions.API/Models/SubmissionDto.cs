using System.Text.Json;

namespace FormSubmissions.API.Models;

public sealed record SubmissionDto(
    Guid Id,
    Guid FormId,
    DateTimeOffset CreatedAt,
    Dictionary<string, JsonElement> Payload
);

public sealed record CreateSubmissionRequest(
    Guid FormId,
    Dictionary<string, JsonElement> Payload
);
