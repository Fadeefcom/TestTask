namespace FormSubmissions.API.Models;

public sealed record SearchSubmissionsRequest(
    Guid? FormId,
    string? Query,
    DateTimeOffset? From,
    DateTimeOffset? To
);
