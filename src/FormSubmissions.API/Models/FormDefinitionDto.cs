namespace FormSubmissions.API.Models;

public sealed record FormDefinitionDto(
    Guid Id,
    string Name,
    int Version,
    IReadOnlyList<FormFieldDefinitionDto> Fields
);

public sealed record FormFieldDefinitionDto(
    string Key,
    string Label,
    string Type,
    bool Required,
    IReadOnlyList<string>? Options,
    string? Pattern,
    double? Min,
    double? Max
);

public sealed record CreateFormDefinitionRequest(
    string Name,
    IReadOnlyList<FormFieldDefinitionDto> Fields
);
