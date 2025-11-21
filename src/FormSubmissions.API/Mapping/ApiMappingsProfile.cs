using FormSubmissions.API.Models;
using FormSubmissions.Domain.Aggregates.FormAggregate;
using FormSubmissions.Domain.Aggregates.SubmissionAggregate;

namespace FormSubmissions.API.Mapping;

public static class ApiMappingsProfile
{
    public static FormDefinitionDto ToDto(FormDefinition form)
        => new(
            form.Id.Value,
            form.Name,
            form.Version.Value,
            form.Fields.Select(f => new FormFieldDefinitionDto(
                f.Key,
                f.Label,
                f.Type.ToString(),
                f.Required,
                f.Options,
                f.Pattern,
                f.Min,
                f.Max
            )).ToList()
        );

    public static SubmissionDto ToDto(Submission submission)
        => new(
            submission.Id.Value,
            submission.FormId.Value,
            submission.CreatedAt,
            new Dictionary<string, System.Text.Json.JsonElement>(submission.Payload)
        );
}
